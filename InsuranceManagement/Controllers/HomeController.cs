using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InsuranceManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using InsuranceCore.Interfaces;
using static InsuranceCore.DTO.ReusableVariables;
using System.Threading;
using InsuranceInfrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using InsuranceInfrastructure.Services;

namespace InsuranceManagement.Controllers
{

    public class HomeController : Controller
    {
        private readonly ISessionService _session;
        TemporaryVariables temporaryVariables;
        private readonly IUtilityService _utility;

        //GlobalVariables globalVariables;
        //private readonly GlobalVariables _globalVariables;
        private readonly TemporaryVariables _temporaryVariables;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string generalVariable;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ISessionService session, ILogger<HomeController> logger, IUtilityService utility, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _logger = logger;

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
            // _temporaryVariables = _session.Get<TemporaryVariables>("TemporaryVariables");
            //  _globalVariables = _session.Get<GlobalVariables>("GlobalVariables");
            _utility = utility;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ShowMessage("ResultMessage")]
        public IActionResult Encrypt(string value)
        {
            var cancellationTokenSource = new CancellationToken();
            CancellationToken cancellationToken = cancellationTokenSource;
            var encrypt = _utility.Encrypt(value, cancellationToken);
             var decrypt =  _utility.Decrypt(encrypt,cancellationToken);

            TempData["ResultMessage"] = encrypt;

            return RedirectToAction("Encrypt");
        }

        public IActionResult Encrypt()
        {
            // Retrieve the TempData value
            string resultMessage = TempData["ResultMessage"] as string;

            // Clear TempData to prevent it from persisting across requests
            TempData.Remove("ResultMessage");

            // Pass the result message to the view
            ViewData["ResultMessage"] = resultMessage;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        [HttpGet]
        public IActionResult LogOff()
        {
            //  _httpContextAccessor.HttpContext.Session.Remove("GlobalVariables");
            var existingData = GetGlobalVariables();

            if (existingData != null)
            {
             string   userId = existingData.userid;
              //  UserSessionManager.RemoveUserSession(userId);

            }


            HttpContext.Session.Clear();
            _session.Clear("GlobalVariables");
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public class ListItem
        {
            public long Value { get; set; }
            public string Text { get; set; }
        }
       // [HttpGet]
       [HttpPost]
        public ActionResult KeepAlive()
        {
            List<ListItem> loan1 = new List<ListItem>();
            ListItem ln2;
            ln2 = new ListItem() { Value = 0, Text = "" };
            loan1.Add(ln2);
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new SelectList(
                        loan1.ToArray(),
                        "Value",
                        "Text"));

            return View();
        }

        [Route("Home/Error/{statusCode}")]
    public IActionResult Error(int? statusCode)
    {
        var viewModel = new ErrorViewModel();

        if (statusCode.HasValue)
        {
            viewModel.StatusCode = statusCode.Value;
        }

        return View(viewModel);
    }
    }
}
