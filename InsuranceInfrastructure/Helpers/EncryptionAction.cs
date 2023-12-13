using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InsuranceInfrastructure.Helpers
{
    public class EncryptionActionAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            var queryString = filterContext.HttpContext.Request.Query;
            var anc = queryString["anc"];
            if (anc.Any())
            {
                string encryptedQueryString = anc.ToString();
                string decrptedString = Ccheckg.convert_pass2(encryptedQueryString.ToString(), 1);
                string[] paramsArrs = decrptedString.Split('?');

                for (int i = 0; i < paramsArrs.Length; i++)
                {
                    string[] paramArr = paramsArrs[i].Split('=');
                    decryptedParameters.Add(paramArr[0], (paramArr[1]));
                }
            }
            for (int i = 0; i < decryptedParameters.Count; i++)
            {
                filterContext.ActionArguments[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
            }
            //this.OnActionExecuting(filterContext);
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {   // do something after the action executes
        }
    }

}
