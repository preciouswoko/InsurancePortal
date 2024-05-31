using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsuranceCore.Enums;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Helpers;
using InsuranceInfrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static InsuranceCore.DTO.ReusableVariables;

namespace InsuranceManagement.Controllers
{
    [TypeFilter(typeof(AuditFilterAttribute))]
    public class BrokerController : Controller
    {
        private readonly ILogger<BrokerController> _logger;
        private readonly IInsuranceService _service;
        private readonly IRequestService _request;
        //GlobalVariables globalVariables;
        //private readonly GlobalVariables _globalVariables;
        private readonly ISessionService _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string generalVariable;
        public BrokerController(ILogger<BrokerController> logger, ISessionService session, IInsuranceService service, IRequestService request,
             IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _logger = logger;
            _request = request;
            _session = session;
            _httpContextAccessor = httpContextAccessor;
            //generalVariable = _httpContextAccessor.HttpContext.Session.GetString("GlobalVariables");
            //if (generalVariable != null)
            //{
            //    _globalVariables = JsonConvert.DeserializeObject<GlobalVariables>(generalVariable) ?? new GlobalVariables();
            //}
            //else
            //{
            //    // Handle the case where the JSON string is null
            //    _globalVariables = new GlobalVariables();
            //}            // _globalVariables = _session.Get<GlobalVariables>("GlobalVariables");
        }
        // GET:
        // GET: Broker
        public GlobalVariables GetGlobalVariables()
        {
            try
            {
                string generalVariable = _httpContextAccessor.HttpContext.Session.GetString("GlobalVariables");

                if (!string.IsNullOrEmpty(generalVariable))
                {
                    return JsonConvert.DeserializeObject<GlobalVariables>(generalVariable) ?? new GlobalVariables();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.ToString(), "SearchWithParameters");
            }
            return new GlobalVariables();


        }
        public async Task<ActionResult> Index(string message = null)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();

            var getall = await _service.GetAllBroker();
          //  _logger.LogInformation($"{_globalVariables.name} requested for {JsonConvert.SerializeObject(getall)}", "Index");
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
            return View(getall);
        }
        [HttpPost("CreateOtherBroker")]
        public async Task<IActionResult> CreateOtherBroker(string accountNumber)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();

         //   _logger.LogInformation($"User{_globalVariables.name}Enter CreateOtherBroker at{DateTime.Now}");
         

            Broker broker = await FetchBrokerData(accountNumber);
           // _logger.LogInformation($"{_globalVariables.name} fetch broker {JsonConvert.SerializeObject(broker)}", "CreateOtherBroker");

