using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceInfrastructure.Util
{
    public class AppSettings
    {
        public string Key { get; set; }
        public string Iv { get; set; }
        public string Appid { get; set; }
        public string MailServer { get; set; }
        public string SourceEmail { get; set; }
        public string CC { get; set; }
        public string BasePortalURL { get; set; }
        public string GLAccountNumber { get; set; }
        public AumsSettings AumsSettings { get; set; }
        public FileUploadSettings FileUploadSettings { get; set; }
        public string UsernameAuthAD { get; set; }
        public string PasswordAuthAD { get; set; }
        public string UserSecretKey { get; set; }
        public string T24WsUsername { get; set; }
        public string T24Wspassword { get; set; }
        public string T24WsEndpoint { get; set; }
        public string T24Username { get; set; }
        public string T24password { get; set; }
        public string T24Username2 { get; set; }
        public string T24password2 { get; set; }
        public string TestTO { get; set; }
        public string Envirnoment { get; set; }
        public string INTERFREF { get; set; }
        public string COMMISSIONTYPE { get; set; }
        public string Channel { get; set; }
        public string AlwaysSerializeOFS { get; set; }
        public string TRANSACTIONTYPE { get; set; }
        public string DISTRIBNAME { get; set; }
        public string VAT { get; set; }
    }
    public class AumsSettings
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Endpoint { get; set; }
    }
    public class FileUploadSettings
    {
        public string UploadsFolder { get; set; }
        
    }
}
