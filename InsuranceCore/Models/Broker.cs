using InsuranceCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceCore.Models
{
    public class Broker
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Broker Name")]
        public string BrokerName { get; set; }
        public string AccountName { get; set; }
        public string Status { get; set; } = BrokerStatus.Active.ToString();
        public string CustomerID { get; set; }
        public string AccountNumber { get; set; }
        public string EmailAddress { get; set; }
        public ICollection<Underwriter> Underwriters { get; set; }
       // public ICollection<Request> Requests { get; set; }
        // public ICollection<InsuranceRequest> InsuranceRequest { get; set; }
    }
}
        
       