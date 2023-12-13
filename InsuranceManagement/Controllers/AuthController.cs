using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsuranceCore.DTO;
using InsuranceCore.Enums;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceManagement.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InsuranceManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAumsService _authService;
        private readonly ILoggingService _logging;
       // private readonly IUserService _userService;
        private readonly IInsuranceService _service;
        public AuthController(IAumsService authService, ILoggingService logging,
           /* IUserService userService,*/ IInsuranceService service
            )
        {
            _logging = logging;
            _authService = authService;
           // _userService = userService;
            _service = service;
        }
        public IActionResult Index(string message = null)
        {
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
        [HttpGet]
        public  IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login( LoginRequestModel loginRequest)
        {
            try
            {
                _logging.LogInformation($"Inside LoginUser  at {DateTime.Now}", "LoginUser");
                if (!ModelState.IsValid)
                {
                    var responseModel = new LoginResponseModel
                    {
                        AccessToken = null,
                        Message = "Invalid request data."
                    };
                    ViewBag.Message = "Invalid request data.";
                    _logging.LogInformation(responseModel.ToString(), "Login");

                    _logging.LogInformation($"Outside LoginUser  at {DateTime.Now}", "LoginUser");

                    return View("Login");
                }


                // Call the authentication service to validate the user
                var accessToken = await _authService.AuthenticateUser(
                    loginRequest.Username,
                    loginRequest.Password,
                    loginRequest.AccessToken,
                    HttpContext.RequestAborted
                );

                if (accessToken != null)
                {
                    // Authentication successful, return the access token or user data
                    var responseModel = new LoginResponseModel
                    {
                        AccessToken = accessToken.username,
                        Message = "Authentication successful."
                    };
                    _logging.LogInformation(responseModel.ToString(), "Login");
                    ViewBag.Message = "Authentication successful.";

                    //return Ok(responseModel);
                    _logging.LogInformation($"Outside LoginUser  at {DateTime.Now}", "LoginUser");

                    return RedirectToAction("Index", "Auth");
                   // return RedirectToAction("Profiling");
                }
                else
                {
                    // Authentication failed
                    var responseModel = new LoginResponseModel
                    {
                        AccessToken = null,
                        Message = "Invalid credentials."
                    };
                    ViewBag.Message = "Invalid credentials.";
                    _logging.LogInformation(responseModel.ToString(), "Login");

                    _logging.LogInformation($"Outside LoginUser  at {DateTime.Now}", "LoginUser");

                    return View("Login");
                }

            }
            catch (Exception ex)
            {
                _logging.LogError($"Exception{ex.ToString()} at {DateTime.Now}", "LoginUser");
                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login"); 
        }
        [HttpGet]
        public async Task<IActionResult> CreateExternalUser()
        {
            var brokers = await _service.GetAllBroker();
           // var insurance = await _service.GetAllInsuranceType();

            if (brokers == null /*|| insurance == null*/)
            {
                // Handle the case where one or both collections are null
                // You may want to return an error view or handle it differently
                return RedirectToAction("Error");
            }

            // Create a SelectList for the dropdowns
            ViewBag.BrokerId = new SelectList(brokers.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.BrokerName }), "Value", "Text");
            //ViewBag.InsuranceTypeId = new SelectList(insurance.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }), "Value", "Text");

            // Rest of your code...

            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateExternalUser(CreateCustomer model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid) return BadRequest();
        //        var customer = await _userService.CreateExternalUser(model);
        //        return Ok(customer);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.Message, "CreateExternalUser Controller");
        //        return StatusCode(500, "An error occurred while creating the external user.");
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> ManageExternalUser(string username, UserStatus action)
        //{
        //    try
        //    {
        //        await _userService.ManageExternalUser(username, action);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.Message, "ManageExternalUser Controller");

        //        return StatusCode(500, "An error occurred while managing the external user.");
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAllCustomers()
        //{
        //    try
        //    {
        //        var customers = await _userService.GetAllCustomer();
        //        return Ok(customers);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.Message, "GetAllCustomers Controller");
        //        return StatusCode(500, "An error occurred while fetching customers.");
        //    }
        //}

        //[HttpPost]
        //public IActionResult UpdateCustomer(Customer request)
        //{
        //    try
        //    {
        //        var result = _userService.UpdateCustomer(request);
        //        if (result == "Successful")
        //        {
        //            return Ok("Customer updated successfully.");
        //        }
        //        else
        //        {
        //            return BadRequest("Customer update failed.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.Message, "UpdateCustomer Controller");
        //        return StatusCode(500, "An error occurred while updating the customer.");
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> UpdateCustomer(int Id)
        //{
        //    try
        //    {
        //        var getcustomer = await _userService.GetCustomerAsync(Id);
        //        return View(getcustomer);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.Message, "UpdateCustomer Controller");
        //        return StatusCode(500, "An error occurred while updating the customer.");
        //    }
        //}

        //not correct
        [HttpPost]
        public async Task<IActionResult> ForgetPassword()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
        public ActionResult Profiling()
        {
            
                return View();
           

        }
        [HttpGet]
        public IActionResult LoginExternalUser()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async  Task<IActionResult> LoginExternalUser(LoginViewModel model)
        //{
        //    if (!ModelState.IsValid) return BadRequest();
        //    var customer =  await _userService.GetByUsername(model.Username);
        //    if (customer == null || ( customer.Password == _userService.hash(model.Password)))
        //    {
        //        return BadRequest();
        //    }
        //    return RedirectToAction("GetInsuranceDetails", "Insurance", customer.InsuranceRequestId);

        //}
    }
}