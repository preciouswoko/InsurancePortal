using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceInfrastructure.Helpers
{
    public class Utilities
    {

        public static string FormatAmount(decimal? amount = 0)
        {
            if (amount == null || amount == 0) return string.Empty;
           return String.Format("{0:n}", amount);
        }

    }
}
