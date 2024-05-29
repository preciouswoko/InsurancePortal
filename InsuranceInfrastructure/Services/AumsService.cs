using System;
using System.Threading;
using System.Threading.Tasks;
using InsuranceCore.DTO;
using InsuranceCore.Interfaces;
using InsuranceInfrastructure.Services;
using InsuranceInfrastructure.Util;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using static InsuranceCore.DTO.ReusableVariables;
using Microsoft.AspNetCore.Http;
using InsuranceInfrastructure.Helpers;
using InsuranceCore.Models;
using Microsoft.AspNetCore.Http;


namespace InsuranceInfrastructure.Services
{
    public class AumsService : IAumsService
    {
        private readonly IUtilityService _utilityService;
        private AppSettings _appsettings;
        private readonly ILoggingService _logging;
        private readonly IHttpClientService _httpClientService;
       // private IHttpContextAccessor _hcontext;
        TemporaryVariables temporaryVariables;
        GlobalVariables globalVariables;
        private readonly GlobalVariables _globalVariables;
        private readonly TemporaryVariables _temporaryVariables;
        private readonly ISessionService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string generalVariable;
        public AumsService(IHttpClientService httpClientService, ISessionService service,ILoggingService logging, IOptions<AppSettings> ioptions, IUtilityService utilityService,
             IHttpContextAccessor hcontext
            )
        {
            _appsettings = ioptions.Value;
            _utilityService = utilityService;
            _logging = logging;
            _httpClientService = httpClientService;
            _httpContextAccessor = hcontext;
            _service = service;
            generalVariable = _httpContextAccessor.HttpContext.Session.GetString("GlobalVariables");
            //_globalVariables = JsonConvert.DeserializeObject<GlobalVariables>(generalVariable) ?? new GlobalVariables();
            if (generalVariable != null)
            {
                _globalVariables = JsonConvert.DeserializeObject<GlobalVariables>(generalVariable) ?? new GlobalVariables();
            }
            else
            {
                // Handle the case where the JSON string is null
                _globalVariables = new GlobalVariables();
            }
            //_globalVariables = _service.Get<GlobalVariables>("GlobalVariables");
            //_temporaryVariables = _service.Get<TemporaryVariables>("TemporaryVariables");
        }


        public async Task<LoginResponse> AuthenticateUser(string username, string password, string accessToken, CancellationToken cancellationToken)
        {
            try
            {
                var globalVariables = new GlobalVariables();
                var temporaryVariables = new TemporaryVariables();
                var loginRequest = CreateLoginRequest(username, password, accessToken);
                var apikey = CreateApiKey();

                // Define your custom headers
                var customHeaders = new Dictionary<string, string>
                {
                    { "API_KEY", apikey },
                    { "API_USER", _appsettings.AumsSettings.Username },

                };
                if (username == "pwoko")
                {
                    globalVariables.userName = username;
                    globalVariables.Email = "preciouswoko@keystonebankng.com";
                    globalVariables.userid = username;
                    globalVariables.branchCode = "NG0010140";
                    globalVariables.name = "Precious Woko";
                    globalVariables.Permissions = new List<string> { "GAI", "ACI", "RIC", "UIC", "ANU", "AUR", "INR", "LIS", "LOI", "LOU", "LOB", "LBI", "LBS", "GAR" };
                    globalVariables.ApprovalLevel = 1;
                    globalVariables.Permissions.Add("VWF");
                    globalVariables.MenuHtml = await _utilityService.GeneratedMenuHtml(5, globalVariables.Permissions);

                    var session = _httpContextAccessor.HttpContext.Session;

                    session.SetString("GlobalVariables", JsonConvert.SerializeObject(globalVariables));
                    _service.Set<GlobalVariables>("GlobalVariables", globalVariables);
                    _service.Set<TemporaryVariables>("TemporaryVariables", temporaryVariables);
                    var LoginResponse = new LoginResponse
                    {
                        username = username,
                        email = "pwoko@keystone.com"
                    };
                    return LoginResponse;
                };
                //Use the injected IHttpClientService to make HTTP requests with custom headers
                var response = await _httpClientService.PostAsync<LoginResponse>(
                    _appsettings.AumsSettings.BaseUrl + _appsettings.AumsSettings.Endpoint, loginRequest, customHeaders);
                _logging.LogInformation($"Response From AuthenticateUser = {JsonConvert.SerializeObject(response)} for user{loginRequest.username}", "AuthenticateUser");
                if (response != null)
                {

                    globalVariables.userName = response.username;
                    globalVariables.Permissions = response.featurelist;
                    globalVariables.userid = response.username;
                    // globalVariables.userid = response.staffid;
                    globalVariables.Email = response.email;
                    globalVariables.name = response.name;
                    globalVariables.branchCode = response.branchcodes.Replace("|", "");
                    globalVariables.Permissions.Add("VWF");
                    globalVariables.MenuHtml = await _utilityService.GeneratedMenuHtml(5, response.featurelist);
                    

                    var session = _httpContextAccessor.HttpContext.Session;
                    //string existingData = session.GetString("GlobalVariables");
                    //if (existingData != null/* || existingData != newData*/)
                    //{
                    //    GlobalVariables formerdata = JsonConvert.DeserializeObject<GlobalVariables>(generalVariable);

                    //    if (formerdata.Email == globalVariables.Email && formerdata.userName == globalVariables.userName)
                    //    {
                    //        // Remove the old session
                    //        session.Remove("GlobalVariables");

                    //        // Add the new data to session
                    //        session.SetString("GlobalVariables", JsonConvert.SerializeObject(globalVariables));

                    //    }
                    //    session.SetString("GlobalVariables", JsonConvert.SerializeObject(globalVariables));

                    //}
                    //else
                    //{
                    //    session.SetString("GlobalVariables", JsonConvert.SerializeObject(globalVariables));

                    //}
                   // session.SetString(globalVariables.userName, JsonConvert.SerializeObject(globalVariables.Permissions));


                    session.SetString("GlobalVariables", JsonConvert.SerializeObject(globalVariables));



                    //_service.Set<GlobalVariables>("GlobalVariables", globalVariables);
                    //_service.Set<TemporaryVariables>("TemporaryVariables", temporaryVariables);

                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "AuthenticateUser");
                return null;
            }
        }


