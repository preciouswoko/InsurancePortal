using InsuranceCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InsuranceCore.DTO.ReusableVariables;

namespace InsuranceInfrastructure.Helpers
{
    public class PermissionFilter : IAsyncActionFilter
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<PermissionFilter> _logger;
        private readonly string[] _requiredPermissions;
        private GlobalVariables globalVariables;


        public PermissionFilter(ISessionService sessionService, ILogger<PermissionFilter> logger, params string[] requiredPermissions)
        {
            _sessionService = sessionService;
            _logger = logger;
            _requiredPermissions = requiredPermissions;
            globalVariables = _sessionService.Get<GlobalVariables>("GlobalVariables");
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get current user
            var currentUser = globalVariables; //_sessionService.GetCurrentUser();

            if (currentUser == null)
            {
                // Not logged in
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check if user has required permissions
            var hasPermission = currentUser.Permissions.Intersect(_requiredPermissions).Any();

            if (!hasPermission)
            {
                // Log unauthorized attempt
                _logger.LogWarning("User {UserId} attempted to access {Action} without required permissions: {Permissions}",
                    currentUser.userid, context.ActionDescriptor.DisplayName, string.Join(", ", _requiredPermissions));

                context.Result = new ForbidResult();
                return;
            }

            // User is authorized, execute action
            await next();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // This method can be left empty
        }
    }

    //public class PermissionFilter : IAsyncActionFilter
    //{
    //    private readonly ISessionService _sessionService;
    //    private readonly ILogger<PermissionFilter> _logger;

    //    public PermissionFilter(ISessionService sessionService, ILogger<PermissionFilter> logger)
    //    {
    //        _sessionService = sessionService;
    //        _logger = logger;
    //    }

    //    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        // Get permissions from attribute
    //        var requiredPermissions = new[] { AccessControlAttribute.Permissions.INR, AccessControlAttribute.Permissions.AUR };
    //        //var requiredPermissions = new[] { ((AccessControlAttribute)context.ActionDescriptor.FilterDescriptors[0].Filter).Permissions.INR,
    //        //    ((AccessControlAttribute)context.ActionDescriptor.FilterDescriptors[0].Filter).Permissions.AUR};
    //        // Get current user
    //        var currentUser = _sessionService.GetCurrentUser();

    //        if (currentUser == null)
    //        {
    //            // Not logged in
    //            context.Result = new UnauthorizedResult();
    //            return;
    //        }

    //        // Check if user has required permissions
    //        var hasPermission = currentUser.Permissions.Intersect(requiredPermissions).Any();

    //        if (!hasPermission)
    //        {
    //            // Log unauthorized attempt
    //            _logger.LogWarning("User {UserId} attempted to access {Action} without required permissions: {Permissions}",
    //              currentUser.Id, context.ActionDescriptor.DisplayName, string.Join(", ", requiredPermissions));

    //            context.Result = new ForbidResult();
    //            return;
    //        }

    //        // User is authorized, execute action
    //        await next();
    //    }
    //}
}
