using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class TransferResponse
    {
        public string OurTransactionRef { get; set; }
        public string TransactionRef { get; set; }
        public string TransferStatus { get; set; }
        public string FtReferences { get; set; }
    }
}
