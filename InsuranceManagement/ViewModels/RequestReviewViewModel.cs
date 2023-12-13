using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceManagement.ViewModels
{
    public class RequestReviewViewModel
    {
        
            public string AccountNumber { get; set; }

            public string AccountName { get; set; }

            public string CustomerName { get; set; }

            public string CustomerId { get; set; }

            public string CustomerEmail { get; set; }
          
            public long RequestID { get; set; }

            public string AuthorizedByName  { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal CollateralValueFSV  { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal EstimatedPremium { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Premium { get; set; }
        public string InsuranceType { get; set; }

            public string InsuranceFlag { get; set; }
        public string Underwrite { get; set; }
        public string InsuranceSubTypes { get; set; }
            public string Broker { get; set; }
            public string Comment { get; set; }
        public string ErrorMessage { get; set; }
        public string RequestType  { get; set; }
        public string Status { get; set; }
        public string Stage { get; set; }
    }
}
