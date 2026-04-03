using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models.Thailis
{
    public class ThaiLis_Part
    {
        [Key]
        [MaxLength(16)]
        public string part { get; set; }
        [MaxLength(32)]
        public string description { get; set; }
        [MaxLength(2)]
        public string unit_measure { get; set; }
        [MaxLength(16)]
        [Column("class")]
        public string part_class { get; set; }
        [MaxLength(2)]
        public string primary_role { get; set; }
        public DateTime date_added { get; set; }
    }
}