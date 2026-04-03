using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class SystemError
    {
        public int errorStatus { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string message { get; set; }
        public string innerMessage { get; set; }
    }
}