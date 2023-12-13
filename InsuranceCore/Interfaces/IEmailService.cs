using InsuranceCore.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailMessage email);
        void SmtpSendMail(string strTo, string StrBody, string StrSubject, string strFrom = null, string strCc = null);
    }

}
