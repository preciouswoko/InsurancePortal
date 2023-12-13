using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceInfrastructure.Util
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermissionAttribute : Attribute
    {
        public string PermissionName { get; }

        public RequirePermissionAttribute(string permissionName)
        {
            PermissionName = permissionName;
        }
    }

}
