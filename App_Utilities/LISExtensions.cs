using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.App_Utilities
{
    public static class LISExtensions
    {
        public static string SafeSubString(this string value, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (startIndex >= value.Length) return string.Empty;
            return value.Substring(startIndex, Math.Min(length, value.Length - startIndex));
        }
    }
}