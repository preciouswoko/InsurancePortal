using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class LockResponse
    {
        public string ACCOUNT_NUMBER { get; set; }
        public string DESCRIPTION { get; set; }
        public string FROM_DATE { get; set; }
        public string LOCKED_AMOUNT { get; set; }
        public string CURR_NO { get; set; }
        public string INPUTTER { get; set; }
        public string DATE_TIME { get; set; }
        public string AUTHORISER { get; set; }
        public string CO_CODE { get; set; }
        public string DEPT_CODE { get; set; }
    }

}
