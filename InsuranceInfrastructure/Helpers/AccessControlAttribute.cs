using InsuranceCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceInfrastructure.Helpers
{
    public class AccessControlAttribute : TypeFilterAttribute
    {
        //private readonly ISessionService _service;
        //public AccessControlAttribute(ISessionService service) : base(typeof(PermissionFilter))
        //{
        //    Arguments = new object[] { service };
        //}
        //public struct Permissions

        //{

        //    public const string INR = "INR";

        //    public const string AUR = "AUR";

        //}
        public AccessControlAttribute(/*ISessionService service,*/ params string[] permissions) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { /*service,*/ permissions };
        }

      
    }

}
