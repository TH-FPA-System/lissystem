using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Form
{
    public class RoleModules
    {
        public string role_id { get; set; }
        public Guid module_id { get; set; }
        public string module_section { get; set; }
        public string module_description { get; set; }
        public bool? canaccess { get; set; }
        public bool? canwrite { get; set; }
        public bool? candelete { get; set; }
        public bool? disabled { get; set; }
        public bool IsGranted { get; set; }
    }

}