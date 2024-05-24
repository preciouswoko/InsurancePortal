using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceCore.Models
{
    public struct Permissions
    {
        public const string INR = "Initiate New Request";
        public const string AUR = "Authorize Request";
        public const string ANU = "Assign Underwriter";
        public const string UIC = "Upload Insurance Certificate";
        public const string RIC = "Review Insurance Certificate";
        public const string ACI = "Assign Contract ID";
        public const string GAI = "Get All Insurance Request";
        public const string LOB = "List Of Broker";
        public const string LOU = "List Of UnderWrite";
        public const string LOI = "List Of InsuranceType";
        public const string LIS = "List Of InsuranceSubType";
        public const string GAR = "Get All Request";
        public const string LBI = "List Of BrokerInsuranceType";
        public const string LBS = "List Of BrokerInsuranceSubType";
        public const string VWF = "Can View Features";
    }
}
