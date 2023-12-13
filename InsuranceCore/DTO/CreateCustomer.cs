using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InsuranceCore.DTO
{
    public class CreateCustomer
    {
        public string EmailAddress { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        [DisplayName("Company")]
        public string Companyattach { get; set; }
    }
}
