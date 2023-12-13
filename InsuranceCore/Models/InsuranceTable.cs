using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.Models
{
    public class InsuranceTable
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(15)]
        [Column(TypeName = "varchar(20)")]
        public string RequestID { get; set; }

        [Required]
        public int Serial { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Stage { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string RequestType{ get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string RequestByUsername { get; set; }
        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string RequestByName { get; set; }
        [Required]
        [StringLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string RequestByemail { get; set; }
        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string ToBeAuthroiziedBy { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string AuthorizedByUsername { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string AuthorizedByName { get; set; }
        [StringLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string AuthorizedByEmail { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string PolicyNo { get; set; }
        public DateTime? PolicyIssuanceDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string PolicyCertificate { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime CertificateRequestDate { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string CertificateRequestByUsername { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string CertificateRequestByName { get; set; }

        [StringLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string CertificateRequestByemail { get; set; }
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string CertificateToBeAuthroiziedBy { get; set; }

        public DateTime? CertificateAuthorizedDate { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string CertificateAuthorizedByUsername { get; set; }
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string CertificateAuthorizedByName { get; set; }
        [StringLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string CertificateAuthorizedByEmail { get; set; }



        [StringLength(25)]
        [Column(TypeName = "varchar(25)")]
        public string FEESFTReference { get; set; }
        [StringLength(25)]
        [Column(TypeName = "varchar(25)")]
        public string COMMFTReference { get; set; }
        public string ErrorMessage { get; set; }
        public string Status { get; set; }
        public ICollection<FundTransferLookUp> fundTransferLookUps { get; set; }

    }
}
