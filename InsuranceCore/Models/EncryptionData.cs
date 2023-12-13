using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceCore.Models
{
    public class EncryptionData
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Iv { get; set; }

    }
}
