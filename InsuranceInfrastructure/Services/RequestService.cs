using InsuranceCore.DTO;
using InsuranceCore.Enums;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Helpers;
using InsuranceInfrastructure.Util;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static InsuranceCore.DTO.ReusableVariables;

namespace InsuranceInfrastructure.Services
{
    public class RequestService : IRequestService
    {
        private readonly IGenericRepository<Request> _reqRepo;
        private readonly IGenericRepository<InsuranceTable> _InsuranceTbRepo;
        private readonly ILoggingService _logger;
        private readonly IGenericRepository<Underwriter> _WriterRepo;
        private readonly IGenericRepository<InsuranceType> _typeRepo;
        private readonly IGenericRepository<InsuranceSubType> _subTypeRepo;
        private readonly ISessionService _service;
        private readonly GlobalVariables _globalVariables;
        private readonly IEmailService _emailService;
        private AppSettings _appsettings;
        private readonly IT24Service _t24;
        private readonly IGenericRepository<Broker> _brokerRepo;
        public readonly IUtilityService _utilityService;
        private readonly IOracleDataService _oracleDataService;
        public readonly IGenericRepository<BrokerInsuranceType> _brokerinsuranceTypeRepo;
        public readonly IGenericRepository<BrokerSubInsuranceType> _brokerinsuranceSubTypeRepo;
        private readonly IHttpClientService _httpClientService;
        public readonly IAumsService _aum;

        public RequestService(ILoggingService logging, IGenericRepository<Request> reqRepo, IAumsService aum,
            IUtilityService utilityService, IHttpClientService httpClientService,
            IGenericRepository<InsuranceType> typeRepo, IGenericRepository<Underwriter> WriterRepo, IOracleDataService oracleDataService,
            ISessionService service, IEmailService emailService, IOptions<AppSettings> ioptions, IGenericRepository<InsuranceTable> InsuranceTbRepo,
            IT24Service t24, IGenericRepository<Broker> brokerRepo, IGenericRepository<InsuranceSubType> subTypeRepo
            , IGenericRepository<BrokerInsuranceType> brokerinsuranceTypeRepo,
            IGenericRepository<BrokerSubInsuranceType> brokerinsuranceSubTypeRepo)
        {
            _aum = aum;
            _httpClientService = httpClientService;
            _InsuranceTbRepo = InsuranceTbRepo;
            _reqRepo = reqRepo;
            _brokerRepo = brokerRepo;
            _logger = logging;
            _brokerinsuranceSubTypeRepo = brokerinsuranceSubTypeRepo;
            _brokerinsuranceTypeRepo = brokerinsuranceTypeRepo;
            _WriterRepo = WriterRepo;
            _typeRepo = typeRepo;
            _service = service;
            _emailService = emailService;
            _globalVariables = _service.Get<GlobalVariables>("GlobalVariables");
            _appsettings = ioptions.Value;
            _t24 = t24;
            _subTypeRepo = subTypeRepo;
            _utilityService = utilityService;
            _oracleDataService = oracleDataService;
        }

