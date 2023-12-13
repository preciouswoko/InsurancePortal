using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.Models
{
    public class AccountEnquiry
    {
        public string t24AccountNumber { get; set; }
        public string accountNumber { get; set; }
        public string customerId { get; set; }
        public string accountOfficer { get; set; }
        public int accountRestricton { get; set; }
        public string currency { get; set; }
        public string accountName { get; set; }
        public double lockedAmount { get; set; }
        public string status { get; set; }
        public string productCode { get; set; }
        public double availableBalance { get; set; }
        public double ledgerBalance { get; set; }
        public string branchCode { get; set; }
        public bool error { get; set; }
        public string errorMessage { get; set; }
        public string bvn { get; set; }
        public object corporateIndividualFlag { get; set; }
        public object currentSavingFlag { get; set; }
        public string category { get; set; }
        public string openingDate { get; set; }
    }

}
