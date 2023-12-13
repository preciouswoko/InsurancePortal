using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.Models
{
    public class FundTransferLookUp
    {
        [Key]
        public int Id { get; set; }
        public string TransactionNarration { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionRequest { get; set; }
        public string TransactionResponse{ get; set; }

        public DateTime? RequesstDate { get; set; }
        public string UniqueID { get; set; }
        public string TransactionType { get; set; }
       
        public string RequestID { get; set; }
        public long? InsuranceTableId { get; set; }

        [ForeignKey("InsuranceTableId")]
        public virtual InsuranceTable InsuranceTable { get; set; }
    }
}
