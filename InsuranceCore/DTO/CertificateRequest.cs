using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceCore.DTO
{
    public class CertificateRequest
    {
        public long insuranceId { get; set; }
        public string PolicyNumber { get; set; }

        [Display(Name = "Date of Issuance")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateofIssuance { get; set; } = DateTime.Now;

        [Display(Name = "Expiry Date of Certificate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpiryDate { get; set; }= DateTime.Now;


        public IFormFile Certificatefile { get; set; }
    }
}
