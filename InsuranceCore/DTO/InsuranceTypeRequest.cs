using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class InsuranceTypeRequest
    {
        public string Status { get; set; }
        public string InsuranceType { get; set; }
        public int Percentage  { get; set; }
        public string InsuranceSubType { get; set; }

    }
}
