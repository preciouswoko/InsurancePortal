using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceManagement.ViewModels
{
    public class AuthorizeRequestViewModel
    {
        public long RequestId { get; set; }
        [StringLength(2000)]
        public string Comment { get; set; }
        public string IsApproved { get; set; }
       [DisplayName("Customer ID")]
        public string T24CustomerID { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string AccountNumber { get; set; }
        public decimal? EstimatedPremium { get; set; }
        [DisplayName("Insurance Type")]
        public string InsuranceType { get; set; }
        [DisplayName("Insurance SubType")]
        public string InsuranceSubType { get; set; }
        [DisplayName("Broker Name")]
        public string Broker { get; set; }
        public bool IsSelected { get; set; }
       // public string ApprovalStatus { get; set; }
    }
}