        public async Task<List<RecordReport>> SearchWithParameters(string searchParam, int start, int limit = 0, string filter = "")
        {
            try
            {
                IQueryable<Request> baseQuery = _reqRepo.GetBaseQuerywithInclude(r => r.Broker,
                    r => r.Underwriter,
                    r => r.InsuranceType.InsuranceType,
                    r => r.InsuranceSubType
                    );


                if (!string.IsNullOrEmpty(searchParam))
                {
                    baseQuery = baseQuery.Where(r => r.AccountNo.Equals(searchParam) ||
                    r.Underwriter.Name.ToLower().Equals(searchParam.ToLower()) || r.Broker.BrokerName.ToLower().Equals(searchParam.ToLower()) /*|| r.PolicyExpiryDate.Equals(searchParam)*/ ||
                    r.CustomerEmail.ToLower().Equals(searchParam.ToLower()) /*|| r.Stage.Equals(searchParam)*/ || r.CustomerID.Equals(searchParam)

                    );
                }

                return await ProcessInsuranceReport(baseQuery, start, limit);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.ToString(), "SearchWithParameters");
                return null;
            }
        }
        private async Task<List<RecordReport>> ProcessInsuranceReport(IQueryable<Request> query, int start, int limit)
        {
            try
            {
                var request = query.OrderByDescending(d => d.ID).Skip(start).Take(limit).ToList();
                List<RecordReport> records = new List<RecordReport>();
                //foreach (var item in request)
                for (int item = 0; item < request.Count; item++)
                {
                    var insurance = await _InsuranceTbRepo.GetWithIncludeAsync(x => (x.RequestID == request[item].RequestID)
                    , x => x.fundTransferLookUps);
                    string premiumUniqueID = "";
                    string commissionUniqueID = "";
                    decimal commissionAmount = 0;

                    if (insurance != null)
                    {
                        foreach (var i in insurance.fundTransferLookUps)
                        {

                            if (i.TransactionType == "Commission")
                            {
                                commissionUniqueID = i.UniqueID;
                                commissionAmount = _utilityService.GetDebitValue(i.TransactionRequest);
                            }
                            else if (i.TransactionType == "InsuranceRequest")
                            {
                                premiumUniqueID = i.UniqueID;
                            }

                        };
                    }
                   

                    TimeSpan difference = Convert.ToDateTime(insurance.PolicyExpiryDate) - Convert.ToDateTime(insurance.PolicyIssuanceDate);


                    var record = new RecordReport
                    {
                        RecordId = Convert.ToInt64(request[item].RequestID),
                        ContractId = request[item].ContractID,
                        AccountNumber = request[item].AccountNo,
                        AccountName = request[item].AccountName,
                        InsuranceType = request[item].InsuranceType.InsuranceType.Name,
                        DebitPassed = insurance.Serial,
                        ContractMaturityDate = request[item].ContractMaturityDate,
                        CustomerName = request[item].CustomerName,
                        CollateralValue = request[item].CollateralValue,
                        CustomerEmail = request[item].CustomerEmail,
                        SubInsuranceType = request[item].InsuranceSubType?.Name,
                        EstimatedPremium = request[item].UpdatedPremium,
                        Underwriter = request[item].Underwriter?.Name,
                        Broker = request[item].Broker.BrokerName,
                        CustomerID = request[item].CustomerID,
                        InsuranceFlag = insurance.Stage,
                        Certificate = $"<a href='/Insurance/DownloadCertificate/?RequestId={Convert.ToInt64(request[item].RequestID)}' class='btn btn-raised btn-primary waves-effect btn-round'>{insurance.FileName}</a>",
                        //Certificate = insurance.FileName,
                        PolicyNo = insurance.PolicyNo,
                        CommissionAmount = commissionAmount,
                        DateofIssuance = insurance.PolicyIssuanceDate,
                        PolicyExpiryDate = insurance.PolicyExpiryDate,
                        CommissionUniqueID = commissionUniqueID,
                        PremiumAmount = request[item].UpdatedPremium,
                        PremiumCRAccount = request[item].Broker.AccountNumber,
                        PremiumDRAccount = request[item].AccountNo,
                        PremiumUniqueID = premiumUniqueID,
                        Stage = insurance.Stage,
                        Branch = request[item].Branchcode,
                        InsuranceStatus= insurance.RequestType,
                        PolicyDuration = $"{difference.Days} Days",
                        Id= item
                    };
                    var test = JsonConvert.SerializeObject(record);

                    records.Add(record);
                };
                return records;

                //List<RecordReport> insuranceDataList = test.Select(x => new RecordReport
                //{
                //    Disable = x.IsActive
                //    ? $"<span class='btn btn-success btn-xs deleteint' data-keyg='{x.Id}'> <i class='glyphicon glyphicon-save'></i> Enable</span>"
                //    : $"<span class='btn btn-danger btn-xs deleteint' data-keyg='{x.Id}'> <i class='glyphicon glyphicon-save'></i> Disable</span>",

                //    //Disable = x.IsActive ? $"<span class='btn btn-primary btn-xs deleteint' data-keyg='{x.Id}'> <i class='glyphicon glyphicon-save'>Disable</i></span>" :
                //    //         $"<span class='btn btn-primary btn-xs deleteint' data-keyg='{x.Id}'> <i class='glyphicon glyphicon-save'>Enable</i></span>",
                //    View = "<a href='Insurance/GetInsurance/?anc=" + Ccheckg.convert_pass2($"pc={x.Id}", 0) + "' class='btn btn-raised btn-primary waves-effect btn-round'>View</a>",
                //    //Id = x.Id,
                //    InsuranceTrackingId = x.InsuranceTrackingId,
                //    PolicyNumber = x.PolicyNumber,
                //    CollateralValue = x.CollateralValue,
                //    Broker = x.Broker?.BrokerName,
                //    Underwriter = x.Underwriter?.Name,
                //    InsuranceCertificate = x.InsuranceCertificate?.CertificateFile,
                //    Customer = x.CustomerName,
                //    ContractId = x.ContractId,
                //    ContractFlag = x.ContractFlag,
                //    DebitsPassed = x.DebitsPassed,
                //    EstimatedPremium = x.EstimatedPremium,
                //    InsuranceType = x.InsuranceType?.Name,
                //    SubType = x.SubType?.Name,
                //    Comment = x.Comment,
                //    InsuranceFlag = x.InsuranceFlag,
                //    AccountName = x.AccountName,
                //    CustomerName = x.CustomerName,
                //    CustomerEmail = x.CustomerEmail,
                //    CustomerId = x.T24CustomerID,
                //    AccountNumber = x.AccountNumber,
                //    PolicyExpiryDate = x.PolicyExpiryDate,
                //    PolicyStartDate = x.PolicyStartDate,
                //    Stage = x.Stage,
                //    ContractMaturityDate = x.ContractMaturityDate,
                //    ErrorMessage = x.ErrorMessage,
                //    IsActive = x.IsActive
                //}).ToList();

                //  return query.OrderByDescending(d => d.Id).Skip(start).Take(limit)

                //.Select(x => new InsuranceData
                //{

                //    Disable = x.IsActive ? $"<span class='btn btn-primary btn-xs deleteint' data-keyg='{x.Id}'> <i class='glyphicon glyphicon-save'>Disable</i></span>" :
                //         $"<span class='btn btn-primary btn-xs deleteint' data-keyg='{x.Id}'> <i class='glyphicon glyphicon-save'>Enable</i></span>",
                //    View = "<a href='Insurance/GetInsurance/?anc=" + Ccheckg.convert_pass2($"pc={x.Id}", 0) + "' class='btn btn-raised btn-primary waves-effect btn-round'>View</a>",
                //    Id = x.Id,
                //    InsuranceTrackingId = x.InsuranceTrackingId,
                //    PolicyNumber = x.PolicyNumber,
                //    CollateralType = x.CollateralType,
                //    CollateralValue = x.CollateralValue,
                //    Broker = x.Broker.BrokerName,
                //    Underwriter = x.Underwriter.Name,
                //    InsuranceCertificate = x.InsuranceCertificate.CertificateFile,
                //   // InsuranceCertificate = $"<a href='Insurance/DownloadFile/?fileName={x.InsuranceCertificate.CertificateFile}'>CertificateFile</a>",
                //    Customer = x.CustomerName,
                //    ContractId = x.ContractId,
                //    ContractFlag = x.ContractFlag,
                //    DebitsPassed = x.DebitsPassed,
                //    EstimatedPremium = x.EstimatedPremium,
                //    InsuranceType = x.InsuranceType.Name,
                //    SubType = x.SubType.Name,
                //    Comment = x.Comment,
                //    InsuranceFlag = x.InsuranceFlag,
                //    AccountName = x.AccountName,
                //    CustomerName = x.CustomerName,
                //    CustomerEmail = x.CustomerEmail,
                //    CustomerId = x.T24CustomerID,
                //    AccountNumber = x.AccountNumber.ToString(),
                //    PolicyExpiryDate = x.PolicyExpiryDate,
                //    PolicyStartDate = x.PolicyStartDate,
                //    Stage = x.Stage,
                //    ContractMaturityDate = x.ContractMaturityDate,
                //    ErrorMessage = x.ErrorMessage,
                //    IsActive = x.IsActive,
                //}).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "ProcessInsuranceReport");
                return null;
            }

        }

