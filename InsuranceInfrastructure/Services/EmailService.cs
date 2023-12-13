using InsuranceCore.DTO;
using InsuranceCore.Interfaces;
using InsuranceInfrastructure.Util;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;

namespace InsuranceInfrastructure.Services
{
    public class EmailService : IEmailService
    {
        private static ILogger<EmailService> _logging;

       // private readonly IConfiguration _configuration;
        private AppSettings _appsettings;
        public EmailService(ILogger<EmailService> logging, IOptions<AppSettings> ioptions/*, IConfiguration configuration*/)
        {
            _logging = logging;
            _appsettings = ioptions.Value;
           // _configuration = configuration;
        }
        public void SendEmail(EmailMessage email)
        {
            throw new NotImplementedException();
        }
        //public void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body, byte[] attachment = null, string fileName = "")
        //{
        //    // Instantiate a new instance of MailMessage
        //    MailMessage mMailMessage = new MailMessage();

        //    // Set the sender address of the mail message

        //    mMailMessage.From = new MailAddress(from);


        //    // Set the recepient address of the mail message
        //    foreach (var t in to.Split(','))
        //    {
        //        mMailMessage.To.Add(new MailAddress(t));
        //    }
        //    // Check if the bcc value is null or an empty string
        //    if ((bcc != null) && (bcc != string.Empty))
        //    {
        //        // Set the Bcc address of the mail message
        //        mMailMessage.Bcc.Add(new MailAddress(bcc));
        //    }

        //    // Check if the cc value is null or an empty value
        //    if ((cc != null) && (cc != string.Empty))
        //    {
        //        // Set the CC address of the mail message
        //        mMailMessage.CC.Add(new MailAddress(cc));
        //    }       // Set the subject of the mail message
        //    mMailMessage.Subject = subject;
        //    // Set the body of the mail message
        //    // mMailMessage.Body = body;
        //    // Set the format of the mail message body as HTML
        //    mMailMessage.IsBodyHtml = true;
        //    // Set the priority of the mail message to normal
        //    mMailMessage.Priority = MailPriority.Normal;
        //    mMailMessage.AlternateViews.Add(MailBody(body));
        //    //instantiate a new instance of SmtpClient
        //    if (attachment != null)
        //    {
        //        MemoryStream attach = new MemoryStream(attachment);
        //        Attachment data = new Attachment(attach, fileName);
        //        mMailMessage.Attachments.Add(data);
        //    }

        //    SmtpClient smtp = new SmtpClient();
        //    //smtp.Host = _appsettings./* ConfigurationManager.AppSettings["SMTPHost"].ToString();*/
        //    //smtp.Port = int.Parse(ConfigurationManager.AppSettings["SMTPPort"].ToString());
        //    ////send the mail message

        //    //smtp.Send(mMailMessage);
        //    string strSMTP = _appsettings.MailServer; /*GetConfigValue("MailServer");*/
        //    SmtpClient client = new SmtpClient(strSMTP.Trim(), 25)
        //    {
        //        // Configure the SmtpClient with the credentials used to connect
        //        // to the SMTP server.
        //        Credentials = new NetworkCredential()//"user@somecompany.com", "password");
        //    };
        //    client.Send(mMailMessage);
        //}
        //private static AlternateView MailBody(string body)
        //{
        //    var enviroment = System.Environment.CurrentDirectory;
        //    string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;
        //    var appDomain = System.AppDomain.CurrentDomain;
        //    var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
        //    //var basePath = projectDirectory;
        //    string path = Path.Combine(basePath, "Resource\\Template\\Email", "logo.jpg");
        //    //string path = HostingEnvironment.MapPath("~/Resource/Template/Email/logo.jpg");
        //    LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        //    Img.ContentId = "logo";
        //    string str = body;
        //    AlternateView AV = AlternateView.CreateAlternateViewFromString(str, null, MediaTypeNames.Text.Html);
        //    AV.LinkedResources.Add(Img);
        //    return AV;
        //}
        public  void SmtpSendMail(string strTo, string StrBody, string StrSubject, string strFrom = null, string strCc = null)
        {
            if(strFrom == null)
            {
                strFrom = _appsettings.SourceEmail;
            };
            // return;
            new Thread(() =>
            {
               // string strSMTP = _configuration["AppSettings:MailServer"];
                string strSMTP = _appsettings.MailServer; /*GetConfigValue("MailServer");*/
                SmtpClient client = new SmtpClient(strSMTP.Trim(), 25)
                {
                    // Configure the SmtpClient with the credentials used to connect
                    // to the SMTP server.
                    Credentials = new NetworkCredential()//"user@somecompany.com", "password");
                };
                // Create the MailMessage to represent the e-mail being sent.
                try
                {



                    using (MailMessage msg = new MailMessage())
                    {
                       // strFrom = /*string.IsNullOrWhiteSpace(strFrom)*/ /*? $"{Helper.GetConfigValue("SenderName")}<{Util.Helper.GetConfigValue("SenderEmail")}>" : */ strFrom;





                        // Configure the e-mail sender and subject.
                        msg.IsBodyHtml = true;
                        msg.From = new MailAddress(strFrom);
                         


                        msg.Subject = StrSubject;
                        //msg.Body = "<html>" + "<font size='4'  face='Book Antiqua'>Please find attached the Cheque & Cash inflow and Outflow report for " + strDate + "." + " " + "Kindly disseminate to the appropriate business head.<br><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p> Best Regards,</font>" + "</html>";
                        msg.Body = StrBody;



                        msg.To.Add(new MailAddress(strTo));
                        //if (strFileAttachment != null)
                        //{
                        //    string[] arrayAttach = strFileAttachment.Split(',');
                        //    foreach (string strAttach in arrayAttach)
                        //    {
                        //        msg.Attachments.Add(new Attachment(strAttach, "application/vnd.ms-excel"));
                        //    }
                        //}
                        //for (int i = 0; i < strTo.Length; i++)
                        //{
                        //    msg.To.Add(new MailAddress(strTo[i].ToString()));
                        //}



                        //if (strCc != null)
                        //{
                        //    for (int i = 0; i < strCc.Length; i++)
                        //    {
                        //        msg.CC.Add(new MailAddress(strCc[i].ToString()));
                        //    }



                        //}
                        string strCc1 = _appsettings.CC.ToString();
                        if (strCc == null)
                        {
                            string[] ccAddresses = strCc1.Split(',');
                            foreach (string ccAddress in ccAddresses)
                            {
                                msg.CC.Add(new MailAddress(ccAddress.Trim()));
                            }
                        }


                        client.Send(msg);
                        _logging.LogInformation($"Message {StrSubject}) Sent to: {strTo}", "SmtpSendMail");



                    }
                }
                catch (SmtpException ex)
                {
                    _logging.LogError(string.Format("SmtpException from SmtpSendMail {0}", ex.Message + " " + ex.Source.ToString() + " " + ex.StackTrace), "SmtpSendMail");
                    //  throw new Exception(ex.Message + " " + ex.Source.ToString() + " " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    _logging.LogError(string.Format("Exception from SmtpSendMail {0}", ex.Message + " " + ex.Source.ToString() + " " + ex.StackTrace), "SmtpSendMail");
                    // throw new Exception(ex.Message + " " + ex.Source.ToString() + " " + ex.StackTrace);
                }
            }).Start();
        }
    }
}
