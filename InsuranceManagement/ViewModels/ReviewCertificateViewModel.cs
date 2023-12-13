using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceManagement.ViewModels
{
    public class ReviewCertificateViewModel
    {
        public DateTime IssuanceDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string PolicyNumber { get; set; }
      //  [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal EstimatedPremium { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Broker { get; set; }
        public string InsuranceSubType { get; set; }
        public string InsuranceType { get; set; }
        public string Underwrite { get; set; }
        public long CertificateID { get; set; }
        [DisplayName("File")]
        public string PdfFileName { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
    }

}
