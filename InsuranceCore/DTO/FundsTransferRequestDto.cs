using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceCore.DTO
{
    public class FundsTransferRequestDto
    {
        public FundsTransferRequestDto()
        {
            Charges = new List<ChargesRequestDto>();
        }
        public long Id { get; set; }
        public long InsuranceTableId { get; set; }
        public string TransactionId { get; set; }
        [Required]
        public string DebitAccount { get; set; }
        public string DebitCurrency { get; set; }
       // public DateTime DebitValueDate { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string CreditAccount { get; set; }
       // public DateTime CreditValueDate { get; set; }
        public string CreditCurrency { get; set; }
        [MaxLength(50)]
        public string Narration { get; set; }
        public string Channel { get; set; }
        public string BranchCode { get; set; }
        public List<ChargesRequestDto> Charges { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionRef { get; set; }
        public string TransactionType { get; set; }
        public string UnquieId { get; set; }
        public string RequestID { get; set; }

    }


    public class ChargesRequestDto
    {
        public string ChargeCode { get; set; }
        public decimal Charges { get; set; }
    }
}
