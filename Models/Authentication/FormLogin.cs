using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Authentication
{
    public class FormLogin
    {
        public string account_name { get; set; }
        public string password { get; set; }
        public bool rememberMe { get; set; }
        public string RedirectToUrl { get; set; }
    }
}