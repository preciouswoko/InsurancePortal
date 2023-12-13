using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.Models
{
    public class Comments
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(15)]
        [Column(TypeName = "varchar(15)")]
        public string RequestID { get; set; }

        [Required]
        public int Serial { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Action { get; set; }

        [Required]
        [StringLength(2000)]
        [Column(TypeName = "varchar(2000)")]
        public string Comment { get; set; }

        [Required]
        public DateTime CommentDate { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string CommentBy { get; set; }
    }
}
