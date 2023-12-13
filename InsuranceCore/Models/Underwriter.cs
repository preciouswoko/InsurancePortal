using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceCore.Models
{
    public class Underwriter
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Underwriter Name")]
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Status { get; set; }
        [DisplayName("Broker")]
        public int BrokerId { get; set; }

        public virtual Broker Broker { get; set; }
      //  public ICollection<InsuranceRequest> InsuranceRequest { get; set; }

    }
}
