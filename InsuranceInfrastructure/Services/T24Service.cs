using InsuranceCore.DTO;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Util;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsuranceInfrastructure.Services
{
    public class T24Service : IT24Service
    {
        private readonly IHttpClientService _httpClientService;
        private string BASEURL = "";
        private AppSettings _appsettings;
        private readonly ILoggingService _logging;
        private readonly IGenericRepository<FundTransferLookUp> _fundLog;
        private readonly IUtilityService _utilityService;

        public T24Service(IHttpClientService httpClientService, IUtilityService utilityService,
            IGenericRepository<FundTransferLookUp> fundLog, IOptions<AppSettings> ioptions, ILoggingService logging)
        {
            _httpClientService = httpClientService;
            _appsettings = ioptions.Value;
            _logging = logging;
            BASEURL = _appsettings.T24WsEndpoint;
            _fundLog = fundLog;
            _utilityService = utilityService;

        }

        public async Task<HttpResponseMessage> Postofs(string ofs, string transactionRef, string action, CancellationToken cancellation)
        {
            var ofsForDB = RemoveFTPassword(ofs);
            _logging.LogInformation(ofsForDB, "Postofs");

            if (_appsettings.AlwaysSerializeOFS == "Yes")
            {
                ofs = JsonConvert.SerializeObject(ofs);
            }
            

            var RequestTime = DateTime.Now;
            // Use the httpClientService to make POST request
            var ret = await _httpClientService.PostAsync<HttpResponseMessage>(/*_appsettings.T24BaseUrl*/ BASEURL +
                "transaction/postofs",
                ofs
               //, cancellation
            );
            var ResponseTime = DateTime.Now;

            var apilog = new GatewayLog
            {
                APIDatetime = DateTime.Now,
                Source = "transaction/postofs", 

                Request = ofsForDB,
                Response = ret.Content.ReadAsStringAsync().Result,
                TransReference = transactionRef,
                RequestTime = RequestTime,
                ResponseTime = ResponseTime,
                Endpoint = "postofs",
                APIType = action,

            };
            _logging.LogInformation($"{apilog}", "apilog");
            return ret;
        }


        public async Task<AccountEnquiry> NameEnquiry(string AccountNo, CancellationToken cancellation)
        {
            // ... Existing logic ...
            string username = await _utilityService.AESDecryptString(_appsettings.UsernameAuthAD, default(CancellationToken));
            string password = await _utilityService.AESDecryptString(_appsettings.PasswordAuthAD, default(CancellationToken));
            string baseurl = _appsettings.T24WsEndpoint;
            string secretKey = _appsettings.UserSecretKey;
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var nonce = Guid.NewGuid().ToString();
            string dataSign = $"{nonce}:{timeStamp}:{username}:{secretKey}";
            var hashString = GenerateSHA512String(dataSign);
            byte[] cred = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password));
            Dictionary<string, string> GenerateHeaders = new Dictionary<string, string>()
            {
                 { "Authorization", $"Basic {Convert.ToBase64String(cred)}" },
                 { "Accept", "application/json" },
                 { "Timestamp", timeStamp },
                 { "Nonce", nonce },
                 { "Signature", hashString },
                { "SignatureMethod", "SHA512" }
            };
            // Use the httpClientService to make GET request
            return await _httpClientService.GetAsync<AccountEnquiry>(/*_appsettings.T24BaseUrl*/  BASEURL +
                $"enquiry/account/basic/{AccountNo}",
                GenerateHeaders, cancellation

            //,cancellation
            );
        }
        public async Task<ApiResponse<AccountEnquiryResponse>> AccountEnquiry(string AccountNo, CancellationToken cancellation)
        {
            var T24username = await _utilityService.AESDecryptString(_appsettings.T24Username, default(CancellationToken));
            var T24password = await _utilityService.AESDecryptString(_appsettings.T24password, default(CancellationToken));

            String OFS = string.Format("ENQUIRY.SELECT,,{0}/{1}/,ACCOUNTENQUIRY,@ID:EQ", T24username, T24password);
            OFS += "=" + AccountNo;
            var res = await PostofsAsync(OFS);
            //var res = await Postofs(OFS, AccountNo, "NE", cancellation);
            String sResponse = res.Content.ReadAsStringAsync().Result;

            if (res.IsSuccessStatusCode && sResponse.Contains("/1,"))
            {
                try
                {
                    return new ApiResponse<AccountEnquiryResponse>
                    {
                        Data = new AccountEnquiryResponse
                        {
                            AccountBalance = Convert.ToDecimal(ParseSwiftChar(ParseSwiftChar(sResponse.Split('t')[4])).Replace("\\", "").Trim()),
                            AccountName = ParseSwiftChar(ParseSwiftChar(sResponse.Split('t')[2])).Replace("\\", "").Trim(),
                            PhoneNumber = ParseSwiftChar(ParseSwiftChar(sResponse.Split('t')[3])).Replace("\\", "").Trim(),
                            AccountNumber = AccountNo
                        },
                        Message = "Success",
                        Status = true
                    };
                }
                catch (Exception ex)
                {
                    _logging.LogError(ex.ToString(), "AccountEnquiry");
                    if (sResponse.Contains("No records were found"))
                    {
                        return new ApiResponse<AccountEnquiryResponse>
                        {
                            Data = null,
                            Message = "No record found",
                            Status = false
                        };
                    }
                    else
                    {
                        return new ApiResponse<AccountEnquiryResponse>
                        {
                            Data = null,
                            Message = "System Malfunction(01)",
                            Status = false
                        };
                    }
                }
            }
            else
            {
                return new ApiResponse<AccountEnquiryResponse>
                {
                    Data = null,
                    Message = "System Malfunction(02)",
                    Status = false
                };
            }
        }

        public async Task<bool> ReverseFundTransfer(string ftref)
        {


            try
            {
                //FUNDS.TRANSFER,PHB.GENERIC.ACTR/R/PROCESS//,ftid
                String OFS = "FUNDS.TRANSFER,PHB.GENERIC.ACTR/R/PROCESS,";
               
                var T24username = await _utilityService.AESDecryptString(_appsettings.T24Username, default(CancellationToken));
                var T24password = await _utilityService.AESDecryptString(_appsettings.T24password, default(CancellationToken));

                OFS += string.Format("{0}/{1}/,", T24username, T24password);
                //OFS += "MTOUSER01" + "/" + "Ab123456" + "/";
                OFS += ftref;
                var res = await Postofs(OFS, ftref, "FTReversal", default(CancellationToken));
                var sResponse = await res.Content.ReadAsStringAsync();


                //logTransaction(request.getTransRef(), sResponse, request.getCredential().getWebServiceUser(), "doAppzoneFundsTransfer");
                if (res.IsSuccessStatusCode && sResponse.Contains("/1"))
                {
                    sResponse = sResponse.Split('/')[0];
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {

                _logging.LogError($" ReverseFundTransfer : {ex.StackTrace}", "ReverseFundTransfer");
                return false;
            }


        }

        //public async Task<ApiResponse<TransferResponse>> ReverseBulkTransfer(TransactionLog transaction, CancellationToken cancellation)
        //{

        //    var T24username = _appsettings.T24Username;
        //    var T24password = _appsettings.T24password;
        //    //using (var db = new TeamAptFESDb())


        //    String OFS = "";
        //    String sResponse = "";
        //    try
        //    {
        //        OFS = $"FT.BULK.CREDIT.AC,PHB.ACTRBULK/I/PROCESS/1/0,{T24username}/{T24password},," +
        //               $"DR.ACCOUNT::={transaction.OriginatorAccountNumber}," +
        //               $"DR.CURRENCY::=NGN," +
        //               $"DR.AMOUNT::={transaction.Amount},";
        //        var creditAccounts = JsonConvert.DeserializeObject<List<CreditLeg>>(transaction.CreditAccounts);
        //        var id = 0;
        //        foreach (var cr in creditAccounts)
        //        {
        //            id = id + 1;


        //            OFS += $"CR.CURRENCY:1:1=NGN," +
        //            $"CR.ACCOUNT:{id}:1={cr.AccountNo}," +
        //            $"CR.AMOUNT:{id}:1={cr.Amount}," +
        //            $"CR.NARRATIVE:{id}:1={cr.Narration},";
        //        }
        //        OFS += $"PROFIT.CENTRE.DEPT::=4001";



        //        // Define the URL for ReverseBulkTransfer
        //        var res = await Postofs(OFS, transaction.TransactionRef, "FT", cancellation);
        //        sResponse = await res.Content.ReadAsStringAsync();


        //        //logTransaction(request.getTransRef(), sResponse, request.getCredential().getWebServiceUser(), "doAppzoneFundsTransfer");
        //        if (res.IsSuccessStatusCode && sResponse.Contains("/1"))
        //        {
        //            transaction.T24Reference = ((sResponse.Split('/')[0]).Substring(1, 14));
        //            transaction.FTReferences = ExtractFtRefs(sResponse);
        //            //   await db.SaveChangesAsync(cancellation);
        //            return new ApiResponse<TransferResponse>
        //            {
        //                Status = true,
        //                Message = "Successful",
        //                Data = new TransferResponse
        //                {
        //                    OurTransactionRef = transaction.T24Reference,
        //                    TransactionRef = transaction.TransactionRef,
        //                    TransferStatus = "Completed",
        //                    FtReferences = transaction.FTReferences


        //                }
        //            };



        //        }
        //        var message = "";

        //        if (sResponse.Contains("TXN AMT PLUS CHGS EXCEED BALANCE"))
        //        {
        //            message = "TXN AMT PLUS CHGS EXCEED BALANCE";
        //        }
        //        else if (sResponse.Contains("TOTAL DEBIT LESSER BY"))
        //        {
        //            var arr = sResponse.Split('=');
        //            var msg = arr[1].Substring(0, arr[1].Length - 1);
        //            message = msg;
        //        }
        //        else if (sResponse.Contains("POSSIBLE DUPLICATE CONTRACT"))
        //        {
        //            message = "Possible Duplicate Transaction";
        //        }
        //        else if (sResponse.Contains("ACCOUNT RECORD MISSING"))
        //        {
        //            message = "INVALID " + ParseSwiftChar((sResponse.Split(',')[1]));

        //        }
        //        else //ACCOUNT RECORD MISSING
        //        {
        //            message = sResponse;


        //        }

        //        return new ApiResponse<TransferResponse>
        //        {
        //            Status = false,
        //            Message = message,
        //            Data = new TransferResponse { TransferStatus = "Failed" }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.StackTrace, "ReverseBulkTransfer");

        //        return new ApiResponse<TransferResponse>
        //        {
        //            Status = false,
        //            Message = "Internal Server Error",
        //            Data = new TransferResponse { TransferStatus = "Processing" }

        //        };


        //    }

        //}
        //public async Task<TransactionQueryResponse> QueryBulkFt(string TransactionRef, CancellationToken cancellation)
        //{
        //    TransactionQueryResponse enq = new TransactionQueryResponse();

        //    try
        //    {
        //        string connectionString = ConfigurationManager.ConnectionStrings["ORADB"].ToString();
        //        var conn = new OracleConnection(connectionString);
        //        _logging.LogInformation($"QueryBulkFt from oracle   TransactionRef : {TransactionRef} using {conn.ConnectionString}", "QueryBulkFt");
        //        using (conn)
        //        {
        //            conn.Open();

        //            string query = @"select RECID,DR_CURRENCY, DR_AMOUNT, DR_ACCOUNT from T24LIVE.V_FBNK_FT_BULK_CREDIT_AC  where  RECID =  :TransactionRef";

        //            _logging.LogInformation("running query:..." + query, "QueryBulkFt");


        //            enq = await conn.QueryFirstOrDefaultAsync<TransactionQueryResponse>(query, new { TransactionRef });
        //            return enq;

        //            //logger.Info($"Query Returned for :{accountdetails.AccountNo}: {accountdetails.AccountName}");

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError($"Error QueryBulkFt  details for :{TransactionRef}. StackTrace:{ex.Message}{ex.StackTrace}", "QueryBulkFt");
        //        // throw (ex);
        //        return null;
        //    }
        //}
        public async Task<ApiResponse<TransferResponse>> FundTransfer1(TransactionLog transaction, CancellationToken cancellation)
        {
           
            var T24username = await _utilityService.AESDecryptString(_appsettings.T24Username, default(CancellationToken));
            var T24password = await _utilityService.AESDecryptString(_appsettings.T24password, default(CancellationToken));
            TransferResponse returnObj = new TransferResponse();

            String OFS = "";
            String sResponse = "";

            try
            {
                OFS = $"FT.CREDIT.AC,PHB.ACTR/I/PROCESS/1/0,{T24username}/{T24password},," +
                    $"DR.ACCOUNT::={transaction.OriginatorAccountNumber}," +
                    $"DR.CURRENCY::=NGN," +
                    $"DR.AMOUNT::={transaction.Amount},";

                // Assuming there is only one credit account in the single fund transfer

                OFS += $"CR.CURRENCY:1:1=NGN," +
                    $"CR.ACCOUNT:1:1={transaction.CreditAccounts.AccountNo}," +
                    $"CR.AMOUNT:1:1={transaction.CreditAccounts.Amount}," +
                    $"CR.NARRATIVE:1:1={transaction.CreditAccounts.Narration},";

                OFS += $"PROFIT.CENTRE.DEPT::=4001";

                var res = await Postofs(OFS, transaction.TransactionRef, "FT", cancellation);
                sResponse = await res.Content.ReadAsStringAsync();

                if (res.IsSuccessStatusCode && sResponse.Contains("/1"))
                {
                    transaction.T24Reference = ((sResponse.Split('/')[0]).Substring(1, 14));
                    transaction.FTReferences = ExtractFtRefs(sResponse);

                    return new ApiResponse<TransferResponse>
                    {
                        Status = true,
                        Message = "Successful",
                        Data = new TransferResponse
                        {
                            OurTransactionRef = transaction.T24Reference,
                            TransactionRef = transaction.TransactionRef,
                            TransferStatus = "Completed",
                            FtReferences = transaction.FTReferences
                        }
                    };
                }
                // Handle other error cases as needed...
                else
                {
                    return new ApiResponse<TransferResponse>
                    {
                        Status = false,
                        Message = "Handle your error cases here",
                        Data = new TransferResponse { TransferStatus = "Failed "}
                    };
                }
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.StackTrace, "FundTransfer");

                return new ApiResponse<TransferResponse>
                {
                    Status = false,
                    Message = "Internal Server Error",
                    Data = new TransferResponse { TransferStatus = "Processing" }
                };
            }
        }

        private string RemoveFTPassword(string oFS)
        {
            try
            {
                var StringToReplace = _appsettings.T24password;
              

                return oFS.Replace(StringToReplace, "XXXXX");
            }
            catch (Exception)
            {

                throw;
            }

        }
        public static string ParseSwiftChar(string sVal)
        {
            sVal = sVal.Replace("¬", "");
            sVal = sVal.Replace("`", "");
            sVal = sVal.Replace("!", "");
            sVal = sVal.Replace("\"", "");
            sVal = sVal.Replace("£", "");
            sVal = sVal.Replace("$", "");
            sVal = sVal.Replace("%", "");
            sVal = sVal.Replace("^", "");
            sVal = sVal.Replace("&", " and ");
            sVal = sVal.Replace("\\*", "");
            sVal = sVal.Replace("\\_", "");
            sVal = sVal.Replace("\\[", "");
            sVal = sVal.Replace("]", "");
            sVal = sVal.Replace("@", "");
            sVal = sVal.Replace("#", "");
            sVal = sVal.Replace(";", "");
            sVal = sVal.Replace(">", "");
            sVal = sVal.Replace("<", "");
            sVal = sVal.Replace("|", "");
            sVal = sVal.Replace("\\\\", "");
            sVal = sVal.Replace("=", "");
            sVal = sVal.Replace("\\{", "");
            sVal = sVal.Replace("}", "");
            sVal = sVal.Replace("'", ""); //database
            sVal = sVal.Replace(",", ""); //T24 OFS
            sVal = sVal.Replace("\\.", ""); //T24 OFS
            sVal = sVal.Replace("\\+", ""); //T24 OFS
            sVal = sVal.Replace("-", ""); //T24 OFS
            return sVal;
        }
        public string ExtractFtRefs(string sResponse)
        {
            string strList = "";
            try
            {
                var strArry = sResponse.Split(',');
                var ftaary = strArry.Where(x => x.StartsWith("OFS.GEN.ID"));
                foreach (var item in ftaary)
                {
                    var refid = item.Substring(item.Length - 16);
                    strList = strList + "|" + refid;
                }

            }
            catch (Exception ex)
            {

                _logging.LogError($"Unable to Get FTRefs from BulkFT response {ex.Message} {ex.InnerException} {ex.InnerException}", "ExtractFtRefs");

            }

            return strList;
        }

        //test


        public async Task<HttpResponseMessage> PostofsAsync(string ofs)

        {

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken cancellation = cancellationTokenSource.Token;

            //var ofsForDB = RemoveFTPassword(ofs);

            //_logging.LogInformation("Postofs Input:" + RemoveFTPassword(ofs));



            if ( _appsettings.AlwaysSerializeOFS.ToString() == "Yes")

            {

                ofs = JsonConvert.SerializeObject(ofs);

            }

            var RequestTime = DateTime.Now;

            HttpResponseMessage ret = new HttpResponseMessage();

            try

            {

                ret = await PostRequestAsync("transaction/postofs", ofs);

            }

            catch (Exception ex)

            {

                _logging.LogError(ex.ToString(), "PostofsAsync");

                throw;

            }



            var ResponseTime = DateTime.Now;



            //var apilog = new GatewayLog

            //{

            //    APIDatetime = DateTime.Now,

            //    Source = "POS", //"BalanceEnquiry",

            //    Request = ofsForDB,

            //    Response = ret.Content.ReadAsStringAsync().Result,

            //    TransReference = "",

            //    RequestTime = RequestTime,

            //    ResponseTime = ResponseTime,

            //    Endpoint = "postofs",

            //    APIType = "FT",



            //};



           // new GeneralService().LogGatewayRequest(apilog);







            //logger.Info("Postofs Result:" + await ret.Content.ReadAsStringAsync());

            return ret;

            // return response.Content.ReadAsStringAsync().Result;

        }

        public async Task<HttpResponseMessage> GetRequest(string url)

        {

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken cancellation = cancellationTokenSource.Token;

            String POST_URL = BASEURL + url;

            HttpResponseMessage response = await _httpClientService.GetAsync<HttpResponseMessage>(POST_URL,null, cancellation);   //client.GetAsync(POST_URL, cancellation);

            string resourcePath = string.Empty;

            string error = string.Empty;

            return response; //.Content.ReadAsStringAsync().Result;



        }

        public static string GenerateSHA512String(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(bytes);

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private async Task<HttpResponseMessage> PostRequestAsync(string url, string contentString)

        {

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CancellationToken cancellation = cancellationTokenSource.Token;

            HttpResponseMessage response = new HttpResponseMessage();

            HttpContent content = new StringContent(contentString, Encoding.UTF8, "application/json");

            string POST_URL = BASEURL + url;
            string username = await _utilityService.AESDecryptString(_appsettings.UsernameAuthAD, default(CancellationToken));
            string password = await _utilityService.AESDecryptString(_appsettings.PasswordAuthAD, default(CancellationToken));
            string baseurl = _appsettings.T24WsEndpoint;
            string secretKey = _appsettings.UserSecretKey;
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var nonce = Guid.NewGuid().ToString();
            string dataSign = $"{nonce}:{timeStamp}:{username}:{secretKey}";
            var hashString = GenerateSHA512String(dataSign);
            byte[] cred = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password));

            try

            {

                // response = await client.PostAsync(POST_URL, content);

                //contentString=@"";
                using (HttpClient client = new HttpClient())
                {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Timestamp", timeStamp);
                client.DefaultRequestHeaders.Add("Nonce", nonce);
                client.DefaultRequestHeaders.Add("Signature", hashString);
                client.DefaultRequestHeaders.Add("SignatureMethod", "SHA512");
                response = await client.PostAsync(POST_URL, content, cancellation);
            }


                //Dictionary<string, string> GenerateHeaders = new Dictionary<string, string>()
                //{
                //     { "Authorization", $"Basic {Convert.ToBase64String(cred)}" },
                //     { "Accept", "application/json" },
                //     { "Timestamp", timeStamp },
                //     { "Nonce", nonce },
                //     { "Signature", hashString },
                //    { "SignatureMethod", "SHA512" }
                //};
                //response = await _httpClientService.PostAsync<HttpResponseMessage>(POST_URL, contentString, GenerateHeaders); //  client.PostAsync(POST_URL, content);

                //string resourcePath = string.Empty;

                //string error = string.Empty;

            }

            catch (Exception ex)

            {

                _logging.LogError(ex.ToString(), "PostRequestAsync");

                throw;

            }



            return response;

        }

        public async Task<FundstransferResponse> FundTransfer(FundsTransferRequestDto request, Credential credential)
        {
            

            var T24username = credential.T24Username;
            var T24password = credential.T24password;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellation = cancellationTokenSource.Token;
            
            String OFS = "";
            String sResponse = "";


            FundstransferResponse response = new FundstransferResponse();
            var debitAccount = request.DebitAccount;



            try
            {




                var TransactionID = request.TransactionId;
                var RequestID = request.Id;



                //FUNDS.TRANSFER,PHB.GENERIC.ACTR/R/PROCESS//,ftid
                OFS = "FUNDS.TRANSFER,PHB.GENERIC.ACTR/I/PROCESS,";
                OFS += string.Format("{0}/{1}/{2},", T24username, T24password, request.BranchCode);
                OFS += ",DEBIT.ACCT.NO::=" + request.DebitAccount + ",DEBIT.CURRENCY::=" + request.DebitCurrency;
                OFS += ",DEBIT.AMOUNT::=" + request.Amount.ToString("#.00");
                OFS += ",CREDIT.CURRENCY::=" + request.CreditCurrency;
                //OFS += ",CREDIT.ACCT.NO::=" + request.getCreditAccount();
                OFS += ",CREDIT.ACCT.NO::=" + request.CreditAccount;



                String narration = ParseSwiftChar(request.Narration);
                if (narration.Length > 34)
                {
                    OFS += ",PAYMENT.DETAILS:1:1::=" + narration.Substring(0, 34);
                    String temp = narration.Substring(34);
                    if (temp.Length > 34)
                    {
                        OFS += ",PAYMENT.DETAILS:1:2::=" + temp.Substring(0, 34);
                        String temp1 = temp.Substring(34);
                        if (temp1.Length > 34)
                        {
                            OFS += ",PAYMENT.DETAILS:1:3::=" + temp1.Substring(0, 34);
                        }
                        else
                        {
                            OFS += ",PAYMENT.DETAILS:1:3::=" + temp1;
                        }
                    }
                    else
                    {
                        OFS += ",PAYMENT.DETAILS:1:2::=" + temp;
                    }
                }
                else
                {
                    OFS += ",PAYMENT.DETAILS:1:1::=" + narration;
                }
                
                // OFS += ",PROFIT.CENTRE.DEPT::=PROFIT.CENTRE.CUST";
                // OFS += ",COMMISSION.CODE::=WAIVE";
                 OFS += ",COMMISSION.CODE::=DEBIT PLUS CHARGES";
                //OFS += ",COMMISSION.CODE::=CREDIT LESS CHARGE";
                //OFS += ",COMMISSION.AMT::=NGN" + request.Commission;
                 OFS += ",COMMISSION.TYPE::=" + _appsettings.COMMISSIONTYPE;
                OFS += ",INTERF.REF::=IBTR" + _appsettings.INTERFREF;
               // OFS += ",CREDIT.THEIR.REF::=" + request.TransactionId;
               // OFS += ",UNIQUE.IDF::=" + request.TransactionId;
                OFS += ",UNIQUE.IDF::=" + request.UnquieId;
                OFS += ",DISTRIB.NAME::=" + _appsettings.DISTRIBNAME; /* ParseSwiftChar(request.Channel)*/;
                request.TransactionCode = _appsettings.TRANSACTIONTYPE /*"ACCC"*/;
                OFS += ",TRANSACTION.TYPE::=" + request.TransactionCode;
                OFS += ",CR.DOC.ID::=" + request.TransactionRef;// Session id for NIP. fresh each time
                OFS += ",DR.DOC.ID::=" + request.TransactionRef;// Session id for T24 duplication check
                                                                // OFS = "FUNDS.TRANSFER,PHB.GENERIC.ACTR/I/PROCESS,MTOUSER01/Ab123456/,,DEBIT.ACCT.NO::=6020209072,DEBIT.CURRENCY::=NGN,DEBIT.AMOUNT::=5000.00,CREDIT.CURRENCY::=NGN,CREDIT.ACCT.NO::=6020733832,PAYMENT.DETAILS:1:1::=Test ARCA FT,COMMISSION.CODE::=WAIVE,INTERF.REF::=IBTR,CREDIT.THEIR.REF::=TESTARCAREF,UNIQUE.IDF::=TESTARCAREF,DISTRIB.NAME::=ARCA,TRANSACTION.TYPE::=ACCC,CR.DOC.ID::=TESTARCAREF,DR.DOC.ID::=TESTARCAREF";

                 OFS += ",CO.CODE::=" + request.BranchCode;





                var res = await PostofsAsync(OFS);
                sResponse = await res.Content.ReadAsStringAsync();
                var lookup = new FundTransferLookUp()
                {
                    RequesstDate = DateTime.Now,
                    TransactionRequest = JsonConvert.SerializeObject(OFS).Replace(T24password, "[T24PASSWORD]").Replace(T24username , "[T24USER]"),
                    TransactionResponse = JsonConvert.SerializeObject(sResponse),
                    TransactionStatus = ParseSwiftChar((sResponse.Split(',')[1])),
                    TransactionNarration = request.Narration,
                    UniqueID = request.UnquieId,
                    TransactionType = request.TransactionType,
                    RequestID = request.RequestID,
                    InsuranceTableId = request.InsuranceTableId

                };

                _fundLog.Insert(lookup);



                //logTransaction(request.getTransRef(), sResponse, request.getCredential().getWebServiceUser(), "doAppzoneFundsTransfer");
                if (res.IsSuccessStatusCode && sResponse.Contains("/1"))
                {
                    sResponse = sResponse.Split('/')[0];
                    // response.ResponseCode = (ResponseCodes.APPROVED_OR_COMPLETED_SUCCESSFULLY);
                    response.Status = true;
                    response.ResponseMessage = "Success";
                    request.TransactionRef = (sResponse.Substring(1));
                    request.TransactionId = (request.TransactionId);
                    response.TransactionReference = (sResponse.Substring(1));
                   // response.ReponseReference = GeneralService.GenerateResponseReference();

                }
                else if (sResponse.Contains("TXN AMT PLUS CHGS EXCEED BALANCE"))
                {
                   // response.ResponseCode = (ResponseCodes.NO_SUFFICIENT_FUNDS);
                    response.ResponseMessage = ("TXN AMT PLUS CHGS EXCEED BALANCE");
                    request.TransactionRef = "";
                    request.TransactionId = (request.TransactionId);
                    response.TransactionReference = "";
                    response.Status = false;
                    //response.ReponseReference = GeneralService.GenerateResponseReference();


                }
                else if (sResponse.Contains("POSSIBLE DUPLICATE CONTRACT"))
                {
                   // response.ResponseCode = (ResponseCodes.DUPLICATE_TRANSACTION);
                    response.ResponseMessage = ("POSSIBLE DUPLICATE CONTRACT");
                    request.TransactionRef = ParseSwiftChar((sResponse.Split('/')[0]));
                    request.TransactionId = (request.TransactionId);
                    response.TransactionReference = ParseSwiftChar((sResponse.Split('/')[0]));
                    response.Status = false;
                    // response.ReponseReference = GeneralService.GenerateResponseReference();


                }
                else if (sResponse.Contains("ACCOUNT RECORD MISSING"))
                {
                   // response.ResponseCode = (ResponseCodes.INVALID_ACCOUNT);
                    response.ResponseMessage = "INVALID " + ParseSwiftChar((sResponse.Split(',')[1])); 
                    request.TransactionRef = "";
                    request.TransactionId = (request.TransactionId);
                    response.TransactionReference = "";
                    response.Status = false;
                    // response.ReponseReference = GeneralService.GenerateResponseReference();


                }
                else //ACCOUNT RECORD MISSING
                {
                   // response.ResponseCode = (ResponseCodes.SYSTEM_MALFUNCTION);
                    request.TransactionRef = "";
                    response.ResponseMessage = (sResponse);
                    request.TransactionId = (request.TransactionId);
                    response.TransactionReference = "";
                    response.Status = false;
                    // response.ReponseReference = GeneralService.GenerateResponseReference();


                }

            }



            catch (Exception ex)
            {
                // response.ResponseCode = (ResponseCodes.SYSTEM_MALFUNCTION);
                response.Status = false;
                request.TransactionRef = "";
                response.ResponseMessage = ex.Message.ToString();

            }




            
            



            return response;




        }


        public async Task<ApiResponse<string>> LockFunds(string AccountNo, string description, decimal amount, Credential credential)
        {
        var T24username = credential.T24Username;
        var T24password = credential.T24password;
        LockResponse returnObj = new LockResponse();

            String OFS = "";
            String sResponse = "";

            try
            {
                OFS = $"AC.LOCKED.EVENTS,PHB.OFS/I/PROCESS,{T24username}/{T24password}/,," +
                    $"ACCOUNT.NUMBER::={AccountNo},DESCRIPTION::={ description},LOCKED.AMOUNT::={ amount}";

                var res = await PostofsAsync(OFS);

                sResponse = await res.Content.ReadAsStringAsync();

                if (res.IsSuccessStatusCode && sResponse.Contains("/1"))
                {
                  

                    return new ApiResponse<string>
                    {
                        Status = true,
                        Message = "Successful",
                        Data = sResponse
                    };
                }
                // Handle other error cases as needed...
                else
                {
                    return new ApiResponse<string>
                    {
                        Status = false,
                        Message = "Handle your error cases here"
                        
                    };
                }
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.StackTrace, "LockFunds");

                return new ApiResponse<string>
                {
                    Status = false,
                    Message = "Internal Server Error",
                    Data = ex.Message
                };
            }
        }

       
    }
}
