using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.Models
{
    public class AuthResponse
    {
        public string username { get; set; }
        public string name { get; set; }
        public string[] branches { get; set; }
        public string[] featurelist { get; set; }
        public string unit { get; set; }
        public string division { get; set; }
        public string directorate { get; set; }
        public string grade { get; set; }
        public string email { get; set; }
        public string jobfunction { get; set; }
    }
}
