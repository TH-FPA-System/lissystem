using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models.Thailis
{
    public class ThaiLis_PartToolClass
    {
        [Key, Column(Order = 1)]
        [MaxLength(16)]
        public string part { get; set; }
        [Key, Column(Order = 0)]
        [MaxLength(10)]
        public string tool_class { get; set; }
        [MaxLength(1)]
        public string disabled_part_flag { get; set; }
    }
}