using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceManagement.ViewModels
{
    public class AssignContractIDViewModel
    {
        public string CustomerID { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string AccountNumber { get; set; }
        public long RequestId { get; set; }
        public decimal? Premium { get; set; }
        [DisplayName("Under write")]
        public string Underwrite { get; set; }
        [DisplayName("Insurance Type")]
        public string InsuranceType { get; set; }
        [DisplayName("Insurance SubType")]
        public string InsuranceSubType { get; set; }
        [DisplayName("Broker Name")]
        public string Broker { get; set; }
        public string PolicyNumber { get; set; }
        public string CollateralValue { get; set; }
        public DateTime PolicyExpiryDate { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public string Certificate { get; set; }
        public int DebitsPassed { get; set; }
        [DisplayName("Contract ID")]
        public string ContractID { get; set; }
    }
}
