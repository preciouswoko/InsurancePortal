using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceCore.Models
{
    public class GatewayLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime APIDatetime { get; set; }
        public string Source { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string TransReference { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
        public string Endpoint { get; set; }
        public string APIType { get; set; }
    }
}
