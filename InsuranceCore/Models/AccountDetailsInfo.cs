using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsuranceCore.Models
{
    public class AccountDetailsInfo
    {
        // Properties
        public string statusmessage { get; set; }



        public string status { get; set; }
        public string BranchCode { get; set; }



        public string AccountNo { get; set; }



        public string T24AccountNo { get; set; }



        public string AccountName { get; set; }



        public string CustomerID { get; set; }



        public string BankCode { get; set; }



        public string BVN { get; set; }



        public decimal AccountBalance { get; set; }



        public string FormatedAccountBalance =>
            $"{this.AccountBalance:0.##}";



        public string MaskedAccountNo =>
           !string.IsNullOrEmpty(this.AccountNo) ? $"{this.AccountNo.Substring(0, 3)}****{this.AccountNo.Substring(7, 3)}" : "";



        public string CustomerPhone { get; set; }



        public List<string> CustomerPhones
        {
            get
            {
                List<string> list1;
                if (this.CustomerPhone == null)
                {
                    list1 = new List<string>();
                }
                else
                {
                    char[] separator = new char[] { ']' };
                    list1 = this.CustomerPhone.Split(separator).ToList<string>();
                }
                return list1;
            }
        }



        public string CustomerEmail { get; set; }



        public List<string> CustomerEmails
        {
            get
            {
                List<string> list1;
                if (this.CustomerEmail == null)
                {
                    list1 = new List<string>();
                }
                else
                {
                    char[] separator = new char[] { ']' };
                    list1 = this.CustomerEmail.Split(separator).ToList<string>();
                }
                return list1;
            }
        }



        public string DOB { get; set; }



        public string Gender { get; set; }
        public string type_code { get; set; }
        public string currency { get; set; }
        public string ChannelCode { get; set; }
        public string SessionId { get; set; }



    }
}
