using InsuranceCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static InsuranceCore.DTO.ReusableVariables;

namespace InsuranceInfrastructure.Helpers
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISessionService _sessionService;
        private readonly ILogger<PermissionMiddleware> _logger;

        public PermissionMiddleware(RequestDelegate next, ISessionService sessionService, ILogger<PermissionMiddleware> logger)
        {
            _next = next;
            _sessionService = sessionService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var session = context.Session;

            // Get the controller name from route data with a null check and a default value
            var currentRoute = (context.GetRouteData()?.Values["controller"]?.ToString() ?? string.Empty).ToLower();

            if (currentRoute == "Auth" && context.Request.Path.Value.ToLower() == "/Auth/Login")
            {
                // Allow access to the login route without checking permissions
                await _next(context);
                return;
            }

            var userPermissionsList = _sessionService.Get<GlobalVariables>("GlobalVariables") ?? new GlobalVariables();
            if (userPermissionsList.Permissions == null)
            {
                // No permissions in session, return 403
                context.Response.StatusCode = 403;
                return;
            }

            var userPermissions = string.Join(",", userPermissionsList.Permissions);

            if (userPermissions == null)
            {
                // No permissions in session, return 403
                context.Response.StatusCode = 403;
                return;
            }

            var permissions = userPermissions.Split(',');

            if (!UserHasPermission(currentRoute, permissions))
            {
                context.Response.StatusCode = 403;
                return;
            }

            await _next(context);
        }

        private bool UserHasPermission(string route, string[] permissions)
        {
            if (String.IsNullOrEmpty(route))
                return false;

            if (permissions.Contains(route))
                return true;

            // Check for generic permissions
            if (route.Contains("reports") && permissions.Contains("reports"))
                return true;

            return false;
        }
    }
}
