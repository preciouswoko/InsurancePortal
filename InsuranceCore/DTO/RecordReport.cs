using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InsuranceCore.DTO
{
    public class RecordReport
    {
        [DisplayName("S/N")]
        public int Id { get; set; }
       
        public string ContractId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        [DisplayName("Policy Number")]
        public string PolicyNo { get; set; }
        [DisplayName("CollateralValueFSV")]

        public decimal CollateralValue { get; set; }
        public string Broker { get; set; }
        public string Underwriter { get; set; }
        [DisplayName("Policy Issuance")]

        public DateTime? DateofIssuance { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string PolicyDuration { get; set; }
        public string InsuranceType { get; set; }
        public string SubInsuranceType { get; set; }

        [DisplayName("Premium Amount")]
        public decimal EstimatedPremium { get; set; }
        public string PremiumDRAccount { get; set; }
        public string PremiumCRAccount { get; set; }
        public decimal? PremiumAmount { get; set; }
        public string PremiumUniqueID { get; set; }
        public decimal? CommissionAmount { get; set; }
        public string CommissionUniqueID { get; set; }
        public string InsuranceFlag { get; set; }

        public string Stage { get; set; }

        public string InsuranceStatus { get; set; }
        public string Branch { get; set; }


        public int? DebitPassed { get; set; }
        public DateTime? ContractMaturityDate { get; set; }
      
        public string Certificate { get; set; }
       
       
        public long RecordId { get; set; }
    }
}
