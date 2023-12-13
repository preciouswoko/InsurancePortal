using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.Models
{
    //public class InsuranceSubType
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [DisplayName("InsuranceSubType Name")]
    //    public string Name { get; set; }

    //    public string InsurancePercentage { get; set; }

    //    [DisplayName("Insurance Type")]
    //    [ForeignKey("InsuranceType")] 
    //    public int InsuranceTypeId { get; set; }

    //    public string Comment { get; set; }

    //    public string Status { get; set; }

    //    public virtual InsuranceType InsuranceType { get; set; }
    //    public ICollection<Request> Requests { get; set; }
    //    // public ICollection<InsuranceRequest> InsuranceRequest { get; set; }

    //}
    public class InsuranceSubType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("InsuranceSubType Name")]
         public string Name { get; set; }
        //[Required]
        //[Display(Name = "Broker Name")]
        //public int BrokerId { get; set; } 

        //[ForeignKey("BrokerId")]
        //public virtual Broker Broker { get; set; } 

        [Required]
        [Display(Name = "Insurance Type")]
        public int InsuranceTypeId { get; set; } 

        [ForeignKey("InsuranceTypeId")]
        public virtual InsuranceType InsuranceType { get; set; }

        

        //[Display(Name = "Percentage to Bank")]
        //public decimal? PercentageToBank { get; set; }
        //[Display(Name = "Comment")]

        //public string Comment { get; set; }
        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        //public ICollection<Request> Requests { get; set; }
    }

}