            return View(broker);
        }

        private async Task<Broker> FetchBrokerData(string accountNumber)
        {
            // Simulated data for demonstration purposes
            var getT24Details = await _service.FetchDetail(accountNumber);

            return new Broker
            {
                AccountNumber = accountNumber,
                AccountName = getT24Details.AccountName,
                CustomerID = getT24Details.T24CustomerID,
                EmailAddress = getT24Details.CustomerEmail,
                BrokerName = getT24Details.CustomerName,

            };
        }
        [EncryptionAction]
        // GET: Broker/Details/5
        public ActionResult DetailsBroker(int id)
        {
            return View();
        }

        // GET: Broker/Create
        [HttpGet]
        public ActionResult CreateBroker()
        {
            return View();
        }

        // POST: Broker/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult CreateBroker(Broker collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
         //   _logger.LogInformation($"User{_globalVariables.name}Enter CreateBroker at{DateTime.Now}");

            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOB))) return RedirectToAction("Unauthorized", "Insurance");
                _logger.LogInformation($"{_globalVariables.name} created broker {JsonConvert.SerializeObject(collection)} ", "CreateBroker");

                // TODO: Add insert logic here
                var add = _service.CreateBroker(collection);
                //   _logger.LogInformation($"{_globalVariables.name} created broker with {JsonConvert.SerializeObject(collection)} : result {add}", "CreateBroker");
                if (add.StartsWith("Success"))
                {
                    TempData["ResultMessage"] = " Successfully Created Broker ";
                    return RedirectToAction(nameof(Index), new { message = "Successfully Created Broker " });

                }
                return RedirectToAction(nameof(Index), new { message = add });

            }
            catch
            {
                TempData["ResultMessage"] = "Error Creating Broker";
                return View();
            }
        }
        [EncryptionAction]
        // GET: Broker/Edit/5
        public async Task<ActionResult> EditBroker(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOB))) return RedirectToAction("Unauthorized", "Insurance");
          //  _logger.LogInformation($"User{_globalVariables.name} Edited Broker at{DateTime.Now}");

            var statusList = Enum.GetValues(typeof(BrokerStatus))
                        .Cast<BrokerStatus>()
                        .Select(e => new SelectListItem
                        {
                            Text = e.ToString(),
                            Value = ((int)e).ToString()
                        });

            // Assign the SelectList to a ViewBag property
            ViewBag.BrokerStatusList = new SelectList(statusList, "Value", "Text");
            var broker = await _service.GetBroker(id);
          //  _logger.LogInformation($"{_globalVariables.name} requested for {id} result: {JsonConvert.SerializeObject(broker)}", "EditBroker");

            return View(broker);
        }

        // POST: Broker/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult EditBroker( Broker collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOB))) return RedirectToAction("Unauthorized", "Insurance");
                //   _logger.LogInformation($"User{_globalVariables.name}Enter EditBroker at{DateTime.Now}");
                _logger.LogInformation($"{_globalVariables.name} edited broker {JsonConvert.SerializeObject(collection)} ", "EditBroker");

                // TODO: Add update logic here
                collection.Status = _request.GetEnumValueByIndex(Convert.ToInt32(collection.Status )+ 1).ToString();
                var update = _service.UpdateBroker(collection);
              //  _logger.LogInformation($"{_globalVariables.name} updated {JsonConvert.SerializeObject(collection)} : result= {update}", "EditBroker");

                TempData["ResultMessage"] = "Successfully Updated Broker";
                return RedirectToAction(nameof(Index), new { message = "Successfully Updated Broker " });
            }
            catch
            {
                TempData["ResultMessage"] = "Error updating broker";
                return View();
            }
        }

        // GET: Broker/Delete/5
        public ActionResult DeleteBroker(int id)
        {
            return View();
        }

        // POST: Broker/Delete/5
        [HttpPost, ActionName("DeleteBroker")]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOB))) return RedirectToAction("Unauthorized", "Insurance");
              //  _logger.LogInformation($"User{_globalVariables.name}Enter DeleteConfirmed at{DateTime.Now}");

                var broker = await _service.GetBroker(id);
                if (broker == null)
                {
                    return NotFound();
                }
                broker.Status = "Disable";
                _service.UpdateBroker(broker);
               // _service.DeleteBroker(id);
                TempData["ResultMessage"] = "Successfully Deleted Broker";
                return RedirectToAction(nameof(Index), new { message = "Successfully Deleted Broker " });
            }
            catch
            {
                TempData["ResultMessage"] = "Error deleting broker";
                return View();
            }
        }


        public async Task<ActionResult> BrokerInsuranceIndex()
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            _logger.LogInformation($"User: {_globalVariables.name} Enter BrokerInsuranceIndex at{DateTime.Now}");

            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBI))) return RedirectToAction("Unauthorized", "Insurance");

            var getall = await _service.GetAllBrokerInsuranceType();
            //_logger.LogInformation($"{_globalVariables.name} requested for broker insurance result: {JsonConvert.SerializeObject(getall)}", "BrokerInsuranceIndex");

            return View(getall);
        }
        [EncryptionAction]

        // GET: InsuranceType/Details/5
        public ActionResult DetailsBrokerInsurance(int id)
        {
            return View();
        }

        // GET: InsuranceType/Create
        public async Task<ActionResult> CreateBrokerInsuranceType()
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBI))) return RedirectToAction("Unauthorized", "Insurance");
           // _logger.LogInformation($"User{_globalVariables.name}Enter CreateBrokerInsuranceType at{DateTime.Now}");

            var brokers = await _service.GetAllBroker();
            var InsuranceType = await _service.GetAllInsuranceType();
            ViewBag.InsuranceTypeId = new SelectList(InsuranceType.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");

            return View();
        }

        // POST: InsuranceType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult CreateBrokerInsuranceType(BrokerInsuranceType collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBI))) return RedirectToAction("Unauthorized", "Insurance");
                //  _logger.LogInformation($"User{_globalVariables.name}Enter CreateBrokerInsuranceType at{DateTime.Now}");
                _logger.LogInformation($"{_globalVariables.name} created broker Insurance {JsonConvert.SerializeObject(collection)} ", "CreateBrokerInsuranceType");

                // TODO: Add insert logic here
                var add = _service.CreateBrokerInsuranceType(collection);
             //   _logger.LogInformation($"{_globalVariables.name} created broker insurance type with: {JsonConvert.SerializeObject(collection)} : result {add}", "CreateBrokerInsuranceType");

                TempData["ResultMessage"] = " Successfully Created Broker InsuranceType";
                return RedirectToAction(nameof(BrokerInsuranceIndex), new { message = "Successfully Created Broker InsuranceType " });

                //return RedirectToAction(nameof(BrokerInsuranceIndex));
            }
            catch
            {
                TempData["ResultMessage"] = "Error Creating Broker InsuranceType";
                return View();
            }
        }
        [EncryptionAction]

        // GET: InsuranceType/Edit/5
        public async Task<ActionResult> EditBrokerInsuranceType(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBI))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBroker();
            var InsuranceType = await _service.GetAllInsuranceType();
            ViewBag.InsuranceTypeId = new SelectList(InsuranceType.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");

            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
            var getInsuranctype = await _request.GetBrokerInsuranceTypebyId(id);
            var statusList = Enum.GetValues(typeof(BrokerStatus))
                       .Cast<BrokerStatus>()
                       .Select(e => new SelectListItem
                       {
                           Text = e.ToString(),
                           Value = ((int)e).ToString()
                       });

            // Assign the SelectList to a ViewBag property
            ViewBag.BrokerStatusList = new SelectList(statusList, "Value", "Text");

            return View(getInsuranctype);
        }

        // POST: InsuranceType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult EditBrokerInsuranceType(BrokerInsuranceType collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBI))) return RedirectToAction("Unauthorized", "Insurance");

                // collection.Status = _request.GetEnumValueByIndex(Convert.ToInt32(collection.Status) + 1).ToString();
                _logger.LogInformation($"{_globalVariables.name} edited broker Insurance {JsonConvert.SerializeObject(collection)} ", "EditBrokerInsuranceType");

                // TODO: Add update logic here
                var update = _service.UpdateBrokerInsuranceType(collection);
              //  _logger.LogInformation($"{_globalVariables.name} updated broker insurance with: {JsonConvert.SerializeObject(collection)} : result {update}", "EditBrokerInsuranceType");

                TempData["ResultMessage"] = "Successfully Updated Broker InsuranceType";
                return RedirectToAction(nameof(BrokerInsuranceIndex), new { message = "Successfully Updated Broker InsuranceType " });

            }
            catch
            {
                TempData["ResultMessage"] = "Error updating Broker InsuranceType";
                return View();
            }
        }
        [EncryptionAction]

        // GET: InsuranceType/Delete/5
        public ActionResult DeleteBrokerInsuranceType(int id)
        {
            return View();
        }

        // POST: InsuranceType/Delete/5
        [HttpPost, ActionName("DeleteBrokerInsuranceType")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedForinsurance(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBI))) return RedirectToAction("Unauthorized", "Insurance");

                var Insurancetype = await _service.GetBrokerInsuranceType(id);
                if (Insurancetype == null)
                {
                    return NotFound();
                }
                Insurancetype.Status = "Disable";
                _service.UpdateBrokerInsuranceType(Insurancetype);
                // Perform deletion logic here
                //_service.DeleteInsurance(id);
                TempData["ResultMessage"] = "Successfully Deleted Broker InsuranceType";
                return RedirectToAction(nameof(BrokerInsuranceIndex), new { message = "Successfully Deleted Broker InsuranceType " });

            }
            catch
            {
                TempData["ResultMessage"] = "Error deleting Broker InsuranceType";
                return View();
            }
        }

        public async Task<ActionResult> CreateBrokerInsuranceSubType(string message = null)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBS))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBroker();
            var getall = await _service.GetAllInsuranceType();
            var getallbrokersub = await _service.GetAllBrokerInsuranceSubType();
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
            ViewBag.BrokerInsuranceTypeId = new SelectList(getall.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");
            ViewBag.BrokerInsuranceSubTypeId = new SelectList(getallbrokersub.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");

            ViewBag.StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Active", Text = "Active" },
                new SelectListItem { Value = "Inactive", Text = "Inactive" }

            };
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
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public async Task<ActionResult> CreateBrokerInsuranceSubType(BrokerSubInsuranceType collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBS))) return RedirectToAction("Unauthorized", "Insurance");
                _logger.LogInformation($"{_globalVariables.name} created broker Insurance sub type{JsonConvert.SerializeObject(collection)} ", "CreateBrokerInsuranceSubType");

                // TODO: Add insert logic here
                var getname = await _service.GetInsuranceSubTypebyId(Convert.ToInt32(collection.Name));
                //  var getname = await _service.GetBrokerInsuranceSubTypebyId(Convert.ToInt32(collection.Name));
                collection.Name = getname.Name;
                var add = await _service.CreateBrokerInsuranceSubType(collection);
                //  _logger.LogInformation($"{_globalVariables.name} created broker insurance sub Type with: {JsonConvert.SerializeObject(collection)} : result {add}", "CreateBrokerInsuranceSubType");
                if (add.StartsWith("Success"))
                {
                    TempData["ResultMessage"] = "Successfully Created Broker InsuranceSubType";
                    return RedirectToAction(nameof(BrokerInsuranceSubTypeIndex), new { message = "Successfully Created Broker InsuranceSubType" });

                }
                return RedirectToAction(nameof(CreateBrokerInsuranceSubType), new { message = add });
             

            }
            catch
            {
                TempData["ResultMessage"] = "Error creating Broker InsuranceSubType";
                return View();
            }
        }

        public async Task<ActionResult> BrokerInsuranceSubTypeIndex(string message = null)
        {

            var getall = await _service.GetAllBrokerInsuranceSubType();
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
            return View(getall);
        }
        [EncryptionAction]
        public async Task<ActionResult> EditBrokerInsuranceSubType(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBS))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBrokerInsuranceType();
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Broker.BrokerName }), "Value", "Text");
            var getall = await _service.GetAllbrokerSubInsuranceType();
            ViewBag.BrokerInsuranceTypeId = new SelectList(getall.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.InsuranceType.InsuranceType.Name }), "Value", "Text"); /*{ Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");*/
            var statusList = Enum.GetValues(typeof(BrokerStatus))
                       .Cast<BrokerStatus>()
                       .Select(e => new SelectListItem
                       {
                           Text = e.ToString(),
                           Value = ((int)e).ToString()
                       });

            // Assign the SelectList to a ViewBag property
            ViewBag.BrokerStatusList = new SelectList(statusList, "Value", "Text");
            var getInsuranctype = await _service.GetBrokerInsuranceSubTypebyId(id);
            return View(getInsuranctype);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult EditBrokerInsuranceSubType(BrokerSubInsuranceType collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBS))) return RedirectToAction("Unauthorized", "Insurance");

                //collection.Status = _request.GetEnumValueByIndex(Convert.ToInt32(collection.Status) + 1).ToString();
                _logger.LogInformation($"{_globalVariables.name} edited broker Insurance sub type{JsonConvert.SerializeObject(collection)} ", "EditBrokerInsuranceSubType");

                // TODO: Add update logic here
                var update = _service.UpdateBrokerInsuranceSubType(collection);
             //   _logger.LogInformation($"{_globalVariables.name} updated broker insurance sub type with: {JsonConvert.SerializeObject(collection)} : result {update}", "EditBrokerInsuranceSubType");

                TempData["ResultMessage"] = "Successfully Edited Broker InsuranceSubType";
                return RedirectToAction(nameof(BrokerInsuranceSubTypeIndex), new { message = "Successfully Edited Broker InsuranceSubType" });
            }
            catch
            {
                TempData["ResultMessage"] = "Error Editing Broker InsuranceSubType";
                return View();
            }
        }
        // GET: InsuranceType/Delete/5
        public ActionResult DeleteBrokerInsuranceSub(int id)
        {
            return View();
        }

        // POST: InsuranceType/Delete/5
        [HttpPost, ActionName("DeleteBrokerInsuranceSub")]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> DeleteConfirmedForBrokerType(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LBS))) return RedirectToAction("Unauthorized", "Insurance");

                var InsuranceSubtype = await _service.GetBrokerInsuranceSubTypebyId(id);
               // _logger.LogInformation($"{_globalVariables.name} deleted broker type {JsonConvert.SerializeObject(InsuranceSubtype)}", "DeleteConfirmedForBrokerType");

                if (InsuranceSubtype == null)
                {
                    return NotFound();
                }
                InsuranceSubtype.Status = "Disable";
                _service.UpdateBrokerInsuranceSubType(InsuranceSubtype);
                // Perform deletion logic here
                //_service.DeleteInsuranceSub(id);
                TempData["ResultMessage"] = "Successfully Deleted Broker InsuranceSubType";
                return RedirectToAction(nameof(BrokerInsuranceSubTypeIndex), new { message = "Successfully Deleted Broker InsuranceSubType" });
            }
            catch
            {
                TempData["ResultMessage"] = "Error deleting Broker InsuranceSubType";
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetInsuranceSubTypes(int insuranceTypeId)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();


            var subTypes = await _service.InsuranceSubTypebyId(insuranceTypeId);

          //  _logger.LogInformation($"{_globalVariables.name} requested for insurance sub type : result {subTypes}", "GetInsuranceSubTypes");



            var subTypeItems = subTypes.Select(subType => new SelectListItem
            {
                Value = subType.Id.ToString(),
                Text = subType.Name
            });
            //  ViewBag.InsuranceSubTypeId = new SelectList(subTypes.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");
            return Json(subTypeItems);
        }

    }
}