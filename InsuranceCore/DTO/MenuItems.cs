using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.DTO
{
    public class MenuItems
    {
        public MenuItems(string code, string desc, string group, string controller, string action, string parameter, int sequence)
        {
            Code = code;
            Description = desc;
            Group = group;
            Controller = controller;
            Action = action;
            Parameter = parameter;
            Sequence = sequence;
        }

        public string Code { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Parameter { get; set; }
        public int Sequence { get; set; }
    }
}
