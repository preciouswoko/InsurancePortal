using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceInfrastructure.Helpers
{
   // [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AccessControlAttribute1 : Attribute, IActionFilter
    {
        public string[] RequiredPermissions { get; }

        public AccessControlAttribute1(params string[] permissions)
        {
            RequiredPermissions = permissions;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Your code here if needed after the action executes
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Your authorization logic here
        }
    }
}
