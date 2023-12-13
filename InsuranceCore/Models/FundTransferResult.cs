using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.Models
{
    public class FundTransferResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Request { get; set; }
        public string Stage { get; set; }
    }
}
