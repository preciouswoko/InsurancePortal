using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace InsuranceInfrastructure.Helpers
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public SessionMiddleware(RequestDelegate next, ILoggerFactory logFactory)
        {
            _next = next;
            _logger = logFactory.CreateLogger("MyMiddleware");
        }

        public async Task Invoke(HttpContext httpContext)
        {
            _logger.LogInformation("MyMiddleware executing..");

            if (httpContext.Session == null ||
                !httpContext.Session.TryGetValue("GlobalVariables", out byte[] val))
            {
                try
                {
                    // Generate the login URL using UrlHelper
                    var urlHelper = httpContext.RequestServices.GetRequiredService<IUrlHelper>();
                    var loginUrl = urlHelper.Action("Login", "Auth");

                    httpContext.Response.Redirect(loginUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString(), "SessionMiddleware-Invoke");

                    // Handle the exception or redirect to the login page again
                    httpContext.Response.Redirect("/Auth/Login");
                }
            }

            await _next(httpContext); // calling the next middleware
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionMiddleware>();
        }
    }
}
