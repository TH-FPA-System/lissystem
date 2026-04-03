using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace lissystem.Models
{
    public class UserAccountInformation
    {
        [Key]
        [ForeignKey("UserAccount")]
        public Guid account_id { get; set; }
        [MaxLength(12)]
        public string code { get; set; }
        [MaxLength(64)]
        public string display_name { get; set; }
        [MaxLength(4)]
        public string role_id { get; set; }
        public int section_id { get; set; }
        public bool disabled { get; set; }
        public bool canlogin { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? updated_by { get; set; }

        public virtual UserAccount UserAccount { get; set; }

        [ForeignKey("role_id")]
        public virtual UserAccountRole Role { get; set; }
    }
}