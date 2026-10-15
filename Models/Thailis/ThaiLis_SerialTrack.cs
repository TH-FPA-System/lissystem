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
        [MaxLength(16)]
        public string serial { get; set; }
        [Required]
        [Key, Column(Order = 1)]
        [MaxLength(16)]
        public string part { get; set; }
        public int workcell { get; set; }
        public short task { get; set; }
        [Required]
        [MaxLength(12)]
        public string store_location { get; set; }
        [MaxLength(1)]
        public string status { get; set; }
        [MaxLength(1)]
        public string grade { get; set; }
        public DateTime last_maint { get; set; }
        [MaxLength(16)]
        public string last_maint_logon { get; set; }
        [MaxLength(2)]
        public string part_issue { get; set; }
        public DateTime serial_issue_date { get; set; }
        public int zone { get; set; }
        public int store { get; set; }
        [MaxLength(16)]
        public string update_reference { get; set; }
    }
}