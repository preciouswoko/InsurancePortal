using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InsuranceCore.DTO;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using static InsuranceCore.DTO.ReusableVariables;
using Microsoft.AspNetCore.StaticFiles;
using InsuranceCore.Enums;
using static InsuranceInfrastructure.Helpers.AccessControlAttribute;
using InsuranceManagement.ViewModels;
using InsuranceInfrastructure.Middlewares;
using System.Text;
using CsvHelper;

namespace InsuranceManagement.Controllers
{
    [TypeFilter(typeof(AuditFilterAttribute))]
    public class InsuranceController : Controller
    {
        //private readonly ILogger<InsuranceController> _logger;
        private readonly IInsuranceService _service;
        private readonly ILoggingService _logging;
        private readonly IRequestService _reqservice;
        private readonly IHttpClientService _httpClientService;
        private IHttpContextAccessor _hcontext;
        TemporaryVariables temporaryVariables;
        GlobalVariables globalVariables;
        private readonly GlobalVariables _globalVariables;
        private readonly TemporaryVariables _temporaryVariables;
        private readonly ISessionService _session;
        public InsuranceController(IHttpClientService httpClientService, IHttpContextAccessor hcontext,/* ILogger<InsuranceController> logger,*/
            ISessionService session, IInsuranceService service, ILoggingService logging, IRequestService reqservice)
        {
            //_logger = logger;
            _logging = logging;
            _service = service;
            _reqservice = reqservice;
            _httpClientService = httpClientService;
            _hcontext = hcontext;
            _session = session;
            _globalVariables = _session.Get<GlobalVariables>("GlobalVariables");
            _temporaryVariables = _session.Get<TemporaryVariables>("TemporaryVariables");
        }
        public IActionResult Index()
        {
            _session.Set<GlobalVariables>("GlobalVariables", globalVariables);

            return View();
        }
        public async Task<IActionResult> Report()
        {
            var reports = await _reqservice.ReportReport();

            return View(reports);
        }
        public IActionResult Unauthorized()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FetchAccountDetails()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FetchAccountDetails(string accountNumber)
        {

            if (accountNumber == null) return null;
            var getaccountdetail = await _service.FetchDetail(accountNumber.Trim());
            //var accountDetail = new AccountDetail
            //{
            //    // Populate properties based on fetched data
            //    AccountName = "Sample Account Name",
            //    CustomerName = "Sample Customer Name",
            //    T24CustomerID = "Sample Customer ID",
            //    CustomerEmail = "sample@example.com"
            //};
            if (getaccountdetail == null) return null;
            _logging.LogInformation(JsonConvert.SerializeObject(getaccountdetail), "FetchAccountDetails");

            return Json(getaccountdetail);
        }
        //[HttpPost]
        //public async Task<AccountDetail> FetchAccountDetail(string AccountNUmber)
        //{
        //   return  await _service.FetchDetail(AccountNUmber);
        //}
        //[EncryptionActionAttribute]
        //[HttpGet]
        //public IActionResult GetAllInsurance()
        //{
        //   // var getall = await _service.GetAllInsuranceRequest();
        //    return View();
        //    //var response = await _reqservice.FetchInsurancesData();
        //    //return Json(response);
        //}
        //  [AccessControl( "INR", "AUR")]
        [HttpPost]
        public async Task<IActionResult> FetchInsurances(DataTablesRequest request)
        {
            try
            {
                var json = await _reqservice.FetchInsurancesForDataTableAsync(request);
                //var test = JsonConvert.SerializeObject(json);
                // _logging.LogInformation($"Received JSON data: {JsonConvert.SerializeObject(json.data)}", "FetchInsurances");
                return Json(json);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "FetchInsurances");
                throw;
            }


        }
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public async Task<IActionResult> FetchInsurancesWithoutCodition()
        //{
        //    try
        //    {
        //        var data = await _reqservice.ProcessInsuranceRequest();

        //        // Create a DataTablesResponse object with the simulated data
        //        var dataResponse = new DataTablesResponse(1, data, data.Count, data.Count);

        //        return Json(dataResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.ToString(), "FetchInsurancesWithoutCodition");
        //        throw;
        //    }


