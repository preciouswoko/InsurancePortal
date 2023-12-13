using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceManagement.ViewModels
{
    public class UploadCertViewModel
    {
        public string T24CustomerID { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string AccountNumber { get; set; }
        public long RequestId { get; set; }
       
        public decimal? EstimatedPremium { get; set; }
        [DisplayName("Under write")]
        public string Underwrite { get; set; }
        [DisplayName("Insurance Type")]
        public string InsuranceType { get; set; }
        [DisplayName("Insurance SubType")]
        public string InsuranceSubType { get; set; }
        [DisplayName("Broker Name")]
        public string Broker { get; set; }
        public string Upload { get; set; }
    }
}
