using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.Models
{
    //public class InsuranceType
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    [DisplayName("InsuranceType Name")]
    //    public string Name { get; set; }
    //    public string InsurancePercentage { get; set; }
    //    public string Comment { get; set; }
    //    public string Status { get; set; }
    //    [DisplayName("Broker")]
    //    public int BrokerId { get; set; }

    //    public virtual Broker Broker { get; set; }
    //    [DisplayName("Insurance SubType")]
    //    public int? InsuranceSubType { get; set; }
    //    [DisplayName("Insurance SubTypes")]
    //    public ICollection<InsuranceSubType> SubTypes { get; set; }
    //    public ICollection<Request> Requests { get; set; }
    //    // public List<InsuranceSubType> SubTypes { get; set; }
    //    //  public ICollection<InsuranceRequest> InsuranceRequest { get; set; }

    //}
    public class InsuranceType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("InsuranceType Name")]
        public string Name { get; set; }
        //[Required]
        //[Display(Name = "Broker Name")]
        //public int BrokerId { get; set; }

        //[ForeignKey("BrokerId")]
        //public virtual Broker Broker { get; set; } 

        //[Display(Name = "Percentage to Bank")]
        //public decimal? PercentageToBank { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }
        //[Display(Name = "Comment")]
        //public string Comment { get; set; }
        public virtual ICollection<InsuranceSubType> SubInsuranceTypes { get; set; }
        //public ICollection<Request> Requests { get; set; }

    }

}
