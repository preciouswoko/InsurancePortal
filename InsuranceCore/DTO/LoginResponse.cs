using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceCore.DTO
{
    public class LoginResponse
    {
        public string username { get; set; }
        public string name { get; set; }
        public string staffid { get; set; }
        public string branchcodes { get; set; }
        public List<string> branches { get; set; }
        public string features { get; set; }
        public List<string> featurelist { get; set; }
        public object unit { get; set; }
        public object divisioncode { get; set; }
        public object division { get; set; }
        public object directorate { get; set; }
        public object grade { get; set; }
        public string email { get; set; }
        public object jobfunction { get; set; }
        public object jobfunctiontier { get; set; }
        public object jobcategory { get; set; }
        public object joblevel { get; set; }
        public object region { get; set; }
        public object regioncode { get; set; }
        public object department { get; set; }

    }
}
