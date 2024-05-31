using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceManagement.ViewModels
{
    public class InsuranceRequestViewModel
    {
        [Required]
        [StringLength(10, ErrorMessage = "Number must be 10 digits or less")]
        [Column(TypeName = "varchar(10)")]
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
       

        [Display(Name = "Collateral Value FSV")]
        public string CollateralValue { get; set; }
        [Required]
        [Display(Name = "Premium")]
        public string EstimatedPremium { get; set; }
       
        public string Branchcode { get; set; }

        [Required]
        [DisplayName("Insurance Type")]
        public int InsuranceTypeId { get; set; }

        public IEnumerable<SelectListItem> InsuranceTypes { get; set; }
        [DisplayName("Insurance Sub Type")]
        public int? InsuranceSubTypeId { get; set; }

        public IEnumerable<SelectListItem> InsuranceSubTypes { get; set; }
        public int selectedInsuranceTypeId { get; set; }
        public int selectedInsuranceSubTypeId { get; set; }
    }
}
