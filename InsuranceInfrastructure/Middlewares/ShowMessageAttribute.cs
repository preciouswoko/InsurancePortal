using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InsuranceInfrastructure.Middlewares
{
    public class ShowMessageAttribute : ActionFilterAttribute
    {
        public string TempDataKey { get; set; }

        public ShowMessageAttribute(string tempDataKey)
        {
            TempDataKey = tempDataKey;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is Controller controller)
            {
                if (controller.TempData.TryGetValue(TempDataKey, out var message))
                {
                    controller.ViewBag.ResultMessage = message.ToString();
                }
            }

            base.OnActionExecuted(context);
        }
    }

}
