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
using static InsuranceCore.DTO.ReusableVariables;

namespace InsuranceManagement.Controllers
{
    [TypeFilter(typeof(AuditFilterAttribute))]
    public class UnderwritesController : Controller
    {
        private readonly ILogger<UnderwritesController> _logger;
        private readonly IInsuranceService _service;
        private readonly IRequestService _request;
       GlobalVariables globalVariables;
        private readonly GlobalVariables _globalVariables;
        private readonly ISessionService _session;
        public UnderwritesController(ILogger<UnderwritesController> logger, ISessionService session, IInsuranceService service, IRequestService request)
        {
            _service = service;
            _logger = logger;
            _request = request;
            _session = session;
            _globalVariables = _session.Get<GlobalVariables>("GlobalVariables");
        }
        // GET: Underwrites
        public async Task<ActionResult> Index(string message = null)
        {
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOU))) return RedirectToAction("Unauthorized", "Insurance");

            var getall = await _service.GetAllUnderwriter();
            if(message != null)
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
        // GET: Underwrites/Details/5
        public ActionResult DetailsUnderwriter(int id)
        {
            return View();
        }

        // GET: Underwrites/Create
        public async Task<ActionResult> CreateUnderwriter()
        {
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOU))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBroker();
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");

            return View();
        }

        // POST: Underwrites/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult CreateUnderwriter(Underwriter collection)
        {
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOU))) return RedirectToAction("Unauthorized", "Insurance");

                // TODO: Add insert logic here
                var add = _service.CreateUnderwriter(collection);
                TempData["ResultMessage"] = "Successfully Created Underwriter";
                ViewData["Message"] = "Successfully Created Underwriter";
                return RedirectToAction(nameof(Index), new { message = "Successfully Created Underwriter" });
            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = "Error creating Underwriter";
                ViewData["Error"] = "Error creating Underwriter";

                _logger.LogError(ex.ToString());
                return View();
            }
        }
        [EncryptionAction]
        // GET: Underwrites/Edit/5
        public async Task<ActionResult> EditUnderwriter(int id)
        {
            if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOU))) return RedirectToAction("Unauthorized", "Insurance");

            var brokers = await _service.GetAllBroker();
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
            var statusList = Enum.GetValues(typeof(BrokerStatus))
                       .Cast<BrokerStatus>()
                       .Select(e => new SelectListItem
                       {
                           Text = e.ToString(),
                           Value = ((int)e).ToString()
                       });

            // Assign the SelectList to a ViewBag property
            ViewBag.BrokerStatusList = new SelectList(statusList, "Value", "Text");
            var getunderwrite = await _request.GetUnderwritebyId(id);
            return View(getunderwrite);
        }

        // POST: Underwrites/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public ActionResult EditUnderwriter(Underwriter collection)
        {
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOU))) return RedirectToAction("Unauthorized", "Insurance");

                collection.Status = _request.GetEnumValueByIndex(Convert.ToInt32(collection.Status) + 1).ToString();

                // TODO: Add update logic here
                var update = _service.UpdateUnderwriter(collection);
                TempData["ResultMessage"] = "Successfully Edited Underwriter";
                ViewData["Message"]  = "Successfully Edited Underwriter";
                // return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Index), new { message = "Successfully Edited Underwriter" });
                }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString(), "EditUnderwriter");

                TempData["ResultMessage"] = "Error creating Underwriter";
                ViewData["Error"] = "Error Editing Underwriter";
                return View();
            }
        }

        [EncryptionAction]
        // GET: Underwrites/Delete/5
        public ActionResult DeleteUnderwriter(int id)
        {
            return View();
        }

        // POST: Underwrites/Delete/5
        [HttpPost, ActionName("DeleteUnderwriter")]
        [ValidateAntiForgeryToken]
        [ShowMessage("ResultMessage")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (!_globalVariables.Permissions.Contains(_request.GetPermissionName(Permissions.LOU))) return RedirectToAction("Unauthorized", "Insurance");

                var underwrite = await _service.GetUnderwrite(id);
                if (underwrite == null)
                {
                    return NotFound();
                }

                underwrite.Status = "Disable";
                _service.UpdateUnderwriter(underwrite);
                // Perform deletion logic here
                //_service.DeleteUnderwriter(id);
                TempData["ResultMessage"] = "Successfully Deleted Underwriter";
                ViewData["Message"] = "Successfully Deleted Underwriter";

                return RedirectToAction(nameof(Index), new { message = "Successfully Deleted Underwriter" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString(), "DeleteUnderwriter");
                ViewData["Error"] = "Error deleting Underwriter";

                TempData["ResultMessage"] = "Error deleting Underwriter";
                return View();
            }
        }
    }
}