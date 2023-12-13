using InsuranceCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class InsuranceData 
    {

        public string Disable { get; set; }
        public string View { get; set; }
        //public int Id { get; set; }
        public string InsuranceTrackingId { get; set; }
        public string PolicyNumber { get; set; }
       // public string CollateralType { get; set; }
        public decimal CollateralValue { get; set; }
        public string Broker { get; set; }
        public string Underwriter { get; set; }
        public string InsuranceCertificate { get; set; }
        public string Customer { get; set; }
        public string ContractId { get; set; }
        public string ContractFlag { get; set; }
        public int? DebitsPassed { get; set; }
        public decimal? EstimatedPremium { get; set; }
        public string InsuranceType { get; set; }
        public string SubType { get; set; }
        public string Comment { get; set; }
        public string InsuranceFlag { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerId { get; set; }

        public string AccountNumber { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public DateTime? PolicyStartDate { get; set; }
        public string Stage { get; set; }
        public DateTime? ContractMaturityDate { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsActive { get; set; }

    }
}
