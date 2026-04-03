using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Ajax
{
    public class JsonResultMessage
    {
        public string status { get; set; } = "OK";
        public string message { get; set; }
        public object data { get; set; }
    }
}