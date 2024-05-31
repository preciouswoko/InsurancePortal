using InsuranceCore.DTO;
using InsuranceCore.Enums;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static InsuranceCore.DTO.ReusableVariables;
using Microsoft.AspNetCore.Http;

namespace InsuranceCore.Implementations
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IHttpClientService _clientService;
        private readonly ILoggingService _logging;
        private readonly IGenericRepository<Request> _reqRepo;
        private readonly IGenericRepository<Comments> _commRepo;
        private readonly IGenericRepository<Broker> _BrokerRepo;
        private readonly IGenericRepository<InsuranceTable> _InsuranceTbRepo;
        private readonly IGenericRepository<Underwriter> _WriterRepo;
        private readonly IEmailService _emailService;
        public readonly IGenericRepository<Underwriter> _underwriteRepo;
        public readonly IGenericRepository<InsuranceType> _insuranceTypeRepo;
        public readonly IGenericRepository<InsuranceSubType> _insuranceSubTypeRepo;
        public readonly IGenericRepository<BrokerInsuranceType> _brokerinsuranceTypeRepo;
        public readonly IGenericRepository<BrokerSubInsuranceType> _brokerinsuranceSubTypeRepo;
        public readonly IUtilityService _utilityService;
        private readonly ISessionService _service;
       // private readonly GlobalVariables _globalVariables;
        private readonly IOracleDataService _oracleDataService;
        private readonly IT24Service _t24;
        public readonly IGenericRepository<FundTransferLookUp> _fundRepo;
        public readonly IAumsService _aum;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string generalVariable = null;

        public InsuranceService(IGenericRepository<Comments> commRepo, IGenericRepository<Request> reqRepo,
            IGenericRepository<InsuranceTable> InsuranceTbRepo,
            IHttpClientService clientService, ILoggingService logging,
            IGenericRepository<Broker> BrokerRepo,
            IGenericRepository<Underwriter> WriterRepo,
            IEmailService emailService, IAumsService aum,
            IGenericRepository<Underwriter> underwriteRepo,
            IGenericRepository<InsuranceType> insuranceTypeRepo,
            IGenericRepository<InsuranceSubType> insuranceSubTypeRepo,
            IUtilityService utilityService, ISessionService service,
            IConfiguration configuration, IOracleDataService oracleDataService,
            IT24Service t24, IGenericRepository<FundTransferLookUp> fundRepo,
            IGenericRepository<BrokerInsuranceType> brokerinsuranceTypeRepo,
            IGenericRepository<BrokerSubInsuranceType> brokerinsuranceSubTypeRepo, IHttpContextAccessor httpContextAccessor
            )
        {
            _aum = aum;
            _reqRepo = reqRepo;
            _commRepo = commRepo;
            _logging = logging;
            _clientService = clientService;
            _BrokerRepo = BrokerRepo;
            _WriterRepo = WriterRepo;
            _emailService = emailService;
            _underwriteRepo = underwriteRepo;
            _insuranceTypeRepo = insuranceTypeRepo;
            _insuranceSubTypeRepo = insuranceSubTypeRepo;
            _utilityService = utilityService;
            _service = service;
            //generalVariable = _httpContextAccessor.HttpContext.Session.GetString("GlobalVariables");
            //if (generalVariable != null)
            //{
            //    _globalVariables = JsonConvert.DeserializeObject<GlobalVariables>(generalVariable) ?? new GlobalVariables();
            //}
            //else
            //{
            //    // Handle the case where the JSON string is null
            //    _globalVariables = new GlobalVariables();
            //}            //_globalVariables = _service.Get<GlobalVariables>("GlobalVariables");
            _oracleDataService = oracleDataService;
            _t24 = t24;
            _InsuranceTbRepo = InsuranceTbRepo;
            _fundRepo = fundRepo;
            _brokerinsuranceSubTypeRepo = brokerinsuranceSubTypeRepo;
            _brokerinsuranceTypeRepo = brokerinsuranceTypeRepo;

        }

        public async Task<AccountDetail> FetchDetail(string Nubam)
        {
            try
            {
                var cancellationTokenSource = new CancellationToken();
                CancellationToken cancellationToken = cancellationTokenSource;
                // _emailService.SmtpSendMail(_globalVariables.Email, $"Request has been assigned to you", "New insurance request assigned", "wokoalex79@gmail.com");
                //var accountEnquiry =  await _t24.AccountEnquiry(Nubam, cancellationTokenSource);
                // var nameEnquiry = await _t24.NameEnquiry(Nubam, cancellationTokenSource);

                var dataTable = await _oracleDataService.ExecuteQuery(Nubam);
                if (dataTable == null) return null;

                var accountdetail = new AccountDetail
                {
                    AccountName = dataTable.AccountName,
                    CustomerEmail = dataTable.CustomerEmail,
                    T24CustomerID = dataTable.CustomerID,
                    CustomerName = dataTable.AccountName,
                    Branchcode = dataTable.BranchCode
                };
                return accountdetail;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "FetchDetail");
                return null;
            }
        }


        //private bool UserHasPermission(string route, string[] permissions)
        //{
        //    if (String.IsNullOrEmpty(route))
        //        return false;

        //    // Check for the custom attribute applied to the action
        //    var requiredPermissionAttribute = GetRequiredPermissionAttribute(route);
        //    if (requiredPermissionAttribute != null)
        //    {
        //        var requiredPermission = requiredPermissionAttribute.PermissionName;
        //        if (permissions.Contains(requiredPermission))
        //            return true;
        //    }

        //    // Check for generic permissions
        //    if (permissions.Contains(route))
        //        return true;

        //    // Check for default permissions
        //    if (permissions.Contains(Permissions.AUR) || permissions.Contains(Permissions.INR))
        //        return true;

        //    return false;
        //}

        
        public async Task<Broker> GetBroker(int id)
        {
            return await _BrokerRepo.GetWithPredicate(x => x.Id == id);
        }
        public bool DeleteBroker(int id)
        {
            return _BrokerRepo.Delete(id);
        }
        public bool DeleteUnderwriter(int id)
        {
            return _underwriteRepo.Delete(id);
        }
        public bool DeleteInsurance(int id)
        {
            return _insuranceTypeRepo.Delete(id);
        }
        public bool DeleteInsuranceSub(int id)
        {
            return _insuranceSubTypeRepo.Delete(id);
        }
        public async Task<BrokerInsuranceType> GetBrokerInsuranceType(int id)
        {
            return await _brokerinsuranceTypeRepo.GetWithPredicate(x => x.Id == id);
        }
        public async Task<InsuranceType> GetInsuranceType(int id)
        {
            return await _insuranceTypeRepo.GetWithPredicate(x => x.Id == id);
        }
        public async Task<Underwriter> GetUnderwrite(int id)
        {
            return await _underwriteRepo.GetWithPredicate(x => x.Id == id);
        }
        public async Task<Request> Getrequest(string requestid)
        {

            //return await _reqRepo.GetWithIncludeAsync(
            //    i => (i.RequestID == requestid),
            //    i => i.Broker
            //    );
            return await _reqRepo.GetWithIncludeAsync(
                  x => (x.RequestID == requestid && x.Status != CommentStatus.Closed.ToString()),
                  x => x.Broker,
                  x => x.InsuranceType,
                  x => x.InsuranceSubType
                  );

            // return await _reqRepo.GetWithPredicate(x => x.RequestID == requestid.ToString());
        }
        public async Task<InsuranceTable> GetInsurancereq(string requestid)
        {
            return await _InsuranceTbRepo.GetWithPredicate(x => x.RequestID == requestid);
        }
        public async Task<InsuranceSubType> GetInsuranceSubType(int id)
        {
            return await _insuranceSubTypeRepo.GetWithPredicate(x => x.Id == id);
        }
        public async Task<IEnumerable<Broker>> GetAllBroker()
        {
            return await _BrokerRepo.GetAllWithPredicate(x => x.Status != BrokerStatus.Disable.ToString() && x.Status != BrokerStatus.Inactive.ToString());
        }
        public async Task<List<Broker>> GetAllBroker(int insuranceTypeId, int insuranceSubTypeID)
        {
            List<Broker> brokers = new List<Broker>();

            var getinsuranceSubTypetrequest = await _insuranceSubTypeRepo.GetWithPredicate(r => r.Id == insuranceSubTypeID && r.InsuranceTypeId == insuranceTypeId);
            if(getinsuranceSubTypetrequest != null)
            {
                var getbrokers = await _brokerinsuranceTypeRepo.GetWithInclude(
                i => (i.Status != BrokerStatus.Disable.ToString() && i.Status != BrokerStatus.Inactive.ToString() && i.InsuranceTypeId == getinsuranceSubTypetrequest.InsuranceTypeId),
                
                i => i.Broker
                );
                if (getbrokers.Any())
                {
                    foreach(var item in getbrokers)
                    {
                        brokers.Add(item.Broker);
                    }
                }
            }



            return brokers;
        }
        public async Task<IEnumerable<InsuranceType>> GetAllInsuranceType()
        {
            var insuranceTypes = await _insuranceTypeRepo.GetWithInclude(
                i => (i.Status != BrokerStatus.Disable.ToString() && i.Status != BrokerStatus.Inactive.ToString()),
                //i => i.Broker,
                i => i.SubInsuranceTypes
                );
            return insuranceTypes;
            //return await _insuranceTypeRepo.GetAllWithPredicate(x => x.Status != "Disable");
        }
        public async Task<IEnumerable<InsuranceSubType>> GetAllSubInsuranceType()
        {
            var insuranceTypes = await _insuranceSubTypeRepo.GetWithInclude(
                i => (i.Status != BrokerStatus.Disable.ToString() && i.Status != BrokerStatus.Inactive.ToString()),
                //i => i.Broker,
                i => i.InsuranceType
                );
            return insuranceTypes;
            //return await _insuranceTypeRepo.GetAllWithPredicate(x => x.Status != "Disable");
        }
        public async Task<IEnumerable<BrokerSubInsuranceType>> GetAllBrokerInsuranceSubType()
        {
            var insuranceTypes = await _brokerinsuranceSubTypeRepo.GetWithInclude(
                i =>( i.Status != BrokerStatus.Disable.ToString() && i.Status != BrokerStatus.Inactive.ToString()),
                i => i.Broker,
                i => i.InsuranceType.InsuranceType
                );
            return insuranceTypes;
            //return await _insuranceTypeRepo.GetAllWithPredicate(x => x.Status != "Disable");
        }
        public async Task<IEnumerable<BrokerInsuranceType>> GetAllBrokerInsuranceType()
        {
            var insuranceTypes = await _brokerinsuranceTypeRepo.GetWithInclude(
                i => (i.Status != BrokerStatus.Disable.ToString() && i.Status != BrokerStatus.Inactive.ToString()),
                i => i.Broker,
                i=> i.InsuranceType
                );
            return insuranceTypes;
            //return await _insuranceTypeRepo.GetAllWithPredicate(x => x.Status != "Disable");
        }
        public async Task<IEnumerable<InsuranceSubType>> GetInsuranceSubTypesByInsuranceType(int insuranceTypeId)
        {
            var insuranceTypes = await _insuranceSubTypeRepo.GetWithInclude(
                i => (i.Status != BrokerStatus.Disable.ToString()  && i.Status != BrokerStatus.Inactive.ToString() && i.InsuranceTypeId == insuranceTypeId),
                //i => i.Broker,
                i => i.InsuranceType
                );
            return insuranceTypes;
            //return await _insuranceSubTypeRepo.GetAllWithPredicate(x => x.Status != "Disable");
        }
        public async Task<IEnumerable<BrokerSubInsuranceType>> GetAllbrokerSubInsuranceType()
        {
            var insuranceTypes = await _brokerinsuranceSubTypeRepo.GetWithInclude(
                i =>( i.Status != BrokerStatus.Disable.ToString() && i.Status != BrokerStatus.Inactive.ToString()),
                i => i.Broker,
                i => i.InsuranceType
                );
            return insuranceTypes;
            //return await _insuranceSubTypeRepo.GetAllWithPredicate(x => x.Status != "Disable");
        }
        public async Task<IEnumerable<Underwriter>> GetAllUnderwriter()
        {
            var underwrites = await _underwriteRepo.GetWithInclude(
                i => (i.Status != BrokerStatus.Disable.ToString() && i.Status != BrokerStatus.Inactive.ToString()),
                i => i.Broker
                );
            return underwrites;
            //return await _underwriteRepo.GetAllWithPredicate(x => x.Status != "Disable");
        }
        public async Task<IEnumerable<Underwriter>> GetUnderwriters(int brokerid)
        {
            var underwrites = await _underwriteRepo.GetWithInclude(
                i => (i.Status != BrokerStatus.Disable.ToString() && i.Status != BrokerStatus.Inactive.ToString() && i.BrokerId == brokerid),
                i => i.Broker
                );
            return underwrites;
            //return await _underwriteRepo.GetAllWithPredicate(x => x.Status != "Disable");
        }





        //public bool RejectRequest(InsuranceRequest request)
        //{
        //    request.Status = RequestStatus.Rejected;
        //}

        //public bool CompleteRequest(InsuranceRequest request)
        //{
        //    // Update db record

        //    request.Status = RequestStatus.Completed;
        //}
        public async Task<string> AssignUnderwriter(InsuranceTable Insurance, Request request)
        {

            try
            {


                // Assign underwriter
                //  getrequest.UnderwriterId = request.UnderwriterId;

                // Update status
                Insurance.Stage = InsuranceStage.CertificateUploaded.ToString();

                // Save changes
                _InsuranceTbRepo.Update(Insurance);
                _reqRepo.Update(request);
                var underwriter = await _underwriteRepo.GetWithPredicate(x => x.Id == request.UnderwriterId);
                //  var broker = await _BrokerRepo.GetWithPredicate(x => x.Id == underwriter.BrokerId);
                //var detail = JsonConvert.SerializeObject(request);
                string emailBody =
                    $"We are pleased to inform you that Request {request.RequestID} has been assigned to you. See details below:.<br><br>" +
                    "Details:<br>" +
                    "Request Type: Insurance Request<br>" +
                    $"Customer Name: {request.CustomerName}<br>" +
                    $"Broker Name: {request.Broker.BrokerName}<br>" +
                    $"Broker Email: {request.Broker.EmailAddress}<br><br>" +
                    "Thank you for using our services.<br>";
                   
                string body = _utilityService.BuildEmailTemplate(underwriter.Name, "New insurance request assigned", emailBody);

                _emailService.SmtpSendMail(underwriter.EmailAddress, body, "New insurance request assigned");
                return "Successfully";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), $"AssignUnderwriter");
                Insurance.ErrorMessage = ex.InnerException.ToString();
                var updateRepuest = _InsuranceTbRepo.Update(Insurance);
                return "UnSuccessFul";
            }

        }
       
        // Upload certificate
        public async Task<string> GetRelativePath(string base64String, string contentType, string file_name)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                throw new ArgumentException("File path is null or empty.", nameof(base64String));
            }
            var certificate = _utilityService.ConvertBase64ToIFormFile(base64String, contentType, file_name);
            var uploadtopath = await _utilityService.GenerateFilePath(certificate);

            // Use Path.GetFileName to get the file name from the path
            string fileName = Path.GetFileName(uploadtopath);

            return fileName;

        }

       



        public string CreateBroker(Broker request)
        {
            try
            {
                // Save new request to db
                //var newbroker =new  Broker{
                //     AccountName = request.AccountName,
                //     EmailAddress = request.CustomerEmail,
                //     CustomerID = request.CustomerID,
                //     BrokerName = request.CustomerName
                // };
                var existaccountnumber = _BrokerRepo.GetWithPredicate(x => x.AccountNumber == request.AccountNumber && x.Status == BrokerStatus.Active.ToString());
                if (existaccountnumber == null)
                {
                    var insert = _BrokerRepo.Insert(request);
                    if (insert == true) return "Successfully";
                }
              
                return "UnSuccessful accountnumber already exist";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "CreateBroker");
                return "Error";
            }

        }
        public string UpdateBroker(Broker request)
        {
            try
            {
                var update = _BrokerRepo.Update(request);
                if (update == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "UpdateBroker");
                return "Error";
            }

        }
        public string CreateUnderwriter(Underwriter request)
        {
            try
            {
                // Save new request to db

                var insert = _underwriteRepo.Insert(request);
                if (insert == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "CreateBroker");
                return "Error";
            }

        }
        public string UpdateUnderwriter(Underwriter request)
        {
            try
            {
                var update = _underwriteRepo.Update(request);
                if (update == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "UpdateUnderwriter");
                return "Error";
            }

        }
       
        public string UpdateComment(Comments request)
        {
            try
            {
                var update = _commRepo.Update(request);
                if (update == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "UpdateUnderwriter");
                return "Error";
            }

        }
        public async Task<Comments> GetLastComment(string requestId)
        {
            try
            {

                return await _commRepo.GetLastWithPredicate(x => x.RequestID == requestId,i => i.ID);

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "GetLastComment");
                return null;
            }

        }
        public string InsertComment(Comments request)
        {
            try
            {
                var update = _commRepo.Insert(request);
                if (update == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "UpdateUnderwriter");
                return "Error";
            }

        }
        public string CreateBrokerInsuranceType(BrokerInsuranceType request)
        {
            try
            {
                // Save new request to db

                var insert = _brokerinsuranceTypeRepo.Insert(request);
                if (insert == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "CreateInsuranceType");
                return "Error";
            }

        }
        public string CreateInsuranceType(InsuranceType request)
        {
            try
            {
                // Save new request to db

                var insert = _insuranceTypeRepo.Insert(request);
                if (insert == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "CreateInsuranceType");
                return "Error";
            }

        }
        public async Task<BrokerSubInsuranceType> GetBrokerInsuranceSubTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _brokerinsuranceSubTypeRepo.GetWithPredicate(r => r.Id == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "GetBrokerInsuranceSubTypebyId");
                return null;
            }

        }

        public async Task<InsuranceSubType> GetInsuranceSubTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _insuranceSubTypeRepo.GetWithPredicate(r => r.Id == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "GetInsuranceSubTypebyId");
                return null;
            }

        }
        public string UpdateBrokerInsuranceType(BrokerInsuranceType request)
        {
            try
            {
                var update = _brokerinsuranceTypeRepo.Update(request);
                if (update == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "UpdateBrokerInsuranceType");
                return "Error";
            }

        }
        public string UpdateInsuranceType(InsuranceType request)
        {
            try
            {
                var update = _insuranceTypeRepo.Update(request);
                if (update == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "UpdateInsuranceType");
                return "Error";
            }

        }
        public async Task<string> CreateBrokerInsuranceSubType(BrokerSubInsuranceType request)
        {
            try
            {

                var existbrokerinsuranceSubType = _brokerinsuranceSubTypeRepo.GetWithPredicate(x => x.BrokerId == request.BrokerId && x.BrokerInsuranceTypeId == request.BrokerInsuranceTypeId && x.Status ==  BrokerStatus.Active.ToString());
                if(existbrokerinsuranceSubType == null)
                {
                    var insert = _brokerinsuranceSubTypeRepo.Insert(request);
                    //var getinsuranceSub = await _brokerinsuranceSubTypeRepo.GetWithPredicate(x => x.InsuranceTypeId == request.InsuranceTypeId);
                    //var getinsurance = _insuranceTypeRepo.GetById(request.InsuranceTypeId);
                    //getinsurance.InsuranceSubType = getinsuranceSub.Id;
                    //var update = _insuranceTypeRepo.Update(getinsurance);
                    if (insert == true) return "Successfully";
                }
               
                return "UnSuccessful  brokerinsuranceSubType already exist";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "CreateInsuranceSubType");
                return "Error";
            }

        }
        public async Task<string> CreateInsuranceSubType(InsuranceSubType request)
        {
            try
            {


                var insert = _insuranceSubTypeRepo.Insert(request);
                var getinsuranceSub = await _insuranceSubTypeRepo.GetWithPredicate(x => x.InsuranceTypeId == request.InsuranceTypeId);
                var getinsurance = _insuranceTypeRepo.GetById(request.InsuranceTypeId);
                //getinsurance.InsuranceSubType = getinsuranceSub.Id;
                //var update = _insuranceTypeRepo.Update(getinsurance);
                if (insert == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "CreateInsuranceSubType");
                return "Error";
            }

        }
        public string UpdateInsuranceSubType(InsuranceSubType request)
        {
            try
            {
                var update = _insuranceSubTypeRepo.Update(request);
                if (update == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "UpdateInsuranceSubType");
                return "Error";
            }

        }
        public string UpdateBrokerInsuranceSubType(BrokerSubInsuranceType request)
        {
            try
            {
                var update = _brokerinsuranceSubTypeRepo.Update(request);
                if (update == true) return "Successfully";
                return "UnSuccessful";

            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "UpdateBrokerInsuranceSubType");
                return "Error";
            }

        }
        public async Task<RecordReport> GetInsuranceInformationAsync(InsuranceTable insurance,int id)
        {
            try
            {


                // Fetch request with includes
                var request = await _reqRepo.GetWithIncludeAsync(r => r.RequestID == insurance.RequestID,
                    r => r.Broker,
                    r => r.Underwriter,
                    r => r.InsuranceType,
                    r => r.InsuranceSubType
                    //r => r.Customer
                    // r => r.ContractId
                    );
                var getfund = await _fundRepo.GetWithInclude(x => (x.RequestID == insurance.RequestID && x.InsuranceTableId == insurance.ID),
                    x => x.InsuranceTable);
                string premiumUniqueID = "";
                string commissionUniqueID = "";
                decimal commissionAmount = 0;

                if (getfund != null)
                {
                    foreach (var i in getfund)
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

                // Map to info model
                var info = new RecordReport
                {
                    Id = 1,
                    ContractId = request.ContractID,
                    AccountNumber = request.AccountNo,
                    AccountName = request.AccountName,
                    CustomerName = request.CustomerName,
                    CustomerEmail = request.CustomerEmail,
                    PolicyNo = insurance.PolicyNo,
                    CollateralValue = request.CollateralValue,
                    Broker = request.Broker.BrokerName,
                    Underwriter = request.Underwriter?.Name,
                     DateofIssuance = Convert.ToDateTime(insurance.PolicyIssuanceDate),
                    PolicyExpiryDate = Convert.ToDateTime(insurance.PolicyExpiryDate),
                    PolicyDuration = $"{difference.Days} Days",
                    InsuranceType = request.InsuranceType.Name,
                    SubInsuranceType = request.InsuranceSubType?.Name,
                    PremiumAmount = request.UpdatedPremium,
                    PremiumCRAccount = request.Broker.AccountNumber,
                    PremiumDRAccount = request.AccountNo,
                    PremiumUniqueID = premiumUniqueID,
                    CommissionAmount = commissionAmount,
                    CommissionUniqueID = commissionUniqueID,
                    InsuranceFlag = insurance.Stage,
                    Stage = insurance.Stage,
                    Branch = request.Branchcode,

                    //RecordId = Convert.ToInt64(request.RequestID),
                    //ContractId = request.ContractID,
                    //AccountNumber = request.AccountNo,
                    //AccountName = request.AccountName,
                    //InsuranceType = request.InsuranceType.InsuranceType.Name,
                    //DebitPassed = insurance.Serial,
                    //ContractMaturityDate = request.ContractMaturityDate,
                    //CustomerName = request.CustomerName,
                    //CollateralValue = request.CollateralValue,
                    //CustomerEmail = request.CustomerEmail,
                    //SubInsuranceType = request.InsuranceSubType?.Name,
                    //EstimatedPremium = request.UpdatedPremium,
                    //Underwriter = request.Underwriter?.Name,
                    //Broker = request.Broker.BrokerName,
                    //CustomerID = request.CustomerID,
                    //InsuranceFlag = insurance.Stage,
                    //Certificate = insurance.FileName,
                    //PolicyNo = insurance.PolicyNo,
                    //CommissionAmount = commissionAmount,
                    //DateofIssuance = Convert.ToDateTime(insurance.PolicyIssuanceDate),
                    //PolicyExpiryDate = Convert.ToDateTime(insurance.PolicyExpiryDate),
                    //CommissionUniqueID = commissionUniqueID,
                    //PremiumAmount = request.UpdatedPremium,
                    //PremiumCRAccount = request.Broker.AccountNumber,
                    //PremiumDRAccount = request.AccountNo,
                    //PremiumUniqueID = premiumUniqueID,
                    //Stage = insurance.Stage,
                    //Branch = request.Branchcode,
                    //InsuranceStatus = insurance.RequestType,
                    //PolicyDuration = $"{difference.Days} Days",
                    //Id = 1

                };

                return info;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "GetInsuranceInformationAsync");
                throw;
            }
        }
        public async Task<IEnumerable<InsuranceTable>> BuildInsuranceReportQuery(InsuranceReportQuery filters)
        {
            try
            {
                // Start with a base query
                var baseQuery = _reqRepo.GetBaseQuery();

                if (!string.IsNullOrEmpty(filters.ContractId))
                {
                    baseQuery = baseQuery.Where(r => r.ContractID == filters.ContractId);
                }

                if (!string.IsNullOrEmpty(filters.CustomerId))
                {
                    var customerId = filters.CustomerId;
                    baseQuery = baseQuery.Where(r => r.CustomerID == customerId);
                }

                if (!string.IsNullOrEmpty(filters.CustomerName))
                {
                    baseQuery = baseQuery.Where(r => r.CustomerName.Contains(filters.CustomerName));
                }


                // Use your GetAllWithPredicate method to execute the filtered query
                var request = await _reqRepo.GetAllWithQuery(baseQuery);
                List<InsuranceTable> Insurance = new List<InsuranceTable>();
                foreach (var item in request)
                {
                    var req = await _InsuranceTbRepo.GetWithPredicate(x => x.RequestID == item.RequestID);
                    Insurance.Add(req);
                }
                return Insurance;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "BuildInsuranceReportQuery");
                throw;
            }
        }
        public async Task<IEnumerable<InsuranceTable>> BuildInsuranceReport(InsuranceReportViewModel filters)
        {
            try
            {


                // Start with a base query
                //var baseQuery = _InsuranceRepo.GetBaseQuery();
                var baseQuery = _InsuranceTbRepo.GetBaseQuery();
                if (filters.PolicyStartDate != DateTime.MinValue)
                {
                    baseQuery = baseQuery.Where(r => r.RequestDate >= filters.PolicyStartDate);
                }
                if (filters.PolicyEndDate != DateTime.MinValue)
                {
                    baseQuery = baseQuery.Where(r => r.RequestDate <= filters.PolicyEndDate);
                }
                if (filters.InsuranceFlag != null)
                {
                    baseQuery = baseQuery.Where(r => r.Stage.ToLower() == filters.InsuranceFlag.ToLower());
                }

                // baseQuery = baseQuery.Where(r => r.RequestDate >= filters.PolicyStartDate && r.RequestDate <= filters.PolicyEndDate);


                return await _InsuranceTbRepo.GetAllWithQuery(baseQuery);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "BuildInsuranceReport");
                throw;
            }
        }
        public async Task<IEnumerable<RecordReport>> MapInsuranceRequestsToInfo(IEnumerable<InsuranceTable> requestList)
        {
            try
            {


                var info = new List<RecordReport>();
                List<InsuranceTable> request = requestList.ToList();
                for (int item = 0; item < request.Count; item++)
                // foreach (var request in requests)
                {
                    // Map request to info model
                    var insuranceInfo = await GetInsuranceInformationAsync(request[item],item);

                    info.Add(insuranceInfo);
                }

                return info;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "MapInsuranceRequestsToInfo");
                throw;
            }
        }

       
        public async Task<string> ReviewCertificateUploaded(InsuranceTable request,Request model)
        {

            try
            {
                var updateRepuest = _reqRepo.Update(model);


                var deleteFile = _utilityService.DeleteFile(request.FileName);

                request.Stage = InsuranceStage.ApprovedCertificate.ToString();
                var updateInsurance= _InsuranceTbRepo.Update(request);
                if (updateInsurance == true) return "Successfully";
                return "UnSuccessful";
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "ReviewCertificate");
                request.ErrorMessage = ex.InnerException.ToString();
                var updateRepuest = _InsuranceTbRepo.Update(request);

                return "UnSuccessFul";
            }

        }
        public async Task<IEnumerable<InsuranceSubType>> GetAllInsuranceSubTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _insuranceSubTypeRepo.GetAllWithPredicate(r => r.InsuranceTypeId == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "GetAllInsuranceSubTypebyId");
                return null;
            }

        }
        public async Task<IEnumerable<BrokerSubInsuranceType>> GetAllBrokerInsuranceSubTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _brokerinsuranceSubTypeRepo.GetAllWithPredicate(r => r.BrokerInsuranceTypeId == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "GetAllBrokerInsuranceSubTypebyId");
                return null;
            }

        }

        public async Task<IEnumerable<InsuranceSubType>> InsuranceSubTypebyId(int requestid)
        {
            try
            {
                var getinsuranctype = await _brokerinsuranceTypeRepo.GetWithPredicate(r => r.Id == requestid);

                var getrequest = await _insuranceSubTypeRepo.GetAllWithPredicate(r => r.InsuranceTypeId == getinsuranctype.InsuranceTypeId);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "InsuranceSubTypebyId");
                return null;
            }

        }
        public async Task<IEnumerable<InsuranceType>> InsuranceTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _insuranceTypeRepo.GetAllWithPredicate(r => r.Id == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "InsuranceTypebyId");
                return null;
            }

        }
        public async Task<IEnumerable<InsuranceType>> GetAllInsuranceTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _insuranceTypeRepo.GetAllWithPredicate(r => r.Id/*BrokerId*/ == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "GetAllInsuranceTypebyId");
                return null;
            }

        }

        public async Task<IEnumerable<BrokerInsuranceType>> GetAllbrokerInsuranceTypebyId(int requestid)
        {
            try
            {
                var getrequest = await _brokerinsuranceTypeRepo.GetWithInclude(
                i => (i.Status == BrokerStatus.Active.ToString() && i.BrokerId == requestid),
                i => i.Broker,
                i => i.InsuranceType
                );


                //GetAllWithPredicate(r => r.BrokerId == requestid);

                if (getrequest == null) return null;
                return getrequest;
            }
            catch (Exception ex)
            {
                _logging.LogFatal(ex.ToString(), "GetAllbrokerInsuranceTypebyId");
                return null;
            }

        }

    }
}
