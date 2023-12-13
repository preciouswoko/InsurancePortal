using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class InsuranceReportQuery
    {
        public string ContractId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AccountNo { get; set; }
        public string InsuranceFlag { get; set; }
        public DateTime? PolicyExpiryStart { get; set; }
        public DateTime? PolicyExpiryEnd { get; set; }

        public int? BrokerId { get; set; }
        public int? UnderwriterId { get; set; }
    }
}
