using InsuranceCore.Interfaces;
using InsuranceInfrastructure.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceInfrastructure.Helpers
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISessionService  session, string key, object value)
        {
            //session.SetString(key, JsonConvert.SerializeObject(value));
            session.Set(key, JsonConvert.SerializeObject(value));
        }

        //public static T GetObject<T>(this ISessionService session, string key)
        //{
        //    var value = session.GetString(key);
        //    return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        //    //return default(T);
        //}
        public static T GetObject<T>(this ISessionService session, string key)
        {
            var value = session.Get<T>(key);
            return value;
        }

    }
}
