using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class TransactionLog
    {
        public string OriginatorAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public CreditLog CreditAccounts { get; set; } 
        public string TransactionRef { get; set; }
        public string T24Reference { get; set; }
        public string FTReferences { get; set; }
    }
    public class CreditLog
    {
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public string Currency { get; set; } 
    }

}
