using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using static InsuranceCore.DTO.ReusableVariables;
using Microsoft.AspNetCore.Http;

namespace InsuranceInfrastructure.Helpers
{
    public class AuditFilterAttribute : Attribute, IActionFilter
    {
        private readonly ApplicationDbContext _db;
        private readonly IUtilityService _utils;
        private readonly ISessionService _service;
        private readonly ILoggingService _logging;
        private readonly GlobalVariables _globalVariables;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string generalVariable;
        public AuditFilterAttribute(ApplicationDbContext db, IUtilityService utils, ISessionService service, ILoggingService logging, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _utils = utils;
            _service = service;
            _logging = logging;
           // _globalVariables = _service.Get<GlobalVariables>("GlobalVariables");
            _httpContextAccessor = httpContextAccessor;
            generalVariable = _httpContextAccessor.HttpContext.Session.GetString("GlobalVariables");
            _globalVariables = JsonConvert.DeserializeObject<GlobalVariables>(generalVariable);

        }

        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string param = "";
            // Generate auditStatusId
            Guid auditStatusId = Guid.NewGuid();

            // Store in session
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("auditStatusId", auditStatusId.ToString());
           // _service.Set("auditStatusId", auditStatusId);
            var request = filterContext.HttpContext.Request;
            string controller = (string)filterContext.RouteData.Values["controller"];
            string action = (string)filterContext.RouteData.Values["action"];
            var method = filterContext.HttpContext.Request.Method;
            var hostAddress = filterContext.HttpContext.Connection.RemoteIpAddress.ToString();
            var userAgent = filterContext.HttpContext.Request.Headers["User-Agent"].ToString();
            var browser = _utils.FindBrowser(userAgent);
            try
            {
                if (filterContext.ActionArguments.Any())
                {
                    // Access the first element and perform your logic
                    if (filterContext.ActionArguments.First().Key != null)
                    {
                        if (filterContext.ActionArguments.First().Value != null)
                        {
                            param = JsonConvert.SerializeObject(filterContext.ActionArguments);
                            if (action.ToString().ToLower().Equals("Login") && method.ToString().ToLower().Equals("post") && controller.ToString().ToLower().Equals("Auth"))
                            {
                                dynamic response = JsonConvert.DeserializeObject(param);
                                response.password = _utils.Encrypt(response.password.ToString(), default(CancellationToken));

                                param = JsonConvert.SerializeObject(response);
                            }
                        }
                    }
                }

                
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "OnActionExecuting");
            }
            // Create audit log
            var audit = new AdminAuditLogs()
            {
                MethodName = action.ToString() + "/" + method,
                BrowserInfo = browser,
                Parameters = param,
                ClientIpAddress = Convert.ToString(ipHostInfo.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)),
                ServiceName = controller,
                ExecutionTime = DateTime.Now,
                AuditStatusId = auditStatusId,
                // UserId =_globalVariables.userid
                UserId = _globalVariables.userName

            };

            _db.AdminAuditLogs.Add(audit);
            _db.SaveChanges();
           // return;
        }
       
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Retrieve from session
            //Guid auditStatusId = _service.Get<Guid>("auditStatusId");
            string status = _httpContextAccessor.HttpContext.Session.GetString("auditStatusId");
          //  Guid auditStatusId = status;
            //_globalVariables = JsonConvert.DeserializeObject<GlobalVariables>(generalVariable);
            // Update status
            var audit = _db.AdminAuditLogs.FirstOrDefault(x => x.AuditStatusId.ToString() == status);
            try
            {
              
                if (audit != null)
                {
                    audit.Status = true;
                    _db.Update(audit);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "OnActionExecuted");
                audit.Exception = ex.Message;
                _db.Update(audit);
                _db.SaveChanges();
                return;
            }
        }
       
    }
}
