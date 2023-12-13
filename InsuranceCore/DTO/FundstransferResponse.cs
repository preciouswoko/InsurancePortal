using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class FundstransferResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ReponseReference { get; set; }
        public string TransactionReference { get; set; }
        public bool Status { get; set; }

    }
}
