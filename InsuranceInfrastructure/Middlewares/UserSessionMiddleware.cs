using InsuranceInfrastructure.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static InsuranceCore.DTO.ReusableVariables;

namespace InsuranceInfrastructure.Middlewares
{
    public class UserSessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            string existingData = session.GetString("GlobalVariables");

            if (existingData == null)
            {
                await _next(context);
                return;
            }

            string userId = null;

            if (existingData != null)
            {
                GlobalVariables formerdata = JsonConvert.DeserializeObject<GlobalVariables>(existingData);
                userId = formerdata.userid;
            }

            // string userId = context.User.Identity.Name; // Replace this with your actual way of getting the user ID
            string sessionId = context.Session.Id;

            if (userId == null && sessionId != null)
            {
                await _next(context);
                return;
            }

            if (UserSessionManager.IsUserAlreadyLoggedIn(userId, sessionId))
            {
                context.Response.Redirect("/Home/LogOff"); // Redirect to logout or another appropriate action
                return;
            }

            UserSessionManager.AddUserSession(userId, sessionId);

            await _next(context);

            UserSessionManager.RemoveUserSession(userId);
        }
    }

}