        public async Task<List<RecordReport>> ListAllWithParameters(int start, int limit = 0, string filter = "", int TenantId = 0)
        {
            try
            {
                // IQueryable<Request> query = _reqRepo.GetBaseQuery();
                IQueryable<Request> query = _reqRepo.GetBaseQuerywithInclude(r => r.Broker,
                    r => r.Underwriter,
                    r => r.InsuranceType.InsuranceType,
                    r => r.InsuranceSubType
                    );
                return await ProcessInsuranceReport(query, start, limit);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.ToString(), "ListAllWithParameters");
                return null;
            }
        }

        public string UpdateRequest(Request request, InsuranceTable Insurance, string email, string name, string comment)
        {
            try
            {
                var update = _reqRepo.Update(request);
                var updateInsurance = _InsuranceTbRepo.Update(Insurance);
                if (update == false && updateInsurance == false) return "UnSuccessfully";

                string emailBodyRequest = 
                          $"The Insurance request with details below has been {request.Status}  .<br><br>" +
                          "Details:<br>" +
                        
                          $"Account Name : {request.AccountName} <br>" +
                          $"Account Number {request.AccountNo}<br>" +
                          $"Premium: {request.UpdatedPremium}<br>" +
                          $"Comment: {comment}<br><br>" +
                          "Thank you for using our services.<br>";
                         
                string body = _utilityService.BuildEmailTemplate(name, "New insurance request assigned", emailBodyRequest);

                _emailService.SmtpSendMail(email, body, "Insurance Request", _appsettings.SourceEmail);

                return "Successfully";


            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex.ToString(), "UpdateRequest");
                return "Error";
            }

        }


        public async Task<Underwriter> GetUnderwritebyId(int requestid)
        {
            try
            {
                var getrequest = await _WriterRepo.GetWithPredicate(r => r.Id == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex.ToString(), "GetUnderwritebyId");
                return null;
            }

        }
        public async Task<InsuranceType> GetInsuranceTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _typeRepo.GetWithPredicate(r => r.Id == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex.ToString(), "GetInsuranceTypebyId");
                return null;
            }

        }

        public async Task<BrokerInsuranceType> GetBrokerInsuranceTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _brokerinsuranceTypeRepo.GetWithPredicate(r => r.Id == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex.ToString(), "GetBrokerInsuranceTypebyId");
                return null;
            }

        }
        public async Task<List<Request>> GetAllNeededforAuth()
        {
            var requests = new List<Request>();
            var insuranceRequests = await _InsuranceTbRepo.GetAllWithPredicate(
                r => r.Stage == InsuranceStage.New.ToString() && r.RequestByUsername != _globalVariables.userName
                );
            foreach (var item in insuranceRequests)
            {
                var request = await _reqRepo.GetWithIncludeAsync(
                    x => (x.RequestID == item.RequestID &&   x.Status != CommentStatus.Closed.ToString() /*x.Status == BrokerStatus.Active.ToString()*/),
                    x => x.Broker,
                    x => x.InsuranceType.InsuranceType,
                    x => x.InsuranceSubType,
                    x => x.Underwriter

                    );
                requests.Add(request);
            }
            return requests;
        }
        public async Task<IEnumerable<Request>> GetAllNeeded1(string stage)
        {
            var requests = new List<Request>();
            var insuranceRequests = await _InsuranceTbRepo.GetAllWithPredicate(
                r => r.Stage == stage
                );
            foreach (var item in insuranceRequests)
            {
                var request = await _reqRepo.GetWithIncludeAsync(
                    x => (x.RequestID == item.RequestID && x.Status  == RequestStatus.Approved.ToString()/*!= CommentStatus.Closed.ToString()*/),
                    x => x.Broker,
                    x => x.InsuranceType.InsuranceType,
                    x => x.InsuranceSubType,
                    x => x.Underwriter
                    );
                requests.Add(request);
            }
            return requests;
        }
        public string FormatSerial(int serial)
        {
            if (serial < 10)
            {
                return $"00{serial}";
            }
            else if (serial < 100)
            {
                return $"0{serial}";
            }
            else
            {
                return serial.ToString();
            }
        }

        public async Task<string> AuthorizeInsurance(InsuranceTable request)
        {
            try
            {
                var getrequest = await _reqRepo.GetWithIncludeAsync(
                    x => (x.RequestID == request.RequestID &&  x.Status != CommentStatus.Closed.ToString()/*x.Status == BrokerStatus.Active.ToString()*/),
                    x => x.Broker,
                    x => x.InsuranceType,
                    x => x.InsuranceSubType
                    );
                //var getrequest = await _reqRepo.GetWithPredicate(x=> x.RequestID == request.RequestID);
                // var broker = await _brokerRepo.GetWithPredicate(x => x.Id == getrequest.BrokerID);

                Random random = new Random();
                var insuranceTypeId = await GetPercentageAsync(getrequest);
                var estimatedPremium = getrequest.UpdatedPremium;

                var commission = Convert.ToDecimal((estimatedPremium * insuranceTypeId) / 100);
                 
                //string formattedDateTime = DateTime.Now.ToString("yy/MM/dd HHmmss");
                string Serial = FormatSerial(request.Serial);
                var transfer = new FundsTransferRequestDto()
                {
                    TransactionType = TransactionType.InsuranceRequest.ToString(),
                    Amount = Convert.ToDecimal(getrequest.UpdatedPremium),
                    DebitAccount = getrequest.AccountNo,
                    Channel = _appsettings.Channel,
                    Narration = $"PREMIUM FOR INSURANCE ISSUANCE FOR   COLLATERAL {getrequest.CollateralValue} ",
                    CreditAccount = getrequest.Broker.AccountNumber,
                    BranchCode = getrequest.Branchcode,
                    CreditCurrency = "NGN",
                    DebitCurrency = "NGN",
                    TransactionCode = _appsettings.Channel,
                    TransactionId = $"{getrequest.UpdatedPremium}-{getrequest.ID}-{GenerateRandomString(3)}",
                    Id = Convert.ToInt64(request.Serial),
                    TransactionRef = $"F{getrequest.RequestID}",
                    UnquieId = $"FEES-{request.RequestID}-{Serial}",
                    RequestID = request.RequestID,
                    InsuranceTableId = request.ID


                };
                request.FEESFTReference = transfer.UnquieId;
                var credential = new Credential
                {
                    T24password = _appsettings.T24password,
                    T24Username = _appsettings.T24Username
                };

                var fundtrans = await _t24.FundTransfer(transfer, credential);
                _logger.LogInformation(fundtrans.ToString(), $"FundTransfer to Broker Account ={getrequest.Broker.AccountNumber}");
                var requery = await _oracleDataService.RequeryFTUniqueId(transfer.UnquieId);
                _InsuranceTbRepo.Update(request);
                if (requery == null) return "UnSuccessful";
                if (fundtrans.Status == true)
                {

                    var paycommission = new FundsTransferRequestDto()
                    {
                        TransactionType = TransactionType.Commission.ToString(),
                        Amount = commission,
                        DebitAccount = getrequest.Broker.AccountNumber,
                        Channel = _appsettings.Channel,
                        Narration = $"COMMISSION ON ISSURANCE ISSUANCE {request.RequestID} {request.Serial} {getrequest.CustomerName}",/*.Replace("-", "/"),*/
                        CreditAccount = _appsettings.GLAccountNumber,
                        BranchCode = null,
                        CreditCurrency = "NGN",
                        DebitCurrency = "NGN",
                        TransactionCode = _appsettings.Channel,
                        TransactionId = $"{commission}-{getrequest.ID}-{GenerateRandomString(3)}",
                        Id = Convert.ToInt64(request.Serial),
                        TransactionRef = $"C{request.RequestID}",
                        UnquieId = $"COMM-{request.RequestID}-{Serial}",
                        RequestID = request.RequestID,
                        InsuranceTableId = request.ID

                    };
                    request.COMMFTReference = paycommission.UnquieId;
                    var fundtranstoglaccount = await _t24.FundTransfer(paycommission, credential);
                    _logger.LogInformation(fundtrans.ToString(), $"FundTransfer to GL account ={_appsettings.GLAccountNumber}");
                    var requeryComm = await _oracleDataService.RequeryFTUniqueId(paycommission.UnquieId);
                    _InsuranceTbRepo.Update(request);
                    if (requeryComm == null) return "UnSuccessful";
                    if (fundtranstoglaccount.Status == true)
                    {
                        var lockfund = await _t24.LockFunds(paycommission.DebitAccount, "Lock " + paycommission.UnquieId, paycommission.Amount, credential);
                        _logger.LogInformation($"{ paycommission.Amount} lock with unquieid {paycommission.UnquieId},", $"lockfund in broker account ={paycommission.DebitAccount}");

                        request.Stage = InsuranceStage.UnderwriterAssigned.ToString();
                        _InsuranceTbRepo.Update(request);

                        string emailSubject = $"Payment for Commission on Insurance was successful for {getrequest.CustomerName}";

                        string emailBody = $"Dear {getrequest.Broker.AccountName},<br><br>" +
                       $"We are pleased to inform you that the payment for Commission on Insurance for {getrequest.CustomerName} has been successfully processed.<br><br>" +
                       "Details:<br>" +
                       "Transaction Type: COMMISSION<br>" +
                       $"Customer Name: {getrequest.CustomerName}<br>" +
                       $"Amount: #{paycommission.Amount}<br>" +
                       $"Unique Number: {paycommission.UnquieId}<br><br>" +
                       "Thank you for using our services.<br>" +
                       "Sincerely,<br>" +
                       "Keystone Bank";

                        // _emailService.SmtpSendMail(getrequest.Broker.EmailAddress, emailBody, emailSubject);


                        //  _emailService.SmtpSendMail(broker.EmailAddress, $"Request for Commission on Insurance was successful. see detail{request.CustomerName}", "Commission on Insurance");
                    }
                    if (fundtranstoglaccount.Status == false)
                    {
                        request.Stage = InsuranceStage.AuthorizeRequest.ToString();
                        request.ErrorMessage = fundtranstoglaccount.ResponseMessage;
                        request.Status = TransferStatus.Failed.ToString();

                       
                        string sanitizedResponseMessage = fundtranstoglaccount.ResponseMessage
                            .Replace("\r", "")   
                            .Replace("\n", "<br>"); 

                        string mailTemplate = "Dear User,<br><br>" +
                            "We are writing to inform you that your transaction has failed due to the following reason:<br><br>" +
                            "Reason:<br>" + $"{sanitizedResponseMessage}<br><br>" +
                            "Please contact our support team for further assistance.<br>" +
                            "via phone {+234 700 2000 3000} or e-mail {contactcentre@keystonebankng.com}<br><br>" +
                            "Sincerely,<br>" +
                            "Keystone Bank";

                        string bodytemp = _utilityService.BuildEmailTemplate(request.RequestByName, "Commission on Insurance", mailTemplate);


                        _emailService.SmtpSendMail(_globalVariables.Email, bodytemp, "Commission on Insurance");

                        //_emailService.SmtpSendMail(request.CustomerEmail, mailTemplate /*$"transaction failed because{fundtrans.ResponseMessage}"*/, "Commission on Insurance");
                    }
                    request.Stage = InsuranceStage.UnderwriterAssigned.ToString();
                    var updateRequest = _InsuranceTbRepo.Update(request);
                    string emailSubjectRequest = $"Payment Request for  Insurance was successful for {getrequest.CustomerName}";

                    string emailBodyRequest = 
                       $"We are pleased to inform you that the request for Payment on Insurance for {getrequest.CustomerName} has been successfully processed.<br><br>" +
                       "Details:<br>" +
                       "Transaction Type: PAYMENT<br>" +
                       $"Customer Name: {getrequest.CustomerName}<br>" +
                       $"Amount: #{transfer.Amount}<br>" +
                       $"Unique Number: {transfer.UnquieId}<br><br>" +
                       "Thank you for using our services.<br>";
                       

                    string body = _utilityService.BuildEmailTemplate(getrequest.Broker.AccountName, "Payment Request for  Insurance was successful", emailBodyRequest);

                    _emailService.SmtpSendMail(getrequest.Broker.EmailAddress, body, emailSubjectRequest);
                    // _emailService.SmtpSendMail(broker.EmailAddress, $"Payment Request for  Insurance was successful. see detail{request.CustomerName}", "Payment on Insurance");

                    return "Successfully";
                }

                if (fundtrans.Status == false)
                {
                    request.Stage = InsuranceStage.AuthorizeRequest.ToString();
                    request.ErrorMessage = fundtrans.ResponseMessage;
                    request.Status = TransferStatus.Failed.ToString();
                    var updateRequest = _InsuranceTbRepo.Update(request);
                    
                    string sanitizedResponseMessage = fundtrans.ResponseMessage
                        .Replace("\r", "")   
                        .Replace("\n", "<br>");

                    string mailTemplate = "Dear User,<br><br>" +
                        "We are writing to inform you that your transaction has failed due to the following reason:<br><br>" +
                        "Reason:<br>" + $"{sanitizedResponseMessage}<br><br>";
                        
                    string body = _utilityService.BuildEmailTemplate(_globalVariables.name, "Debit Notification For Insurance  Process", mailTemplate);

                    _emailService.SmtpSendMail(_globalVariables.Email, body, "Debit Notification For Insurance  Process");

                    // _emailService.SmtpSendMail(request.CustomerEmail, mailTemplate", "Debit Notification For Insurance  Process");
                }
                return "Unsucessful";
            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex.ToString(), "AuthorizeRequest");
                request.ErrorMessage = ex.InnerException.ToString();
                var updateRepuest = _InsuranceTbRepo.Update(request);
                return "UnSuccessful";
            }
        }
        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder stringBuilder = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }
        public async Task<int> GetPercentage(int id)
        {
            int percentage;
            var getinsurancetype = await _typeRepo.GetWithPredicate(x => x.Id == id);
            var getinsurancesubtype = await _subTypeRepo.GetWithPredicate(x => x.InsuranceTypeId == Convert.ToInt32(getinsurancetype.Id));

            //if (getinsurancesubtype.PercentageToBank == null)
            //{
            //    return percentage = Convert.ToInt32(getinsurancetype.PercentageToBank);
            //}
            //return percentage = Convert.ToInt32(getinsurancesubtype.PercentageToBank);
            return 0;
        }
        public async Task<decimal> GetPercentageAsync(Request request)
        {
            decimal percentage;

            if (request.InsuranceSubTypeID == null)
            {
                //return percentage = Convert.ToInt32(request.InsuranceType.PercentageToBank);
                return 0;
            }
            return percentage = Convert.ToDecimal(request.InsuranceSubType.PercentageToBank);

        }

        public BrokerStatus GetEnumValueByIndex(int index)
        {
            BrokerStatus[] values = (BrokerStatus[])Enum.GetValues(typeof(BrokerStatus));

            if (index >= 1 && index <= values.Length)
            {
                return values[index - 1];
            }
            else
            {
                throw new ArgumentException("Invalid index");
            }
        }
        public CommentStatus GetEnumValueByIndex1(int index)
        {
            CommentStatus[] values = (CommentStatus[])Enum.GetValues(typeof(CommentStatus));

            if (index >= 1 && index <= values.Length)
            {
                return values[index - 1];
            }
            else
            {
                
                throw new ArgumentException("Invalid index");
            }
        }

        public async Task<Underwriter> GetUnderwritersbybroker(int brokerid)
        {
            try
            {
                var getrequest = await _WriterRepo.GetWithPredicate(r => r.BrokerId == brokerid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex.ToString(), "GetUnderwritebyId");
                return null;
            }

        }
        public async Task<IEnumerable<InsuranceTable>> GetInsuranceRequestsByStageAsync(string stage)
        {
            return await _InsuranceTbRepo.GetAllWithPredicate(r => r.Stage == stage);
        }
        public async Task<IEnumerable<InsuranceTable>> GetInsuranceRequestsByStageAsync1(string stage)
        {
            return await _InsuranceTbRepo.GetAllWithPredicate(r => r.Stage == stage && r.CertificateRequestByUsername != _globalVariables.userName);
        }
        public async Task<IEnumerable<InsuranceTable>> GetInsuranceByRequester(string email)
        {
            return await _InsuranceTbRepo.GetAllWithPredicate(r => r.RequestByemail == email);
        }
        public async Task<IEnumerable<InsuranceTable>> GetAllInsuranceRequestsByStages(string stage1, string stage2)
        {
            return await _InsuranceTbRepo.GetAllWithPredicate(r => r.Stage == stage1 || r.Stage == stage2 && r.PolicyExpiryDate <= DateTime.Now && r.RequestDate <= r.PolicyExpiryDate);
        }

        public async Task<Request> GetRequestDetailsForInsuranceRequestsAsync(string requestId)
        {

            return await _reqRepo.GetWithIncludeAsync(
                      x => (x.RequestID == requestId && x.Status != CommentStatus.Closed.ToString()),
                      x => x.Broker,
                      x => x.InsuranceType.InsuranceType,
                      x => x.InsuranceSubType,
                      x => x.Underwriter
                  );
        }
        public string UpdateInsuranceReq(InsuranceTable request, string comment)
        {

            try
            {
                var update = _InsuranceTbRepo.Update(request);
                if (update == false) return "UnSuccessfully";
                string emailBody =
                   $"Request for Insurance Certificate- {request.FileName} has been Rejected by {_globalVariables.name}. See reason below:.<br><br>" +
                   "Details:<br>" +
                   "Request Type: Insurance Certificate<br>" +
                   $"Reason: {comment}<br>" +
                   "Thank you for using our services.<br>";

                string body = _utilityService.BuildEmailTemplate(request.CertificateRequestByName, "Certificate Upload", emailBody);

                _emailService.SmtpSendMail(request.CertificateRequestByemail, body, "Update On Certificate Upload", _appsettings.SourceEmail);

                return "Successfully";

            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex.ToString(), "UpdateCertificate");
                return "Error";
            }

        }

        public async Task<List<RecordReport>> FilterReport(string ContractId, string CustomerID, string CustomerName, string AccountNo, string InsuranceFlag, DateTime? PolicyExpiryStartDate, DateTime? PolicyExpiryEndDate, string Broker, string Underwriter)
        {
            try
            {
                var requestList = await _reqRepo.GetWithInclude(
                   i => ((string.IsNullOrEmpty(ContractId) || i.ContractID.Contains(ContractId)) &&
                    (string.IsNullOrEmpty(CustomerID) || i.CustomerID.Contains(CustomerID)) &&
                    (string.IsNullOrEmpty(CustomerName) || i.CustomerName.Contains(CustomerName)) &&
                    (string.IsNullOrEmpty(AccountNo) || i.AccountNo.Contains(AccountNo)) &&
                    //(string.IsNullOrEmpty(InsuranceFlag) || i.InsuranceFlag == InsuranceFlag) &&
                    //(!PolicyExpiryStartDate.HasValue || i.PolicyExpiryDate >= PolicyExpiryStartDate) &&
                    //(!PolicyExpiryEndDate.HasValue || i.PolicyExpiryDate <= PolicyExpiryEndDate) &&
                    (string.IsNullOrEmpty(Broker) || i.Broker.BrokerName == Broker) &&
                    (string.IsNullOrEmpty(Underwriter) || i.Underwriter.Name == Underwriter)),
                i => i.Broker,
                i => i.InsuranceType,
                i => i.Underwriter,
                i => i.InsuranceSubType


                    );

                List<Request> request = requestList.ToList();

                List<RecordReport> records = new List<RecordReport>();
                // foreach (var item in request)
                for (int item = 0; item < request.Count; item++)
                {
                    var insurance = await _InsuranceTbRepo.GetWithIncludeAsync(x => (x.RequestID == request[item].RequestID)
                    , x => x.fundTransferLookUps);
                    string premiumUniqueID = "";
                    string commissionUniqueID = "";
                    decimal commissionAmount = 0;

                    if (insurance != null)
                    {
                        foreach (var i in insurance.fundTransferLookUps)
                        {

                            if (i.TransactionType == "Commission")
                            {
                                commissionUniqueID = i.UniqueID;
                                commissionAmount = _utilityService.GetDebitValue(i.TransactionRequest);
                            }
                            else if (i.TransactionType == "InsuranceRequest")
                            {
                                premiumUniqueID = i.UniqueID;
                            }

                        };
                    }
                    TimeSpan difference = Convert.ToDateTime(insurance.PolicyExpiryDate) - Convert.ToDateTime(insurance.PolicyIssuanceDate);

                    var record = new RecordReport
                    {
                        RecordId = Convert.ToInt64(request[item].RequestID),
                        ContractId = request[item].ContractID,
                        AccountNumber = request[item].AccountNo,
                        AccountName = request[item].AccountName,
                        InsuranceType = request[item].InsuranceType.InsuranceType.Name,
                        DebitPassed = insurance.Serial,
                        ContractMaturityDate = request[item].ContractMaturityDate,
                        CustomerName = request[item].CustomerName,
                        CollateralValue = request[item].CollateralValue,
                        CustomerEmail = request[item].CustomerEmail,
                        SubInsuranceType = request[item].InsuranceSubType?.Name,
                        EstimatedPremium = request[item].UpdatedPremium,
                        Underwriter = request[item].Underwriter?.Name,
                        Broker = request[item].Broker.BrokerName,
                        CustomerID = request[item].CustomerID,
                        InsuranceFlag = insurance.Stage,
                        Certificate = $"<a href='/Insurance/DownloadCertificate/?RequestId={Convert.ToInt64(request[item].RequestID)}' class='btn btn-raised btn-primary waves-effect btn-round'>{insurance.FileName}</a>",
                        PolicyNo = insurance.PolicyNo,
                        CommissionAmount = commissionAmount,
                        DateofIssuance = insurance.PolicyIssuanceDate,
                        PolicyExpiryDate = insurance.PolicyExpiryDate,
                        CommissionUniqueID = commissionUniqueID,
                        PremiumAmount = request[item].UpdatedPremium,
                        PremiumCRAccount = request[item].Broker.AccountNumber,
                        PremiumDRAccount = request[item].AccountNo,
                        PremiumUniqueID = premiumUniqueID,
                        Stage = insurance.Stage,
                        Branch = request[item].Branchcode,
                        InsuranceStatus = insurance.RequestType,
                        PolicyDuration = $"{difference.Days} Days",
                        Id = item

                    };
                    var test = JsonConvert.SerializeObject(record);

                    records.Add(record);
                };
                return records;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "FilterReport");
                return null;
            }

        }

        public async Task<List<RecordReport>> ReportReport()
        {
            try
            {
                var requestList = await _reqRepo.GetWithInclude(
                   i => (i.RequestID != null && i.Status != CommentStatus.Closed.ToString()),
                i => i.Broker,
                i => i.InsuranceType,
                i => i.Underwriter,
                i => i.InsuranceSubType


                    );

                List<Request> request = requestList.ToList();
                List<RecordReport> records = new List<RecordReport>();
                //foreach (var item in request)
                for (int item = 0; item < request.Count; item++)
                {
                    var insurance = await _InsuranceTbRepo.GetWithIncludeAsync(x => (x.RequestID == request[item].RequestID)
                    , x => x.fundTransferLookUps);
                    string premiumUniqueID = "";
                    string commissionUniqueID = "";
                    decimal commissionAmount = 0;

                    if (insurance != null)
                    {
                        foreach (var i in insurance.fundTransferLookUps)
                        {

                            if (i.TransactionType == "Commission")
                            {
                                commissionUniqueID = i.UniqueID;
                                commissionAmount = _utilityService.GetDebitValue(i.TransactionRequest);
                            }
                            else if (i.TransactionType == "InsuranceRequest")
                            {
                                premiumUniqueID = i.UniqueID;
                            }

                        };
                    }
                    TimeSpan difference = Convert.ToDateTime(insurance.PolicyExpiryDate) - Convert.ToDateTime(insurance.PolicyIssuanceDate);

                    var record = new RecordReport
                    {
                        RecordId = Convert.ToInt64(request[item].RequestID),
                        ContractId = request[item].ContractID,
                        AccountNumber = request[item].AccountNo,
                        AccountName = request[item].AccountName,
                        InsuranceType = request[item].InsuranceType.InsuranceType.Name,
                        DebitPassed = insurance.Serial,
                        ContractMaturityDate = request[item].ContractMaturityDate,
                        CustomerName = request[item].CustomerName,
                        CollateralValue = request[item].CollateralValue,
                        CustomerEmail = request[item].CustomerEmail,
                        SubInsuranceType = request[item].InsuranceSubType?.Name,
                        EstimatedPremium = request[item].UpdatedPremium,
                        Underwriter = request[item].Underwriter?.Name,
                        Broker = request[item].Broker.BrokerName,
                        CustomerID = request[item].CustomerID,
                        InsuranceFlag = insurance.Stage,
                        Certificate = $"<a href='/Insurance/DownloadCertificate/?RequestId={Convert.ToInt64(request[item].RequestID)}' class='btn btn-raised btn-primary waves-effect btn-round'>{insurance.FileName}</a>",
                        PolicyNo = insurance.PolicyNo,
                        CommissionAmount = commissionAmount,
                        DateofIssuance = insurance.PolicyIssuanceDate,
                        PolicyExpiryDate = insurance.PolicyExpiryDate,
                        CommissionUniqueID = commissionUniqueID,
                        PremiumAmount = request[item].UpdatedPremium,
                        PremiumCRAccount = request[item].Broker.AccountNumber,
                        PremiumDRAccount = request[item].AccountNo,
                        PremiumUniqueID = premiumUniqueID,
                        Stage = insurance.Stage,
                        Branch = request[item].Branchcode,
                        InsuranceStatus = insurance.RequestType,
                        PolicyDuration = $"{difference.Days} Days",
                        Id = item

                    };
                    var test = JsonConvert.SerializeObject(record);

                    records.Add(record);
                };
                return records;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "ReportReport");
                return null;
            }

        }
        private async Task<List<RecordReport>> ProcessInsuranceReport1(IQueryable<Request> query, int start, int limit, string filter = "")
        {
            try
            {
                var request = query.OrderByDescending(d => d.ID).Skip(start).Take(limit).ToList();
                List<RecordReport> records = new List<RecordReport>();
                for (int item = 0; item < request.Count; item++)

                // foreach (var item in request)
                {
                    var insurance = await _InsuranceTbRepo.GetWithIncludeAsync(x => (x.RequestID == request[item].RequestID)
                    , x => x.fundTransferLookUps);
                    string premiumUniqueID = "";
                    string commissionUniqueID = "";
                    decimal commissionAmount = 0;

                    if (insurance != null)
                    {
                        foreach (var i in insurance.fundTransferLookUps)
                        {

                            if (i.TransactionType == "Commission")
                            {
                                commissionUniqueID = i.UniqueID;
                                commissionAmount = _utilityService.GetDebitValue(i.TransactionRequest);
                            }
                            else if (i.TransactionType == "InsuranceRequest")
                            {
                                premiumUniqueID = i.UniqueID;
                            }

                        };
                    }
                    TimeSpan difference = Convert.ToDateTime(insurance.PolicyExpiryDate) - Convert.ToDateTime(insurance.PolicyIssuanceDate);

                    if (!string.IsNullOrEmpty(filter))
                    {




                        if (!request[item].ContractID.Contains(filter, StringComparison.OrdinalIgnoreCase)) { }
                        if (!request[item].CustomerID.Contains(filter, StringComparison.OrdinalIgnoreCase)) { }
                        if (!request[item].AccountNo.Contains(filter, StringComparison.OrdinalIgnoreCase)) { }

                        if (!insurance.Stage.Contains(filter, StringComparison.OrdinalIgnoreCase)) { }
                        //if (!insurance.PolicyExpiryDate.HasValue(filter, StringComparison.OrdinalIgnoreCase)) { }
                        // if (!insurance.PolicyIssuanceDate.Contains(filter, StringComparison.OrdinalIgnoreCase)) { }


                        if (!request[item].CustomerName.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        {
                            var record = new RecordReport
                            {
                                RecordId = Convert.ToInt64(request[item].RequestID),
                                ContractId = request[item].ContractID,
                                AccountNumber = request[item].AccountNo,
                                AccountName = request[item].AccountName,
                                InsuranceType = request[item].InsuranceType.InsuranceType.Name,
                                DebitPassed = insurance.Serial,
                                ContractMaturityDate = request[item].ContractMaturityDate,
                                CustomerName = request[item].CustomerName,
                                CollateralValue = request[item].CollateralValue,
                                CustomerEmail = request[item].CustomerEmail,
                                SubInsuranceType = request[item].InsuranceSubType?.Name,
                                EstimatedPremium = request[item].UpdatedPremium,
                                Underwriter = request[item].Underwriter?.Name,
                                Broker = request[item].Broker.BrokerName,
                                CustomerID = request[item].CustomerID,
                                InsuranceFlag = insurance.Stage,
                                Certificate = $"<a href='/Insurance/DownloadCertificate/?RequestId={Convert.ToInt64(request[item].RequestID)}' class='btn btn-raised btn-primary waves-effect btn-round'>{insurance.FileName}</a>",
                                //Certificate = insurance.FileName,
                                PolicyNo = insurance.PolicyNo,
                                CommissionAmount = commissionAmount,
                                DateofIssuance = insurance.PolicyIssuanceDate,
                                PolicyExpiryDate = insurance.PolicyExpiryDate,
                                CommissionUniqueID = commissionUniqueID,
                                PremiumAmount = request[item].UpdatedPremium,
                                PremiumCRAccount = request[item].Broker.AccountNumber,
                                PremiumDRAccount = request[item].AccountNo,
                                PremiumUniqueID = premiumUniqueID,
                                Stage = insurance.Stage,
                                Branch = request[item].Branchcode,
                                InsuranceStatus = insurance.RequestType,
                                PolicyDuration = $"{difference.Days} Days",
                                Id = item

                            };
                            var test = JsonConvert.SerializeObject(record);

                            records.Add(record);
                        }
                        continue;
                        //return records;

                    }

                    else
                    {
                        var record = new RecordReport
                        {
                            RecordId = Convert.ToInt64(request[item].RequestID),
                            ContractId = request[item].ContractID,
                            AccountNumber = request[item].AccountNo,
                            AccountName = request[item].AccountName,
                            InsuranceType = request[item].InsuranceType.InsuranceType.Name,
                            DebitPassed = insurance.Serial,
                            ContractMaturityDate = request[item].ContractMaturityDate,
                            CustomerName = request[item].CustomerName,
                            CollateralValue = request[item].CollateralValue,
                            CustomerEmail = request[item].CustomerEmail,
                            SubInsuranceType = request[item].InsuranceSubType?.Name,
                            EstimatedPremium = request[item].UpdatedPremium,
                            Underwriter = request[item].Underwriter?.Name,
                            Broker = request[item].Broker.BrokerName,
                            CustomerID = request[item].CustomerID,
                            InsuranceFlag = insurance.Stage,
                            Certificate = $"<a href='/Insurance/DownloadCertificate/?RequestId={Convert.ToInt64(request[item].RequestID)}' class='btn btn-raised btn-primary waves-effect btn-round'>{insurance.FileName}</a>",
                            //Certificate = insurance.FileName,
                            PolicyNo = insurance.PolicyNo,
                            CommissionAmount = commissionAmount,
                            DateofIssuance = insurance.PolicyIssuanceDate,
                            PolicyExpiryDate = insurance.PolicyExpiryDate,
                            CommissionUniqueID = commissionUniqueID,
                            PremiumAmount = request[item].UpdatedPremium,
                            PremiumCRAccount = request[item].Broker.AccountNumber,
                            PremiumDRAccount = request[item].AccountNo,
                            PremiumUniqueID = premiumUniqueID,
                            Stage = insurance.Stage,
                            Branch = request[item].Branchcode,
                            InsuranceStatus = insurance.RequestType,
                            PolicyDuration = $"{difference.Days} Days",
                            Id = item

                        };
                        var test = JsonConvert.SerializeObject(record);

                        records.Add(record);
                    }
                };
                return records;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "ProcessInsuranceReport");
                return null;
            }

        }

        public async Task<string> AssignUnderwriter(InsuranceTable Insurance, Request request)
        {
            try
            {
                if (Insurance.RequestType == "Renewal")
                {
                    Insurance.Stage = InsuranceStage.CertificateUploaded.ToString();

                    _InsuranceTbRepo.Update(Insurance);
                    return "Successfully";
                }

                Random random = new Random();
                var insuranceTypeId = await GetPercentageAsync(request);
                var estimatedPremium = request.UpdatedPremium;

                // var commission = Convert.ToDecimal((estimatedPremium * insuranceTypeId) / 100);
                var commission = (Convert.ToDecimal((estimatedPremium * insuranceTypeId) / 100)) / Convert.ToDecimal(_appsettings.VAT);

                var formattedCommission = Decimal.Round(commission, 2, MidpointRounding.AwayFromZero);
                //string formattedDateTime = DateTime.Now.ToString("yy/MM/dd HHmmss");
                string Serial = FormatSerial(Insurance.Serial);
                var transfer = new FundsTransferRequestDto()
                {
                    TransactionType = TransactionType.InsuranceRequest.ToString(),
                    Amount = Convert.ToDecimal(request.UpdatedPremium),
                    DebitAccount = request.AccountNo,
                    Channel = "IM",
                    Narration = $"PREMIUM FOR INSURANCE ISSUANCE FOR   COLLATERAL {request.CollateralValue} ",
                    CreditAccount = request.Broker.AccountNumber,
                    BranchCode = request.Branchcode,
                    CreditCurrency = "NGN",
                    DebitCurrency = "NGN",
                    TransactionCode = "IM",
                    TransactionId = $"{request.UpdatedPremium}-{request.ID}-{GenerateRandomString(3)}",
                    Id = Convert.ToInt64(Insurance.Serial),
                    TransactionRef = $"F{request.RequestID}",
                    UnquieId = $"FEES-{request.RequestID}-{Serial}",
                    RequestID = request.RequestID,
                    InsuranceTableId = Insurance.ID


                };
                Insurance.FEESFTReference = transfer.UnquieId;
                var credential = new Credential
                {
                    T24password = _appsettings.T24password,
                    T24Username = _appsettings.T24Username
                };

                var fundtrans = await _t24.FundTransfer(transfer, credential);
                _logger.LogInformation(fundtrans.ToString(), $"FundTransfer to Broker Account ={request.Broker.AccountNumber}");
                var requery = await _oracleDataService.RequeryFTUniqueId(transfer.UnquieId);
                _InsuranceTbRepo.Update(Insurance);
                if (requery == null) return "UnSuccessful";
                if (fundtrans.Status == true)
                {

                    var paycommission = new FundsTransferRequestDto()
                    {
                        TransactionType = TransactionType.Commission.ToString(),
                        Amount = formattedCommission,
                        DebitAccount = request.Broker.AccountNumber,
                        Channel = "IM",
                        Narration = $"COMMISSION ON ISSURANCE ISSUANCE   {request.RequestID} {Insurance.Serial} {request.CustomerName}",/*.Replace("-", "/"),*/
                        CreditAccount = _appsettings.GLAccountNumber,
                        BranchCode = null,
                        CreditCurrency = "NGN",
                        DebitCurrency = "NGN",
                        TransactionCode = "IM",
                        TransactionId = $"{formattedCommission}-{request.ID}-{GenerateRandomString(3)}",
                        Id = Convert.ToInt64(Insurance.Serial),
                        TransactionRef = $"C{request.RequestID}",
                        UnquieId = $"COMM-{request.RequestID}-{Serial}",
                        RequestID = request.RequestID,
                        InsuranceTableId = Insurance.ID

                    };
                    Insurance.COMMFTReference = paycommission.UnquieId;
                    var fundtranstoglaccount = await _t24.FundTransfer(paycommission, credential);
                    _logger.LogInformation(fundtrans.ToString(), $"FundTransfer to GL account ={_appsettings.GLAccountNumber}");
                    var requeryComm = await _oracleDataService.RequeryFTUniqueId(paycommission.UnquieId);



                    _InsuranceTbRepo.Update(Insurance);
                    if (requeryComm == null)
                    {
                        var lockfund = await _t24.LockFunds(paycommission.DebitAccount, "Lock " + paycommission.UnquieId, paycommission.Amount, credential);

                        return "UnSuccessful";
                    }

                    if (fundtranstoglaccount.Status == false)
                    {
                        var lockfund = await _t24.LockFunds(paycommission.DebitAccount, "Lock " + paycommission.UnquieId, paycommission.Amount, credential);

                        Insurance.Stage = InsuranceStage.UnderwriterAssigned.ToString();
                        Insurance.ErrorMessage = fundtranstoglaccount.ResponseMessage;
                        request.Status = TransferStatus.Failed.ToString();
                        _InsuranceTbRepo.Update(Insurance);
                        
                        string sanitizedResponseMessage = fundtranstoglaccount.ResponseMessage
                            .Replace("\r", "")   
                            .Replace("\n", "<br>");

                        string mailTemplate =
                            "We are writing to inform you that your transaction has failed due to the following reason:<br><br>" +
                            "Reason:<br>" + $"{sanitizedResponseMessage}<br><br>";
                           

                        string body = _utilityService.BuildEmailTemplate(_globalVariables.name, "Commission on Insurance", mailTemplate);


                        _emailService.SmtpSendMail(_globalVariables.Email, body, "Commission on Insurance");

                        //_emailService.SmtpSendMail(request.CustomerEmail, mailTemplate /*$"transaction failed because{fundtrans.ResponseMessage}"*/, "Commission on Insurance");
                    }
                    Insurance.Stage = InsuranceStage.CertificateUploaded.ToString();
                    var updateRequest = _InsuranceTbRepo.Update(Insurance);
                    string emailSubjectRequest = $"Payment Request for  Insurance was successful for {request.CustomerName}";

                    string emailBodyRequest =
                       $"We are pleased to inform you that the request for Payment on Insurance for {request.CustomerName} has been successfully processed.<br><br>" +
                       "Details:<br>" +
                       "Transaction Type: PAYMENT<br>" +
                       $"Customer Name: {request.CustomerName}<br>" +
                       $"Amount: #{transfer.Amount}<br>" +
                       $"Unique Number: {transfer.UnquieId}<br><br>";
                      
                    string bodytemp = _utilityService.BuildEmailTemplate(request.Broker.AccountName, "Payment Request for  Insurance was successful", emailBodyRequest);


                    _emailService.SmtpSendMail(request.Broker.EmailAddress, bodytemp, emailSubjectRequest);
                    // _emailService.SmtpSendMail(broker.EmailAddress, $"Payment Request for  Insurance was successful. see detail{request.CustomerName}", "Payment on Insurance");

                    return "Successfully";
                }

                if (fundtrans.Status == false)
                {
                    Insurance.Stage = InsuranceStage.UnderwriterAssigned.ToString();
                    Insurance.ErrorMessage = fundtrans.ResponseMessage;
                    request.Status = TransferStatus.Failed.ToString();
                    var updateRequest = _InsuranceTbRepo.Update(Insurance);
                    
                    string sanitizedResponseMessage = fundtrans.ResponseMessage
                        .Replace("\r", "")  
                        .Replace("\n", "<br>");

                    string mailTemplate =
                        $"We are writing to inform you that your transaction has failed due to the following reason:<br><br>" +
                        "Reason:<br>" + $"{sanitizedResponseMessage}<br><br>";
                       
                    string body1 = _utilityService.BuildEmailTemplate(_globalVariables.name, "Debit Notification For Insurance  Process", mailTemplate);

                    _emailService.SmtpSendMail(_globalVariables.Email, body1, "Debit Notification For Insurance  Process");

                    // _emailService.SmtpSendMail(request.CustomerEmail, mailTemplate", "Debit Notification For Insurance  Process");
                }
                return "Unsucessful";
            }
            catch (Exception ex)
            {
                _logger.LogFatal(ex.ToString(), "AuthorizeRequest");
                Insurance.ErrorMessage = ex.InnerException.ToString();
                var updateRepuest = _InsuranceTbRepo.Update(Insurance);
                return "UnSuccessful";
            }
        }
        public async Task<int> CountAll()
        {
            try
            {
                return _InsuranceTbRepo.GetAllWithPredicate(x => x.Stage != null).Result.Count();

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.ToString());
            }
            return 0;
        }
        public async Task<DataTablesResponse> FetchInsurancesForDataTableAsync(DataTablesRequest request)
        {
            try
            {
                var filteredPurchases = new List<RecordReport>();
                var totalCount = await CountAll();
                var start = request.Start;
                var limit = request.Length;
                var sortedColumnList = new List<string>();
                string filterString = "";
                //var data = new List<dynamic>();
                foreach (var order in request.Order)
                {
                    string _sortedColumnString = $"{request.Columns[order.Column].Data} {order.Dir}";
                    sortedColumnList.Add(_sortedColumnString);
                }
                if (sortedColumnList.Count > 0)
                {
                    filterString = string.Join(",", sortedColumnList);

                }

                if (!string.IsNullOrEmpty(request.Search.value))
                {
                    string searchValue = request.Search.value.Trim();
                    filteredPurchases = await SearchWithParameters(searchValue, start, limit, filterString);
                }
                else
                {
                    filteredPurchases = await ListAllWithParameters(start, limit, filterString);
                }

                var filteredCount = totalCount;



                var response = new DataTablesResponse(request.Draw, filteredPurchases, filteredCount, totalCount);
                return response;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.ToString(), "FetchInsurancesForDataTableAsync");
                throw;
            }
        }
        public  string GetPermissionName(string permissionConstant)
        {
            Type permissionsType = typeof(Permissions);
            foreach (var field in permissionsType.GetFields())
            {
                if ((string)field.GetValue(null) == permissionConstant)
                {
                    return field.Name;
                }
            }
            return "Abbreviation not found";
        }

        public async Task<string> CreateRequest(Request request)
        {
            try
            {

                //request.Branchcode = _globalVariables.branchCode;
                string dateTimeString = DateTime.Now.ToString("yyyy/MM/ddHHmmss");
                string formattedDateTime = dateTimeString.Replace("/", "");
                request.RequestID = formattedDateTime;

                var detail = JsonConvert.SerializeObject(request);
                _reqRepo.Insert(request);

                var insurance = new InsuranceTable()
                {
                    Serial = 1,
                    Stage = InsuranceStage.New.ToString(),
                    RequestID = formattedDateTime,
                    RequestDate = DateTime.Now,
                    RequestByName = _globalVariables.name,
                    RequestByemail = _globalVariables.Email,
                    RequestByUsername = _globalVariables.userName,
                    ToBeAuthroiziedBy = _globalVariables.branchCode,
                    RequestType = "New"

                };


                var insert = _InsuranceTbRepo.Insert(insurance);
                string authEmail = "";
                string authname = "";
                if (insert == true)
                {

                    var environment = _appsettings.Envirnoment;
                    if (environment.ToLower() != "Test")
                    {
                        var authResp = await _aum.GetUserInFeature(_globalVariables.branchCode, "RIC");
                        authEmail = string.Join(",", authResp.Select(o => o.email));
                        authname = string.Join(",", authResp.Select(o => o.name));
                    }
                    else
                    {
                        authname = "Team";
                        authEmail = _appsettings.TestTO;
                    }

                    string emailBodyRequest =
                          $"The Insurance request with details below has been assigned to you to authorize .<br><br>" +
                          $"Account Name : {request.AccountName} <br>" +
                          $"Account Number {request.AccountNo}<br>" +
                          $"Premium: {request.UpdatedPremium}<br>" +
                           $"Initiator: {_globalVariables.name}<br>" +
                          "Thank you for using our services.<br>";

                    string body = _utilityService.BuildEmailTemplate(authname, " Authorize New Insurance Request", emailBodyRequest);

                    _emailService.SmtpSendMail(authEmail, body, "Authorize New Insurance Request");
                    return "Successfully";
                }
                return "UnSuccessful";

                // request.Status = RequestStatus.New;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "CreateRequest");
                return "Error";
            }

        }
        public async Task<string> UploadCertificate(CertificateRequest certificateRequest)
        {
            var request = await _InsuranceTbRepo.GetWithPredicate(x => x.RequestID == certificateRequest.insuranceId.ToString());
            if (request == null) return "Not Found";
            try
            {
                var getfullrequest = await _reqRepo.GetWithPredicate(x => x.RequestID == certificateRequest.insuranceId.ToString());
                var certificate = _utilityService.ConvertIFormFileToBase64(certificateRequest.Certificatefile);
                // var uploadtopath = await _utilityService.GenerateFilePath(certificateRequest.Certificatefile);
                // Create certificate
                //var certificate = new InsuranceCertificate
                //{
                //    CertificateFile = uploadtopath,
                //    IssuanceDate = certificateRequest.DateofIssuance,
                //    ExpiryDate = certificateRequest.ExpiryDate,
                //    InsuranceRequestId = certificateRequest.insuranceId,
                //    UserId = _globalVariables.userid,
                //    DateCreated = DateTime.Now
                //};

                //// Save to DB
                //certificate.UserId = _globalVariables.userid;
                //var addcertificate = _CertificateRepo.Insert(certificate);
                //var getcertificate = await _CertificateRepo.GetWithPredicate(x => x.InsuranceRequestId == certificateRequest.insuranceId);
                //// Associate with request
                //request.InsuranceCertificateId = getcertificate.CertificateID;

                // Update status
                request.PolicyCertificate = certificate;
                request.Stage = InsuranceStage.ReviewCertificate.ToString();
                request.CertificateRequestDate = DateTime.Now;
                request.CertificateRequestByName = _globalVariables.name;
                request.CertificateRequestByemail = _globalVariables.Email;
                request.CertificateRequestByUsername = _globalVariables.userName;
                request.CertificateToBeAuthroiziedBy = _globalVariables.branchCode;
                request.ContentType = certificateRequest.Certificatefile.ContentType;
                request.FileName = certificateRequest.Certificatefile.FileName;
                // Update dates
                request.PolicyNo = certificateRequest.PolicyNumber;
                request.PolicyExpiryDate = certificateRequest.ExpiryDate;
                request.PolicyIssuanceDate = certificateRequest.DateofIssuance;

                // Save request
                string authEmail = "";
                string authname = "";
                var update = _InsuranceTbRepo.Update(request);
                if (update == true)
                {
                    var environment = _appsettings.Envirnoment;
                    if (environment.ToLower() != "Test")
                    {
                        var authResp = await _aum.GetUserInFeature(_globalVariables.branchCode, "RIC");
                        authEmail = string.Join(",", authResp.Select(o => o.email));
                        authname = string.Join(",", authResp.Select(o => o.name));
                    }
                    else
                    {
                        authname = "Team";
                        authEmail = _appsettings.TestTO;
                    }


                    string emailBodyRequest =
                          $"An insurance policy authorization request has been assigned to you in respect of  {getfullrequest.AccountName} with Policy Nos:{certificateRequest.PolicyNumber} and Endorsement Nos: {certificateRequest.insuranceId} from Keystone Bank. " +
                          $"Kindly log on to the Insurance Policy Portal platform URL:{_appsettings.BasePortalURL}, review the request and process in line with the attached template.< br><br>" +

                          "Thank you for using our services.<br>";

                    string body = _utilityService.BuildEmailTemplate(authname, " Authorize New Insurance Request", emailBodyRequest);

                    _emailService.SmtpSendMail(authEmail, body, "Authorize New Insurance Request");
                    return "Successfully";
                }
                return "UnSuccessful";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "UploadCertificate");
                request.ErrorMessage = ex.InnerException.ToString();
                var updateRepuest = _InsuranceTbRepo.Update(request);
                return "UnSuccessFul";
            }

        }
    }
}
    

