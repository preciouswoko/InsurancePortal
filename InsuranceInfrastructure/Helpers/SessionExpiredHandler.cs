using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
//using System.Web;
//using System.Web.Http.Filters;

namespace InsuranceInfrastructure.Helpers
{
    public class SessionExpiredHandler : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute, IActionFilter
    {
        private IHttpContextAccessor _hcontext;
        public SessionExpiredHandler(IHttpContextAccessor hcontext)
        {
            _hcontext = hcontext;
        }

        void Microsoft.AspNetCore.Mvc.Filters.IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {

            // check if session is supported
            if (_hcontext.HttpContext.Session != null)
            {

                // check if a new session id was generated
                if (!_hcontext.HttpContext.Session.IsAvailable)
                {

                    // If it says it is a new session, but an existing cookie exists, then it must
                    // have timed out
                    string sessionCookie = _hcontext.HttpContext.Request.Headers["Cookie"];
                    if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new
                            {
                                controller = "Auth",
                                action = "Login"
                            }));
                    }
                }
            }

            this.OnActionExecuting(filterContext);
        }
    }
}