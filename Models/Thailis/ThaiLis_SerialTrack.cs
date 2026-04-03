using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models.Thailis
{
    public class ThaiLis_SerialTrack
    {
        [Required]
        [Key, Column(Order = 0)]
        public string serial { get; set; }
        [Required]
        [Key, Column(Order = 1)]
        public string part { get; set; }
        public int workcell { get; set; }
        public short task { get; set; }
        [Required]
        public string store_location { get; set; }
        public string status { get; set; }
        public string grade { get; set; }
        public DateTime last_maint { get; set; }
        public string last_maint_logon { get; set; }
        public string part_issue { get; set; }
        public DateTime serial_issue_date { get; set; }
        public int zone { get; set; }
        public int store { get; set; }
        public string update_reference { get; set; }
    }
}