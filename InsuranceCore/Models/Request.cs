using InsuranceCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.Models
{
    public class Request
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(15)]
        [Column(TypeName = "varchar(15)")]
        public string RequestID { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Number must be 10 digits or less")]
        [Column(TypeName = "varchar(10)")]
        [DisplayName("Account Number")]
        public string AccountNo { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        [DisplayName("Account Name")]
        public string AccountName { get; set; }
        [Required]
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string Branchcode  { get; set; }
        [Required]
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        [DisplayName("Customer ID")]
        public string CustomerID { get; set; }
        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string CustomerName { get; set; }
        [DisplayName("Customer Email")]
        [Required(ErrorMessage = "The Customer Email field is required.")]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string CustomerEmail { get; set; }
        

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        [DisplayName("CollateralValueFSV")]
        public decimal CollateralValue { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Premium { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal UpdatedPremium { get; set; }
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        [DisplayName("Contract ID")]
        public string ContractID { get; set; }


        [Required]
        [DisplayName("Broker")]
        public int BrokerID { get; set; }

        [ForeignKey("BrokerID")]
        public virtual Broker Broker { get; set; }

        [Required]
        [DisplayName("Insurance Type")]
        public int InsuranceTypeId { get; set; }

        [ForeignKey("InsuranceTypeId")]
        //public virtual InsuranceType InsuranceType { get; set; }
        public virtual BrokerInsuranceType InsuranceType { get; set; }
        [DisplayName("Insurance Sub Type")]

        public int? InsuranceSubTypeID { get; set; }

        [ForeignKey("InsuranceSubTypeID")]
        // public virtual InsuranceSubType InsuranceSubType { get; set; }
        public virtual BrokerSubInsuranceType InsuranceSubType { get; set; }
        [DisplayName("Underwriter")]
        public int? UnderwriterId { get; set; }
        [ForeignKey("UnderwriterId")]
        public virtual Underwriter Underwriter { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; } = BrokerStatus.Active.ToString();
        public DateTime? ContractMaturityDate { get; set; }
    }

}
