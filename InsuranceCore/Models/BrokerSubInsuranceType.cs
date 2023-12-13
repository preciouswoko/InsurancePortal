using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.Models
{
    public class BrokerSubInsuranceType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Broker Name")]
        public int BrokerId { get; set; }

        [ForeignKey("BrokerId")]
        public virtual Broker Broker { get; set; }

        [Required]
        [Display(Name = "Insurance Type")]
        public int BrokerInsuranceTypeId { get; set; } 

        [ForeignKey("BrokerInsuranceTypeId")]
        public virtual BrokerInsuranceType InsuranceType { get; set; }

        [Display(Name = "Percentage to Bank")]
        public decimal? PercentageToBank { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }
    }

}
