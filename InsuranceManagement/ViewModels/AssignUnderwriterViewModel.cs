using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceManagement.ViewModels
{
    public class AssignUnderwriterViewModel
    {
        public long RequestId { get; set; }


        [DisplayName("Customer ID")]
        public string CustomerID { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string AccountNumber { get; set; }
        public decimal UpdatedPremium { get; set; }
        public decimal Premium { get; set; }

        public string RequestType { get; set; }
        public string CollateralValue { get; set; }
        public string Comment { get; set; }

        [Required]
        [DisplayName("Broker")]
        public int? BrokerId { get; set; }
        [Required]
        [DisplayName("Underwriter")]
        public int? UnderwriterId { get; set; }
        public IEnumerable<SelectListItem> Brokers { get; set; }
      
        public int selectedBrokerId { get; set; }
       
        // public List<Underwriters> Underwriters { get; set; }
        public IEnumerable<SelectListItem> Underwriters { get; set; }
        public int selectedUnderwriterId { get; set; }


        [DisplayName("Insurance Type")]
        public string InsuranceType { get; set; }
        [DisplayName("Insurance SubType")]
        public string InsuranceSubType { get; set; }


        //public IEnumerable<SelectListItem> Brokers { get; set; }
        //public List<Underwriters> Underwriters { get; set; }


    }
    public class Underwriters
    {
        public int Id { get; set; }
        public string Underwrite { get; set; }
    }
}
