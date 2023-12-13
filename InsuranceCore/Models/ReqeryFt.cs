using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.Models
{
    public class ReqeryFt
    {
        public string REFERENCE_NO { get; set; }
        public string DEBIT_ACCOUNT { get; set; }
        public string CREDIT_ACCOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal AMOUNT { get; set; }
        public string UNIQUEID { get; set; }
        public string RRN { get; set; }
        public string REVERSALSTATUS { get; set; }


    }
}
