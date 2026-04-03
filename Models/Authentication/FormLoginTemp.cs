using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Authentication
{
    public class FormLoginTemp
    {
        public bool Success { get; set; }
        public LISGlobal.LIS_LOGIN_ERROR ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}