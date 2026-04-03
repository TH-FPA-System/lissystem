using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Authentication
{
    public class LISUserAccount_Validate
    {
        public int code { get; set; }
        public string status { get; set; }
        public Guid? account_id { get; set; }
    }
}