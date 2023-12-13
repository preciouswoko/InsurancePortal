using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class InsuranceInformation
    {
        public int RecordId { get; set; }
        public string ContractId { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string Broker { get; set; }
        public string Underwriter { get; set; }
        public decimal CollateralValue { get; set; }
        public decimal EstimatedPremium { get; set; }
        public string InsuranceType { get; set; }
        public string SubInsuranceType { get; set; }
        public int NoOfDebitPassed { get; set; }
        public string InsuranceFlag { get; set; }
        public DateTime ContractMaturityDate { get; set; }
    }

}
