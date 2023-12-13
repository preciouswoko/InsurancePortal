using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceCore.Models
{
    public class AdminAuditLogs
    {
        [Key]
        public long Id { get; set; }
        public string UserId { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }

        public DateTime ExecutionTime { get; set; }

        public string ClientIpAddress { get; set; }

        public string Exception { get; set; }

        public string BrowserInfo { get; set; }

        public bool Status { get; set; }

        public Guid AuditStatusId { get; set; }
    }
}
