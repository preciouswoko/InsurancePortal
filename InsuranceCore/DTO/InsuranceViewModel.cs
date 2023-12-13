using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InsuranceCore.DTO
{
    public class InsuranceViewModel
    {
        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public string CustomerName { get; set; }

        public string CustomerID { get; set; }

        public string CustomerEmail { get; set; }
        [DisplayName("Broker")]
        public int BrokerId { get; set; }
        [DisplayName("CollateralValueFSV")]
        public decimal CollateralValue { get; set; }

        public decimal? EstimatedPremium { get; set; }
        [DisplayName("Insurance Type")]

        public int? InsuranceTypeId { get; set; }
        [DisplayName("Insurance SubType")]

        public int? InsuranceSubTypeId { get; set; }
        public string  Stage { get; set; }

    }
}
