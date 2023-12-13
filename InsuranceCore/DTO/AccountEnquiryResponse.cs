using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class AccountEnquiryResponse
    {
        public decimal AccountBalance { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
    }

}
