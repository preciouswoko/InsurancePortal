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

namespace InsuranceManagement.Controllers
{

    public class HomeController : Controller
    {
        private readonly ISessionService _session;
        TemporaryVariables temporaryVariables;
        private readonly IUtilityService _utility;

        GlobalVariables globalVariables;
        private readonly GlobalVariables _globalVariables;
        private readonly TemporaryVariables _temporaryVariables;

        public HomeController(ISessionService session, IUtilityService utility)
        {
            _session = session;
            _globalVariables = _session.Get<GlobalVariables>("GlobalVariables");
            _temporaryVariables = _session.Get<TemporaryVariables>("TemporaryVariables");
            //  _globalVariables = _session.Get<GlobalVariables>("GlobalVariables");
            _utility = utility;
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
             var decrypt = _utility.Decrypt(encrypt,cancellationToken);

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
