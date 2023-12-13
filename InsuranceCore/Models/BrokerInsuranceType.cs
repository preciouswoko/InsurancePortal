using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.Models
{
    public class BrokerInsuranceType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Broker Name")]
        public int BrokerId { get; set; }
        //[Required]
        //public string Name { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        [Display(Name = "InsuranceType Name")]
        public int InsuranceTypeId { get; set; }
       
        [ForeignKey("BrokerId")]
        public virtual Broker Broker { get; set; }
        public virtual InsuranceType InsuranceType { get; set; }
    }
}
