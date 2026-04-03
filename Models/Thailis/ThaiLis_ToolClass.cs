using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lissystem.Models.Thailis
{
    public class ThaiLis_ToolClass
    {
        [Key]
        [MaxLength(10)]
        public string tool_class { get; set; }
        [MaxLength(32)]
        public string tool_class_desc { get; set; }
    }
}