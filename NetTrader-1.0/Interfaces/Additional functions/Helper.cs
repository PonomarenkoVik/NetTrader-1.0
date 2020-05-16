using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Additional_functions
{
    public static class Helper
    {
        public static string FormatValue(double val, string currency, int precision = 2)
        {
            double res = Math.Round(val, precision);
            return $"{res} {currency}";
        }
    }
}
