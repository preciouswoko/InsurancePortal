//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace InsuranceInfrastructure.Services
//{
//    public class SessionAdapter
//    {
//        private ISession _session;

//        public SessionAdapter(ISession session)
//        {
//            _session = session;
//        }

//        public void SetString(string key, string value)
//        {
//            Encoding encoding = Encoding.UTF8;
//            _session.Set(key, encoding.GetBytes(value));
//        }

//        //public string GetString(string key)
//        //{
//        //    return _session.Get(key) as string;
//        //}
//    }
//}