        //}
        public IActionResult GetAllInsurance()
        {
            return View();

        }
        public async Task<IActionResult> AuthorizeRequest(string message = null)
        {
            try
            {
                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.AUR))) return RedirectToAction("Unauthorized");
                var getAll = await _reqservice.GetAllNeededforAuth();
                //await _reqservice.GetAllNeeded(r => r.Stage == InsuranceStatus.New.ToString()  &&  r.UserId != _globalVariables.userid);
                _logging.LogInformation(getAll.Count().ToString(), "AuthorizeRequest");

                if (getAll == null)
                {
                    return RedirectToAction("GetAllInsurance");
                }

                var records = new List<AuthorizeRequestViewModel>();
                var approveRejectList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Approve", Value = RequestStatus.Approved.ToString() },
                    new SelectListItem { Text = "Reject", Value = RequestStatus.Rejected.ToString() }
                };
                ViewBag.ApproveRejectList = new SelectList(new[]
                {
                    new { Text = "Approve", Value = "Approved" },
                    new { Text = "Reject", Value = "Rejected" }
                }, "Value", "Text");

                // Set it in ViewBag
                //ViewBag.ApproveRejectList = new SelectList(approveRejectList, "Value", "Text");

                foreach (var item in getAll)
                {


                    var record = new AuthorizeRequestViewModel()

                    {
                        RequestId = Convert.ToInt64(item.RequestID),
                        Comment = null,
                        IsApproved = null,
                        T24CustomerID = item.CustomerID,
                        AccountName = item.AccountName,
                        CustomerName = item.CustomerName,
                        CustomerEmail = item.CustomerEmail,
                        AccountNumber = item.AccountNo,
                        EstimatedPremium = item.UpdatedPremium,
                        InsuranceType = item.InsuranceType.InsuranceType.Name,/*.Name,*/
                        //InsuranceType = getInsuranceType.Name,
                        InsuranceSubType = item.InsuranceSubType?.Name, /*getInsuranceSubType?.Name ?? null,*/
                        Broker = item.Broker.AccountName
                        //,ApprovalStatus = ""
                    };

                    records.Add(record);
                }
                if (message != null)
                {
                    if (message.Contains("Error"))
                    {
                        ViewData["Error"] = message;
                    }
                    else
                    {
                        ViewData["Message"] = message;
                    }

                }
                return View(records);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "AuthorizeRequest");
                throw;
            }
        }
        [HttpPost]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> AuthorizeRequest(string requestId, string comment, string approvalStatus)
        {
            try
            {
                string message = "";
                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.AUR))) return RedirectToAction("Unauthorized");
                _logging.LogInformation($"Inside AuthorizeRequest for this request{requestId}", "Post _AuthorizeRequest");

                // Handle form submit logic
                var getRequest = await _service.Getrequest(requestId.ToString());

                var getinsurance = await _service.GetInsurancereq(requestId);
                //_service.GetInsuranceRequest(requestId);
                if (getinsurance.RequestByemail == _globalVariables.Email) return RedirectToAction("Unauthorized"); //return Unauthorized();
                getinsurance.AuthorizedByEmail = _globalVariables.Email;
                getinsurance.AuthorizedByName = _globalVariables.name;
                getinsurance.AuthorizedByUsername = _globalVariables.userName;
                getinsurance.AuthorizedDate = DateTime.Now;
                var commentreq = new Comments()
                {
                    RequestID = requestId.ToString(),
                    CommentDate = DateTime.Now,
                    Comment = comment,
                    CommentBy = /*$"{_globalVariables.name}[*/ $"{_globalVariables.userName}",
                    Action = InsuranceStage.AuthorizeRequest.ToString(),
                    Serial = getinsurance.Serial

                };

                var updateComment = _service.InsertComment(commentreq);
                if (approvalStatus == RequestStatus.Rejected.ToString())
                {

                    getRequest.Status = RequestStatus.Rejected.ToString();
                    string name = getinsurance.RequestByName;
                    string email = getinsurance.RequestByemail;
                    var update = _reqservice.UpdateRequest(getRequest, getinsurance, email, name, comment);
                    if (update.StartsWith("Success"))
                    {
                         message = update + " Rejected  Record";
                        TempData["ResultMessage"] = message;
                    }
                    else
                    {
                        message = "Error: Failed to Update the request.";
                        TempData["ResultMessage"] = message;
                    }
                }
                else if (approvalStatus == RequestStatus.Approved.ToString())
                {
                    getRequest.Status = RequestStatus.Approved.ToString();


                    var authorize = await _service.AuthorizeRequest(getinsurance);
                    if (authorize.StartsWith("Success"))
                    {
                        message = authorize + " Approved  Record";
                        TempData["ResultMessage"] = message;
                    }
                    else
                    {
                        message = "Error: Failed to authorize the request.";
                        TempData["ResultMessage"] = message;
                    }
                }

                return RedirectToAction(nameof(AuthorizeRequest), new { message = message });
                //  return RedirectToAction("AuthorizeRequest");
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "AuthorizeRequest");
                throw;
            }
           
        }

        public static string GetPermissionName(string permissionConstant)
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
        [HttpPost]
        public IActionResult ExportToExcel(string tableDataJson)
        {

            if (tableDataJson != null)
            {
                var records = JsonConvert.DeserializeObject<IEnumerable<RecordReport>>(tableDataJson);

                // Create a MemoryStream to write CSV data
                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                using (var csvWriter = new CsvWriter(streamWriter))
                {
                    // Write records to the CSV writer
                    csvWriter.WriteRecords(records);
                    streamWriter.Flush();

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Now you can read the CSV data from memoryStream as a string
                    using (var reader = new StreamReader(memoryStream))
                    {
                        var csvData = reader.ReadToEnd();

                        // Convert the CSV data string to bytes and return as a file
                        return File(Encoding.UTF8.GetBytes(csvData), "text/csv", $"GeneratedReport-{DateTime.Now}.csv");
                    }
                }
            }


            return RedirectToAction("Report");
        }

        [HttpPost]
        public async Task<ActionResult> GenerateReport(InsuranceReportViewModel model)
        {
            if (model.InsuranceFlag == null && model.PolicyEndDate == DateTime.MinValue &&
                model.PolicyStartDate == DateTime.MinValue)
            {
                return RedirectToAction("GetAllInsurance");

            }
            else

            {
                var filter = await _service.BuildInsuranceReport(model);
                var records = await _service.MapInsuranceRequestsToInfo(filter);

                // Create a MemoryStream to write CSV data
                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                using (var csvWriter = new CsvWriter(streamWriter))
                {
                    // Write records to the CSV writer
                    csvWriter.WriteRecords(records);
                    streamWriter.Flush();

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Now you can read the CSV data from memoryStream as a string
                    using (var reader = new StreamReader(memoryStream))
                    {
                        var csvData = reader.ReadToEnd();

                        // Convert the CSV data string to bytes and return as a file
                        return File(Encoding.UTF8.GetBytes(csvData), "text/csv", $"GeneratedReport-{DateTime.Now}.csv");
                    }
                }

                //return RedirectToAction("GetAllInsurance");
            }
        }

        public async Task<ActionResult> GetAllRequest(string message = null)
        {
            try
            {
                var statusList = Enum.GetValues(typeof(CommentStatus))
                        .Cast<CommentStatus>()
                        .Select(e => new SelectListItem
                        {
                            Text = e.ToString(),
                            Value = ((int)e).ToString()
                        });

                // Assign the SelectList to a ViewBag property
                ViewBag.CommentStatusList = new SelectList(statusList, "Value", "Text");
                

                var records = new List<RequestReviewViewModel>();
                var getrecord = await _reqservice.GetInsuranceByRequester(_globalVariables.Email);
                _logging.LogInformation(getrecord.Count().ToString(), "GetAllRequest");

                if (getrecord == null)
                {
                    return View();
                }
                foreach (var item in getrecord)
                {
                    var request = await _reqservice.GetRequestDetailsForInsuranceRequestsAsync(item.RequestID);
                    var getcomment = await _service.GetLastComment(item.RequestID);

                    var record = new RequestReviewViewModel()

                    {
                        RequestID = Convert.ToInt64(item.RequestID),
                        CustomerId = request.CustomerID,
                        AccountName = request.AccountName,
                        CustomerName = request.CustomerName,
                        CustomerEmail = request.CustomerEmail,
                        AccountNumber = request.AccountNo,
                        Premium = request.UpdatedPremium,
                        EstimatedPremium = 0,
                        InsuranceType = request.InsuranceType.InsuranceType.Name,
                        InsuranceSubTypes = request.InsuranceSubType?.Name,
                        Broker = request.Broker.BrokerName,
                        Underwrite = request.Underwriter?.Name,
                        CollateralValueFSV = request.CollateralValue,
                        AuthorizedByName =item?.AuthorizedByName,
                        InsuranceFlag = item.Stage,
                       Comment = getcomment?.Comment,
                       ErrorMessage = item?.ErrorMessage,
                       RequestType = item.RequestType,
                       Status = request.Status,
                       Stage = item.Stage
                    };

                    records.Add(record);
                }
                if (message != null)
                {
                    if (message.Contains("Error"))
                    {
                        ViewData["Error"] = message;
                    }
                    else
                    {
                        ViewData["Message"] = message;
                    }

                }
                return View(records);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "GetAllRequest");
                throw;
            }
            
        }

        [HttpPost]
        public async Task<ActionResult> ModifyRequest(long requestId,  decimal premium, string comment, string Status)
        {
            try
            {
                string message = "";
                _logging.LogInformation($"Inside ModifyRequest for this request{requestId}", "Post ModifyRequest");

                var approvalStatus = _reqservice.GetEnumValueByIndex1(Convert.ToInt32(Status) + 1).ToString();

                var getInsurance = await _service.GetInsurancereq(requestId.ToString());
                  if (getInsurance.Status != "Rejected" && (getInsurance.Stage != InsuranceStage.AuthorizeRequest.ToString() || getInsurance.Stage != InsuranceStage.UnderwriterAssigned.ToString()))
                {
                    message = "Error: Failed to Update the request because request type is awaiting renewal.";
                    TempData["ResultMessage"] = message;
                   // return RedirectToAction("GetAllRequest");
                    return RedirectToAction(nameof(GetAllRequest), new { message = message });
                }
              

                if (getInsurance.RequestType == "Renewal")
                {
                    message = "Error: Failed to Update the request because request type is renewal.";
                    TempData["ResultMessage"] = message;
                    return RedirectToAction(nameof(GetAllRequest), new { message = message });

                }
                var getRequest = await _service.Getrequest(requestId.ToString());
                var commentreq = new Comments()
                {
                    RequestID = requestId.ToString(),
                    CommentDate = DateTime.Now,
                    Comment = comment + $": Estimated premium ={premium}_ Selected Status ={approvalStatus}",
                    CommentBy = /*{_globalVariables.name}*/ $"{_globalVariables.userName}",
                    Action = "ModifyRequest",
                    Serial = getInsurance.Serial

                };
                if (getRequest == null)
                {
                    message = "Error: Failed to assign underwriter.";
                    TempData["ResultMessage"] = message;
                   // return RedirectToAction("ModifyRequest");
                    return RedirectToAction(nameof(ModifyRequest), new { message = message });

                }
                getRequest.UpdatedPremium = premium;
                getRequest.Status = approvalStatus;
                var insertComment = _service.InsertComment(commentreq);
                string name = getInsurance.AuthorizedByName;
                string email = getInsurance.AuthorizedByEmail;
                var update = _reqservice.UpdateRequest(getRequest, getInsurance,email,name,comment);
                if (update.StartsWith("Success"))
                {
                    message = update + " Rejected  Record";
                    TempData["ResultMessage"] = message;
                }
                else
                {
                    message = "Error: Failed to Update the request.";
                    TempData["ResultMessage"] = message;
                }
                //return RedirectToAction("GetAllRequest");
                return RedirectToAction(nameof(GetAllRequest), new { message = message });

            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "ModifyRequest");
                throw;
            }
        }



        [HttpGet]
        public async Task<IActionResult> CreateRequest()
        {
            var brokers = await _service.GetAllBroker();
            var insurance = await _service.GetAllInsuranceType();
            // var underwrites = await _service.GetAllUnderwriter();
            var insuranceTypes = await _service.GetAllSubInsuranceType();

            if (brokers == null || insurance == null)
            {
                // Handle the case where one or both collections are null
                // You may want to return an error view or handle it differently
                return RedirectToAction("Error");
            }

            // Create a SelectList for the dropdowns
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
            ViewBag.InsuranceTypeId = new SelectList(insurance.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");
            // ViewBag.UnderwriterId = new SelectList(underwrites.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");

            ViewBag.InsuranceSubTypeId = new SelectList(insuranceTypes.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");

            // Rest of your code...

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetInsuranceSubTypesByInsuranceType(int insuranceTypeId)
        {
            // Retrieve InsuranceSubTypes based on the selected InsuranceTypeId
            var insuranceSubTypes = await _service.GetInsuranceSubTypesByInsuranceType(insuranceTypeId);

            // Convert InsuranceSubTypes to a SelectList
            var selectList = new SelectList(insuranceSubTypes, "Value", "Text");

            return Json(selectList);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRequest(Request request)
        {
            //if (ModelState.IsValid)
            //{
            var isSuccess = await _reqservice.CreateRequest(request);

            if (isSuccess == "Successful")
            {
                TempData["SuccessMessage"] = "Request created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Request creation failed.";
                return RedirectToAction("CreateRequest");

            }
            return RedirectToAction("Index", "Auth");

            //}

            //    TempData["ErrorMessage"] = "Request creation failed because modelstate is not valid.";

            //            return RedirectToAction("CreateRequest");

        }



        public async Task<IActionResult> InsuranceCertificate(string message = null)
        {
            if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.UIC))) return RedirectToAction("Unauthorized");

            try
            {

                var getAll = await _reqservice.GetAllNeeded1(InsuranceStage.CertificateUploaded.ToString());

                _logging.LogInformation(getAll.Count().ToString(), "InsuranceCertificate");

                if (getAll == null)
                {
                    //return RedirectToAction("AssignUnderwriter");
                    return View();
                }

                var records = new List<UploadCertViewModel>();


                foreach (var item in getAll)
                {


                    var record = new UploadCertViewModel()

                    {
                        RequestId = Convert.ToInt64(item.RequestID),
                        T24CustomerID = item.CustomerID,
                        AccountName = item.AccountName,
                        CustomerName = item.CustomerName,
                        CustomerEmail = item.CustomerEmail,
                        AccountNumber = item.AccountNo,
                        EstimatedPremium = item.UpdatedPremium,
                        InsuranceType = item.InsuranceType.InsuranceType.Name,
                        InsuranceSubType = item.InsuranceSubType?.Name,
                        Broker = item.Broker.BrokerName,
                        Underwrite = item.Underwriter.Name,
                        Upload = "true"
                    };

                    records.Add(record);
                }
                if (message != null)
                {
                    if (message.Contains("Error"))
                    {
                        ViewData["Error"] = message;
                    }
                    else
                    {
                        ViewData["Message"] = message;
                    }

                }
                return View(records);

            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "InsuranceCertificate");
                throw;
            }
        }

        [HttpGet]
        public IActionResult UploadCertificate(long RequestId)
        {
            var model = new CertificateRequest { insuranceId = RequestId };
            return View(model);
        }

        [HttpPost]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> UploadCertificate(CertificateRequest model)
        {
            try
            {
                string text = ""; 
                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.UIC))) return RedirectToAction("Unauthorized");
                _logging.LogInformation($"Inside UploadCertificate for this request{model.insuranceId}", "Post UploadCertificate");

                string message = "File not valid";

                if (ModelState.IsValid)
                {
                    // Process the uploaded certificate file here
                    if (model.DateofIssuance >= model.ExpiryDate)
                    {
                        text = "Error: Policy Issuance Date is Greater than Policy Expiry Date";
                        TempData["ResultMessage"] = text;
                        return RedirectToAction(nameof(InsuranceCertificate), new { message = text });

                        //return RedirectToAction("InsuranceCertificate");
                    }

                    var upload = await _reqservice.UploadCertificate(model);
                    message = upload;
                    ViewBag.Message = upload;
                    text = upload + "uploaded certificate";
                    TempData["ResultMessage"] = text;

                    return RedirectToAction(nameof(InsuranceCertificate), new { message = text });
                }

                // If the model is invalid or the file upload failed, return to the upload form with validation errors
                ViewBag.Message = message;
                return RedirectToAction(nameof(InsuranceCertificate), new { message = message });

                //  return RedirectToAction("InsuranceCertificate");
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "UploadCertificate");
                throw;
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> AssignContractID(string message = null)
        {
            try
            {
                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.ACI))) return RedirectToAction("Unauthorized");

                var getrecord = await _reqservice.GetInsuranceRequestsByStageAsync(InsuranceStage.ApprovedCertificate.ToString());
                _logging.LogInformation(getrecord.Count().ToString(), "AssignContractID");


                //_reqservice.GetPendingRequests();
                var records = new List<AssignContractIDViewModel>();


                foreach (var item in getrecord)
                {
                    var request = await _reqservice.GetRequestDetailsForInsuranceRequestsAsync(item.RequestID);


                    var record = new AssignContractIDViewModel()

                    {
                        RequestId = Convert.ToInt64(item.RequestID),
                        CustomerID = request.CustomerID,
                        AccountName = request.AccountName,
                        CustomerName = request.CustomerName,
                        CustomerEmail = request.CustomerEmail,
                        AccountNumber = request.AccountNo,
                        Premium = request.UpdatedPremium,
                        InsuranceType = request.InsuranceType.InsuranceType.Name,
                        InsuranceSubType = request.InsuranceSubType?.Name,
                        Broker = request.Broker.BrokerName,
                        Underwrite = request.Underwriter.Name,
                        ContractID = " ",
                        CollateralValue = request.CollateralValue.ToString(),
                        DebitsPassed = item.Serial,
                        PolicyExpiryDate = Convert.ToDateTime(item.PolicyExpiryDate),
                        PolicyStartDate = Convert.ToDateTime(item.PolicyIssuanceDate),
                        PolicyNumber = item.PolicyNo,
                        Certificate = item.FileName
                    };

                    records.Add(record);
                }
                if (message != null)
                {
                    if (message.Contains("Error"))
                    {
                        ViewData["Error"] = message;
                    }
                    else
                    {
                        ViewData["Message"] = message;
                    }

                }
                return View(records);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "AssignContractID");
                throw;
            }
           
        }
        public async Task<IActionResult> UpdateRequest(long RequestId)
        {
            //var getdetail = await _reqservice.GetReportByID(RequestId);
            var getdetail = await _reqservice.GetRequestDetailsForInsuranceRequestsAsync(RequestId.ToString());
            return View(getdetail);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateRequest(long RequestId, string ContractID)
        {
            string message = "";
            var getRequest = await _reqservice.GetRequestDetailsForInsuranceRequestsAsync(RequestId.ToString());
            getRequest.ContractID = ContractID.Trim();
            var assign = await _service.SetContractId(getRequest);
            if (assign == true)
            {
                // Success message
                message = $"Message: Successfully updated Contract ID {ContractID}.";
                TempData["ResultMessage"] = message;
            }
            else
            {
                // Error message
                message = $" Error: Failed to update Contract ID {ContractID} , customer Id mismatch or maturityDate has expired  .";
                TempData["ResultMessage"] = message;
            }
            return RedirectToAction(nameof(AssignContractID), new { message = message });

           // return RedirectToAction("AssignContractID");

        }
        [HttpPost]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> AssignContractID(Request model)
        {
            try
            {


                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.ACI))) return RedirectToAction("Unauthorized");
                _logging.LogInformation($"Inside AssignContractID for this request{model.RequestID}", "Post AssignContractID");
                string message = "";
                if (ModelState.IsValid)
                {
                    var validContract = await _service.SetContractId(model);

                    if (validContract)
                    {
                        // Success message
                        message = "Message: Successfully assigned Contract ID";
                        TempData["ResultMessage"] = message;
                        return RedirectToAction(nameof(GetAllInsurance), new { message = message });

                       // return RedirectToAction("GetAllInsurance");
                    }
                    else
                    {
                        // Error message
                        ViewBag.Message = "Error:Contract ID doesn't match insurance details.";
                        message = " Error:Contract ID doesn't match insurance details.";
                        TempData["ResultMessage"] = message;
                        return RedirectToAction(nameof(AssignContractID), new { message = message });

                       // return RedirectToAction("AssignContractID");
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "AssignContractID");
                throw;
            }
        }



        public async Task<IActionResult> CreateBroker(string accountNo, Broker request)
        {
            var t24Detail = await _service.FetchDetail(accountNo);
            var newbroker = new Broker
            {
                AccountName = t24Detail.AccountName,
                EmailAddress = t24Detail.CustomerEmail,
                CustomerID = t24Detail.T24CustomerID,
                BrokerName = t24Detail.CustomerName
            };
            var Add = _service.CreateBroker(newbroker);
            return View(Add);
        }
        public async Task<IActionResult> UpdateBroker(Broker request)
        {
            var t24Detail = await _service.FetchDetail(request.AccountNumber);
            var broker = new Broker
            {
                AccountName = t24Detail.AccountName,
                EmailAddress = t24Detail.CustomerEmail,
                CustomerID = t24Detail.T24CustomerID,
                BrokerName = t24Detail.CustomerName
            };
            var update = _service.UpdateBroker(broker);
            return View(update);
        }

        public IActionResult UpdateUnderwrite(Underwriter request)
        {

            var update = _service.UpdateUnderwriter(request);
            return View(update);
        }

        public IActionResult UpdateInsuranceType(InsuranceType request)
        {

            var update = _service.UpdateInsuranceType(request);
            return View(update);
        }
        //public IActionResult SetUpInsuranceSubType(int brokerId, InsuranceTypeRequest request)
        //{

        //    var Add = _service.CreateInsuranceSubType(brokerId, request);
        //    return View(Add);
        //}
        public IActionResult UpdateInsuranceSubType(InsuranceSubType request)
        {

            var update = _service.UpdateInsuranceSubType(request);
            return View(update);
        }
        public IActionResult GetAllUnderwriter(string message = null)
        {
            var getAll = _service.GetAllUnderwriter();
            if (message != null)
            {
                if (message.Contains("Error"))
                {
                    ViewData["Error"] = message;
                }
                else
                {
                    ViewData["Message"] = message;
                }

            }
            return View(getAll);
        }
        [HttpGet]
        public async Task<IActionResult> AssignUnderwriter(string message = null)
        {
            try
            {


                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.ANU))) return RedirectToAction("Unauthorized");

                var getAll = await _reqservice.GetAllNeeded1(InsuranceStage.UnderwriterAssigned.ToString());
                //var detail = JsonConvert.SerializeObject(getAll);
                _logging.LogInformation(getAll.Count().ToString(), "AssignUnderwriter");

                //var underwriters = await _service.GetAllUnderwriter();
                //ViewBag.Underwriters = new SelectList(underwriters, "Id", "Name");
                if (getAll == null)
                {
                    //return RedirectToAction("AssignUnderwriter");
                    return View();
                }
                var records = new List<AssignUnderwriterViewModel>();

                foreach (var item in getAll)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    var underwritemodels = new List<Underwriters>();

                    var getunderwriters = await _service.GetUnderwriters(item.BrokerID);
                    var getinsurance = await _service.GetInsurancereq(item.RequestID);
                    foreach (var i in getunderwriters)
                    {
                        var underwritemodel = new Underwriters()
                        {
                            Id = i.Id,
                            Underwrite = i.Name
                        };
                        underwritemodels.Add(underwritemodel);
                    };

                    var record = new AssignUnderwriterViewModel()

                    {
                        RequestId = Convert.ToInt64(item.RequestID),
                        CustomerID = item.CustomerID,
                        AccountName = item.AccountName,
                        CustomerName = item.CustomerName,
                        CustomerEmail = item.CustomerEmail,
                        AccountNumber = item.AccountNo,
                        UpdatedPremium = item.UpdatedPremium ,
                        Premium = 0,
                        InsuranceType = item.InsuranceType.InsuranceType.Name,
                        InsuranceSubType = item.InsuranceSubType?.Name,
                        Broker = item.Broker.AccountName,
                        CollateralValue = item.CollateralValue.ToString(),
                        Underwriters = underwritemodels,
                        RequestType = getinsurance.RequestType
                    };

                    records.Add(record);
                }

                if (message != null)
                {
                    if (message.Contains("Error"))
                    {
                        ViewData["Error"] = message;
                    }
                    else
                    {
                        ViewData["Message"] = message;
                    }

                }
                return View(records);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "AssignUnderwriter");
                throw;
            }
        }


        public async Task<IActionResult> GetUnderwiter(int brokerId)
        {
            var underwriters = await _reqservice.GetUnderwritersbybroker(brokerId);

            // Pass the underwriters to your view
            //ViewBag.Underwriters = underwriters;

            //return View();
            return Json(underwriters);
        }



        //[HttpGet]
        //public async Task<IActionResult> AssignUnderwriter(int requestId)
        //{

        //    var underwriters = await  _service.GetAllUnderwriter();
        //    ViewBag.Underwriters = new SelectList(underwriters.Select(u => new { u.Id, u.Name }), "Id", "Name");


        //    var request = _service.GetInsuranceRequest(requestId);

        //    if (request == null)
        //    {
        //        return NotFound(); 
        //    }

        //    return View(request);
        //}

        [HttpPost]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> AssignUnderwriter(long requestId, int selectedUnderwriter, decimal premium, string comment, string approvalStatus)
        {
            try
            {


                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.ANU))) return RedirectToAction("Unauthorized");
                _logging.LogInformation($"Inside AssignUnderwriter for this request{requestId}", "Post AssignUnderwriter");
                string message = "";
                //// Process the selected underwriters for each insurance request
                //var getInsurance = await _service.GetInsurancereq(requestId.ToString());
                //var getRequest = await _service.Getrequest(requestId.ToString());
                ////_service.GetInsuranceRequest(id);
                //if (getRequest == null)
                //{
                //    TempData["ResultMessage"] = "Error: Failed to assign underwriter.";

                //    return RedirectToAction("AssignUnderwriter"); // or return an error view

                //}
                ////if (getRequest.UserId == _globalVariables.userid) return RedirectToAction("Unauthorized"); //return Unauthorized();
                ////var underwriter = await _service.GetUnderwrite(selectedUnderwriter);
                ////if(underwriter.BrokerId != getRequest.BrokerId)
                ////{
                ////    TempData["ResultMessage"] = "Error: Failed to assign underwriter. because underwrite selected is not under broker";

                ////    // Handle the case where the selected Underwriter does not exist
                ////    return RedirectToAction("AssignUnderwriter");
                ////}
                ////if (underwriter == null)
                ////{
                ////    TempData["ResultMessage"] = "Error: Failed to assign underwriter.";

                ////    // Handle the case where the selected Underwriter does not exist
                ////    return RedirectToAction("AssignUnderwriter"); // or return an error view
                ////}
                //getRequest.UnderwriterId = selectedUnderwriter;

                //// Assign the underwriter for this insurance request
                //var assign = await _service.AssignUnderwriter(getInsurance, getRequest);
                //if (assign == "SuccessFul")
                //{
                //    TempData["ResultMessage"] = " Successfully assigned Underwriter";
                //}
                //else
                //{
                //    TempData["ResultMessage"] = "Error: Failed to assign underwriter.";
                //}


                //return RedirectToAction("AssignUnderwriter");

                // return View();
                // Process the selected underwriters for each insurance request
                var getInsurance = await _service.GetInsurancereq(requestId.ToString());
                var getRequest = await _service.Getrequest(requestId.ToString());
                //_service.GetInsuranceRequest(id);
                if (getInsurance.RequestByemail == _globalVariables.Email) return RedirectToAction("Unauthorized"); //return Unauthorized();

                var commentreq = new Comments()
                {
                    RequestID = requestId.ToString(),
                    CommentDate = DateTime.Now,
                    Comment = comment,
                    CommentBy = /*$"{_globalVariables.name}[*/ $"{_globalVariables.userName}",
                    Action = InsuranceStage.UnderwriterAssigned.ToString(),
                    Serial = getInsurance.Serial

                };
                if (getRequest == null)
                {
                    message = "Error: Failed to assign underwriter.";
                    TempData["ResultMessage"] = message;
                    return RedirectToAction(nameof(AssignUnderwriter), new { message = message });

                   // return RedirectToAction("AssignUnderwriter");

                }
                var underwriter = await _service.GetUnderwrite(selectedUnderwriter);
                if (underwriter == null)
                {
                    message = "Error: Failed to assign underwriter.";
                    TempData["ResultMessage"] = message;
                    return RedirectToAction(nameof(AssignUnderwriter), new { message = message });

                    //return RedirectToAction("AssignUnderwriter");
                }
                if (premium != 0)
                {
                    getRequest.UpdatedPremium = premium;
                }
                getRequest.UnderwriterId = selectedUnderwriter;
                var insertComment = _service.InsertComment(commentreq);
                if (approvalStatus == RequestStatus.Rejected.ToString())
                {

                    getRequest.Status = RequestStatus.Rejected.ToString();
                    string name = getInsurance.RequestByName;
                    string email = getInsurance.RequestByemail;
                    var update = _reqservice.UpdateRequest(getRequest, getInsurance, email, name, comment);
                    if (update.StartsWith("Success"))
                    {
                        message = "Message:" + update + " Rejected  Record";
                        TempData["ResultMessage"] = message;
                    }
                    else
                    {
                        message = "Error: Failed to Update the request.";
                        TempData["ResultMessage"] = message;
                    }
                }
                else if (approvalStatus == RequestStatus.Approved.ToString())
                {
                    getRequest.Status = RequestStatus.Approved.ToString();

                    // Assign the underwriter for this insurance request
                    var assign = await _reqservice.AssignUnderwriter(getInsurance, getRequest);
                    if (assign == "Successfully")
                    {
                        message = "Message: Successfully assigned Underwriter";
                        TempData["ResultMessage"] = message;
                    }
                    else
                    {
                        message = "Error: Failed to assign underwriter.";
                        TempData["ResultMessage"] = message;
                    }

                }

                return RedirectToAction(nameof(AssignUnderwriter), new { message = message });

                //return RedirectToAction("AssignUnderwriter");
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "AssignUnderwriter");
                throw;
            }
        }






        public async Task<IActionResult> Review(string message = null)
        {
            try
            {


                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.RIC))) return RedirectToAction("Unauthorized");

                // You can use Entity Framework or any other data access method
                var certificate = await _reqservice.GetInsuranceRequestsByStageAsync1(InsuranceStage.ReviewCertificate.ToString());
                _logging.LogInformation(certificate.Count().ToString(), "Review");


                if (certificate == null)
                {
                    return NotFound(); // Certificate not found
                }
                var records = new List<ReviewCertViewModel>();


                foreach (var item in certificate)
                {
                    var request = await _reqservice.GetRequestDetailsForInsuranceRequestsAsync(item.RequestID);

                    var viewModel = new ReviewCertViewModel
                    {
                        CertificateID = Convert.ToInt64(item.RequestID),
                        IssuanceDate = item.PolicyIssuanceDate,
                        ExpiryDate = item.PolicyExpiryDate,
                        PolicyNumber = item.PolicyNo,
                        EstimatedPremium = request.UpdatedPremium,
                        CustomerEmail = request.CustomerEmail,
                        AccountName = request.AccountName,
                        AccountNumber = request.AccountNo,
                        Broker = request.Broker.BrokerName,
                        CustomerName = request.CustomerName,
                        T24CustomerID = request.CustomerID,
                        InsuranceSubType = request.InsuranceSubType?.Name,
                        InsuranceType = request.InsuranceType.InsuranceType.Name,
                        Underwrite = request.Underwriter.Name,
                        Upload = "true"

                    };
                    records.Add(viewModel);
                }
                if (message != null)
                {
                    if (message.Contains("Error"))
                    {
                        ViewData["Error"] = message;
                    }
                    else
                    {
                        ViewData["Message"] = message;
                    }

                }
                return View(records);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "Review");
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReviewCertificateUpload(long CertificateID)
        {
            try
            {


                var getRequest = await _reqservice.GetRequestDetailsForInsuranceRequestsAsync(CertificateID.ToString());
                var getInsurance = await _service.GetInsurancereq(CertificateID.ToString());
                _logging.LogInformation($" Got record for {getRequest.RequestID}", "ReviewCertificateUpload");

                var record = new ReviewCertificateViewModel()
                {
                    EstimatedPremium = getRequest.UpdatedPremium,
                    ExpiryDate = Convert.ToDateTime(getInsurance.PolicyExpiryDate),
                    Broker = getRequest.Broker.BrokerName,
                    AccountName = getRequest.AccountName,
                    AccountNumber = getRequest.AccountNo,
                    CustomerEmail = getRequest.CustomerEmail,
                    CustomerID = getRequest.CustomerID,
                    CustomerName = getRequest.CustomerName,
                    PolicyNumber = getInsurance.PolicyNo,
                    Underwrite = getRequest.Underwriter.Name,
                    IssuanceDate = Convert.ToDateTime(getInsurance.PolicyIssuanceDate),
                    InsuranceSubType = getRequest.InsuranceSubType?.Name,
                    InsuranceType = getRequest.InsuranceType.InsuranceType.Name,
                    CertificateID = CertificateID,
                    PdfFileName = await _service.GetRelativePath(getInsurance.PolicyCertificate, getInsurance.ContentType, getInsurance.FileName),
                    Comment = "",
                    Status = ""

                };
                return View(record);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "ReviewCertificateUpload");
                throw;
            }
        }

        [HttpPost, ActionName("ReviewCertificateUpload")]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> ReviewCertificateUpload2(ReviewCertificateViewModel model)
        {
            try
            {


                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.RIC))) return RedirectToAction("Unauthorized");
                _logging.LogInformation($"Inside ReviewCertificateUpload2 for this request{model.CertificateID}", "Post ReviewCertificateUpload2");
                string message = "";
                if (ModelState.IsValid)
                {
                    // Handle form submit logic
                    var getRequest = await _service.GetInsurancereq(model.CertificateID.ToString());
                    //_service.GetInsuranceRequest(requestId);
                    // if (getRequest.RequestByemail == _globalVariables.Email) return RedirectToAction("Unauthorized"); //return Unauthorized();
                    getRequest.CertificateAuthorizedByEmail = _globalVariables.Email;
                    getRequest.CertificateAuthorizedByName = _globalVariables.name;
                    getRequest.CertificateAuthorizedByUsername = _globalVariables.userName;
                    getRequest.CertificateAuthorizedDate = DateTime.Now;
                    var commentreq = new Comments()
                    {
                        RequestID = model.CertificateID.ToString(),
                        CommentDate = DateTime.Now,
                        Comment = model.Comment,
                        CommentBy =/* $"{_globalVariables.name}[*/ $"{_globalVariables.userName}",
                        Action = InsuranceStage.ReviewCertificate.ToString(),
                        Serial = getRequest.Serial

                    };

                    //var updateComment = _service.InsertComment(commentreq);

                    //var getcertificate = await _reqservice.GetCertificate(Convert.ToInt32(model.CertificateID));

                    //// Process the review based on the model.Status (Approved/Rejected)
                    //getcertificate.InsuranceFlag = model.Status;
                    //getcertificate.Comment = model.Comment;
                    //getcertificate.DateModified = DateTime.Now;
                    //getcertificate.ApprovalUserId = _globalVariables.userid;

                    if (model.Status.ToString() == "Rejected")
                    {
                        var update = _reqservice.UpdateInsuranceReq(getRequest, model.Comment);
                        message = update + " Rejected  Record";
                        TempData["ResultMessage"] = message;
                    }
                    else if (model.Status.ToString() == "Approved")
                    {
                        var review = await _service.ReviewCertificateUploaded(getRequest);
                        message = review + " Approved  Record";
                        TempData["ResultMessage"] = message;
                    }
                }
                else
                {
                    message = "Invalid model data.";
                    TempData["ResultMessage"] = message;
                }
                return RedirectToAction(nameof(Review), new { message = message });

            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "ReviewCertificateUpload2");
                throw;
            }
        }

        public async Task<IActionResult> DownloadCertificate(long RequestId)
        {
            var getinsurance = await _service.GetInsurancereq(RequestId.ToString());
            byte[] fileBytes = Convert.FromBase64String(getinsurance.PolicyCertificate);


            return File(fileBytes, getinsurance.ContentType, getinsurance.FileName);
        }


        public async Task<ActionResult> Filter(string ContractId, string CustomerID, string CustomerName, string AccountNo, string InsuranceFlag, DateTime? PolicyExpiryStartDate, DateTime? PolicyExpiryEndDate, string Broker, string Underwriter)
        {
            var filteredRecords = await _reqservice.FilterReport(ContractId, CustomerID, CustomerName, AccountNo, InsuranceFlag,
                PolicyExpiryStartDate, PolicyExpiryEndDate, Broker, Underwriter);



            return View("Report", filteredRecords);
        }
        [HttpGet]
        public async Task<IActionResult> CreateRequest1()
        {
            try
            {


                if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.INR))) return RedirectToAction("Unauthorized");

                var model = new InsuranceRequestViewModel();
                var brokers = await _service.GetAllBroker();
                var insurance = await _service.GetAllBrokerInsuranceType();
                var insuranceTypes = await _service.GetAllbrokerSubInsuranceType();


                // Populate initial dropdowns
                model.Brokers = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
                model.InsuranceTypes = new SelectList(insurance.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Broker.BrokerName }), "Value", "Text");
                model.InsuranceSubTypes = new SelectList(insuranceTypes.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");

                return View(model);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "CreateRequest1");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInsuranceTypes(int brokerId)
        {
            var insuranceTypes = await _service.GetAllbrokerInsuranceTypebyId(brokerId);

            var insuranceTypeItems = insuranceTypes.Select(type => new SelectListItem
            {
                Value = type.Id.ToString(),
                Text = type.InsuranceType.Name
            });
            //  ViewBag.InsuranceTypeId = new SelectList(insuranceTypes.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");

            return Json(insuranceTypeItems);
        }

        [HttpGet]
        public async Task<IActionResult> GetInsuranceSubTypes(int insuranceTypeId)
        {

            var subTypes = await _service.GetAllBrokerInsuranceSubTypebyId(insuranceTypeId);




            var subTypeItems = subTypes.Select(subType => new SelectListItem
            {
                Value = subType.Id.ToString(),
                Text = subType.Name
            });
            //  ViewBag.InsuranceSubTypeId = new SelectList(subTypes.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");
            return Json(subTypeItems);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRequest1(InsuranceRequestViewModel model)
        {
            if (!_globalVariables.Permissions.Contains(GetPermissionName(Permissions.INR))) return RedirectToAction("Unauthorized");

            try
            {
                string message = "";
                var request = new Request()
                {
                    //InsuranceSubTypeID = model?.InsuranceSubTypeId,
                    //InsuranceTypeId = model.selectedInsuranceTypeId,
                    //BrokerID = model.SelectedBrokerId,
                    InsuranceSubTypeID = model?.InsuranceSubTypeId,
                    InsuranceTypeId = model.InsuranceTypeId,
                    BrokerID = model.BrokerId,
                    CustomerEmail = model.CustomerEmail,
                    CollateralValue = Convert.ToDecimal(model.CollateralValue),
                    CustomerName = model.CustomerName,
                    CustomerID = model.CustomerId,
                    AccountNo = model.AccountNumber,
                    AccountName = model.AccountName,
                    Premium = Convert.ToDecimal(model.EstimatedPremium),
                    UpdatedPremium = Convert.ToDecimal(model.EstimatedPremium),
                    Branchcode = model.Branchcode.Replace("|", "")
            };
                var isSuccess = await _reqservice.CreateRequest(request);

                if (isSuccess == "Successfully")
                {
                    message = "Request created successfully.";
                    TempData["ResultMessage"] = message;
                }
                else
                {
                    message = "Request creation failed.";
                    TempData["ResultMessage"] = message;
                    return RedirectToAction("CreateRequest1");

                }
                return RedirectToAction("Index", "Auth", new { message = message });

              //  return RedirectToAction("Index", "Auth");

            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "CreateRequest");
                return RedirectToAction("Index", "Auth");

            }

        }

    }
}