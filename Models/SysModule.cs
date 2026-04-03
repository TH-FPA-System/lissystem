using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class SysModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid module_id { get; set; }
        public Guid? parent_module_id { get; set; }
        [MaxLength(64)]
        public string module_section { get; set; }
        [MaxLength(64)]
        public string submodule_section { get; set; }
        [MaxLength(64)]
        public string controller_name { get; set; }
        [MaxLength(64)]
        public string action_name { get; set; }
        [MaxLength(32)]
        public string module_name { get; set; }
        [MaxLength(250)]
        public string module_description { get; set; }
        public bool disabled { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? updated_by { get;set; }
    }
}