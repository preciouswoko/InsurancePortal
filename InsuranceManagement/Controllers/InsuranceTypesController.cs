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
    public class InsuranceTypesController : Controller
    {
        private readonly ILogger<InsuranceTypesController> _logger;
        private readonly IRequestService _request;
        private readonly IInsuranceService _service;
        private readonly ILoggingService _logging;
        //GlobalVariables globalVariables;
        //private readonly GlobalVariables _globalVariables;
        private readonly ISessionService _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string generalVariable;
        public InsuranceTypesController(IInsuranceService service, IHttpContextAccessor httpContextAccessor, ISessionService session, ILoggingService logging, ILogger<InsuranceTypesController> logger, IRequestService request)
        {
            _logging = logging;
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
            //}           // _globalVariables = _session.Get<GlobalVariables>("GlobalVariables");
        }
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
        // GET: InsuranceType
        public async Task<ActionResult> Index(string message = null)
        {
            var getall = await _service.GetAllInsuranceType();
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

        // GET: InsuranceType/Details/5
        public ActionResult DetailsInsurance(int id)
        {
            return View();
        }

        // GET: InsuranceType/Create
        public async Task<ActionResult> CreateInsurance()
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOI))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBroker();
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");

            return View();
        }

        // POST: InsuranceType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult CreateInsurance(InsuranceType collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOI))) return RedirectToAction("Unauthorized", "Insurance");

                // TODO: Add insert logic here
                var add = _service.CreateInsuranceType(collection);
                TempData["ResultMessage"] = " Successfully Created InsuranceType";
                return RedirectToAction(nameof(Index), new { message = " Successfully Created InsuranceType" });
            }
            catch
            {
                TempData["ResultMessage"] = "Error Creating InsuranceType";
                return View();
            }
        }
        [EncryptionAction]

        // GET: InsuranceType/Edit/5
        public async Task<ActionResult> EditInsurance(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOI))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBroker();
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
            var getInsuranctync = await _request.GetInsuranceTypebyId(id);
            var statusList = Enum.GetValues(typeof(BrokerStatus))
                       .Cast<BrokerStatus>()
                       .Select(e => new SelectListItem
                       {
                           Text = e.ToString(),
                           Value = ((int)e).ToString()
                       });

            // Assign the SelectList to a ViewBag property
            ViewBag.BrokerStatusList = new SelectList(statusList, "Value", "Text");

            return View(getInsuranctync);
        }

        // POST: InsuranceType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult EditInsurance( InsuranceType collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOI))) return RedirectToAction("Unauthorized", "Insurance");

                collection.Status = _request.GetEnumValueByIndex(Convert.ToInt32(collection.Status) + 1).ToString();

                // TODO: Add update logic here
                var update = _service.UpdateInsuranceType(collection);
                TempData["ResultMessage"] = "Successfully Updated InsuranceType";
                return RedirectToAction(nameof(Index), new { message = " Successfully Updated InsuranceType" });
            }
            catch
            {
                TempData["ResultMessage"] = "Error updating InsuranceType";
                return View();
            }
        }
        [EncryptionAction]

        // GET: InsuranceType/Delete/5
        public ActionResult DeleteInsurance(int id)
        {
            return View();
        }

        // POST: InsuranceType/Delete/5
        [HttpPost, ActionName("DeleteInsurance")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOI))) return RedirectToAction("Unauthorized", "Insurance");

                var Insurancetype = await _service.GetInsuranceType(id);
                if (Insurancetype == null)
                {
                    return NotFound();
                }
                Insurancetype.Status = "Disable";
                _service.UpdateInsuranceType(Insurancetype);
                // Perform deletion logic here
                //_service.DeleteInsurance(id);
                TempData["ResultMessage"] = "Successfully Deleted InsuranceType";
                return RedirectToAction(nameof(Index), new { message = " Successfully Deleted InsuranceType" });
            }
            catch
            {
                TempData["ResultMessage"] = "Error deleting InsuranceType";
                return View();
            }
        }

        public async Task<ActionResult> CreateInsuranceSubType()
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LIS))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBroker();
            var  getall = await _service.GetAllInsuranceType();
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
            ViewBag.InsuranceTypeId = new SelectList(getall.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");
            ViewBag.StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Active", Text = "Active" },
                new SelectListItem { Value = "Inactive", Text = "Inactive" }

            };
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public async Task< ActionResult> CreateInsuranceSubType(InsuranceSubType collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LIS))) return RedirectToAction("Unauthorized", "Insurance");

                // TODO: Add insert logic here
                var add = await _service.CreateInsuranceSubType(collection);
                TempData["ResultMessage"] = "Successfully Created InsuranceSubType";
                return RedirectToAction(nameof(InsuranceSubTypeIndex), new { message = " Successfully Created InsuranceSubType" });

            }
            catch
            {
                TempData["ResultMessage"] = "Error creating InsuranceSubType";
                return View();
            }
        }

        public async Task<ActionResult> InsuranceSubTypeIndex(string message = null)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LIS))) return RedirectToAction("Unauthorized", "Insurance");

            var getall = await _service.GetAllSubInsuranceType();
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
        public async Task<ActionResult> EditInsuranceSubType(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LIS))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBroker();
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
            var getall = await _service.GetAllInsuranceType();
            ViewBag.InsuranceTypeId = new SelectList(getall.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");
            var statusList = Enum.GetValues(typeof(BrokerStatus))
                       .Cast<BrokerStatus>()
                       .Select(e => new SelectListItem
                       {
                           Text = e.ToString(),
                           Value = ((int)e).ToString()
                       });

            // Assign the SelectList to a ViewBag property
            ViewBag.BrokerStatusList = new SelectList(statusList, "Value", "Text");
            var getInsuranctync = await _service.GetInsuranceSubTypebyId(id);
            return View(getInsuranctync);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult EditInsuranceSubType(InsuranceSubType collection)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LIS))) return RedirectToAction("Unauthorized", "Insurance");

                collection.Status = _request.GetEnumValueByIndex(Convert.ToInt32(collection.Status) + 1).ToString();

                // TODO: Add update logic here
                var update = _service.UpdateInsuranceSubType(collection);
                TempData["ResultMessage"] = "Successfully Edited InsuranceSubType";
               // return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(InsuranceSubTypeIndex), new { message = " Successfully Edited InsuranceSubType" });

            }
            catch
            {
                TempData["ResultMessage"] = "Error Editing InsuranceSubType";
                return View();
            }
        }
        // GET: InsuranceType/Delete/5
        public ActionResult DeleteInsuranceSub(int id)
        {
            return View();
        }

        // POST: InsuranceType/Delete/5
        [HttpPost, ActionName("DeleteInsuranceSub")]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> DeleteConfirmedForType(int id)
        {
            GlobalVariables _globalVariables = GetGlobalVariables();
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LIS))) return RedirectToAction("Unauthorized", "Insurance");

                var InsuranceSubtype = await _service.GetInsuranceSubTypebyId(id);
                if (InsuranceSubtype == null)
                {
                    return NotFound();
                }
                InsuranceSubtype.Status = "Disable";
                _service.UpdateInsuranceSubType(InsuranceSubtype);
                // Perform deletion logic here
                //_service.DeleteInsuranceSub(id);
                TempData["ResultMessage"] = "Successfully Deleted InsuranceSubType";
                //return RedirectToAction("Index");
                return RedirectToAction(nameof(InsuranceSubTypeIndex), new { message = " Successfully Deleted InsuranceSubType" });

            }
            catch
            {
                TempData["ResultMessage"] = "Error deleting InsuranceSubType";
                return View();
            }
        }
    }
}