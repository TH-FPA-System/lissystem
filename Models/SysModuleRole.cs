using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class SysModuleRole
    {
        [Key]
        [Column(Order = 1)]
        public string role_id { get; set; }
        [Key]
        [Column(Order = 2)]
        public Guid module_id { get; set; }
        public bool canaccess { get; set; }
        public bool canread { get; set; }
        public bool canwrite { get; set; }
        public bool candelete { get; set; }
        public bool full_control { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? updated_by { get; set; }
    }
}