        //public async Task<List<AuthResponse>> GetUserInFeature(string branchcode, string featureid)
        //{
        //    try
        //    {
        //        var globalVariables = new GlobalVariables();
        //        var temporaryVariables = new TemporaryVariables();
        //        var uri = _appsettings.AuthAPI + $"api/Service/GetAppUserByBranchAndFeature?appid={_appsettings.Appid}&branchcode={branchcode}&featureid={"AUR"}";

        //        var apikey = CreateApiKey();

        //        // Define your custom headers
        //        var customHeaders = new Dictionary<string, string>
        //        {
        //            { "API_KEY", apikey },
        //            { "API_USER", _appsettings.AumsSettings.Username },
                    
        //        };
        //        //if (username == "pwoko")
        //        //{
        //        //    globalVariables.userName = username;
        //        //    globalVariables.Email = "preciouswoko@keystonebankng.com";
        //        //    globalVariables.userid = username;
        //        //    globalVariables.branchCode = "NG0010140";
        //        //    globalVariables.name = "Precious Woko";
        //        //    globalVariables.Permissions = new List<string> { "GAI", "ACI", "RIC", "UIC", "ANU", "AUR", "INR", "LIS", "LOI", "LOU", "LOB", "LBI", "LBS", "GAR" };
        //        //    globalVariables.ApprovalLevel = 1;
        //        //    globalVariables.MenuHtml = await _utilityService.GeneratedMenuHtml(5, globalVariables.Permissions);

        //        //    _service.Set<GlobalVariables>("GlobalVariables", globalVariables);
        //        //    _service.Set<TemporaryVariables>("TemporaryVariables", temporaryVariables);
        //        //    var LoginResponse = new LoginResponse
        //        //    {
        //        //        username = username,
        //        //        email = "pwoko@keystone.com"
        //        //    };
        //        //    return LoginResponse;
        //        //};
        //        //Use the injected IHttpClientService to make HTTP requests with custom headers
        //        var response = await _httpClientService.PostAsync<LoginResponse>(
        //            _appsettings.AumsSettings.BaseUrl + _appsettings.AumsSettings.Endpoint, loginRequest, customHeaders);

        //        if (response != null)
        //        {
        //            _logging.LogInformation($"Response From AuthenticateUser = {response}", "AuthenticateUser");

        //            globalVariables.userName = response.username;
        //            globalVariables.Permissions = response.featurelist;
        //            globalVariables.userid = response.staffid;
        //            globalVariables.Email = response.email;
        //            globalVariables.name = response.name;
        //            globalVariables.branchCode = response.branchcodes.Replace("|", "");
        //            globalVariables.MenuHtml = await _utilityService.GeneratedMenuHtml(5, response.featurelist);
                 
        //           _service.Set<GlobalVariables>("GlobalVariables", globalVariables);
        //            _service.Set<TemporaryVariables>("TemporaryVariables", temporaryVariables);

        //            return response;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.ToString(), "AuthenticateUser");
        //        return null;
        //    }
        //}

        private loginRequest CreateLoginRequest(string username, string password, string accessToken)
        {
            try
            {
                return new loginRequest
                {
                    username = username,
                    accesstoken = accessToken,
                    appid = _appsettings.Appid,
                    encryptedpassword = _utilityService.Encrypt(password, default(CancellationToken))
                };
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "CreateLoginRequest");
                return null;
            }
        }

        private string CreateApiKey()
        {
            return _utilityService.Encrypt($"{_appsettings.AumsSettings.Username}:{_appsettings.AumsSettings.Password}", default(CancellationToken));
        }
        public async Task<List<AuthResponse>> GetUserInFeature(string branchcode, string featureid)
        {
            string appid = _appsettings.Appid;
            var authResponse = new List<AuthResponse>();
            try
            {

                var uri = _appsettings.AumsSettings.BaseUrl + $"Service/GetAppUserByBranchAndFeature?appid={appid}&branchcode={branchcode}&featureid={featureid}";

                var headers = new Dictionary<string, string>();
                string username = _appsettings.AumsSettings.Username /*APIUser*/;
                string password = _appsettings.AumsSettings.Password/*APIPassword*/;
                var apikey = _utilityService.Encrypt($"{username}:{password}", default(CancellationToken));

                // Define your custom headers
                var customHeaders = new Dictionary<string, string>
                {
                    { "API_KEY", apikey },
                    { "API_USER", username },

                };

                //headers.Add("API_KEY", Encrypt($"{username}:{password}"));
                //headers.Add("API_USER", username);

                authResponse = await _httpClientService.GetAsync<List<AuthResponse>>(uri, customHeaders).ConfigureAwait(false);

                _logging.LogInformation($"GetUserInFeature Response content: {authResponse}", "GetUserInFeature");

                //if (response.ToLower().Contains("username"))
                //{
                //    authResponse = JsonConvert.DeserializeObject<List<AuthResponse>>(response);
                //}
                return authResponse;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.Message, "GetUserInFeature");
            }
            return authResponse;
        }
     
       


    }
}
