using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class UserAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid account_id { get; set; }
        public string account_name { get; set; }
        [NotMapped]
        public string password { get; set; }
        public DateTime password_expires { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? updated_by { get; set; }
        public bool deleted { get; set; }
        public DateTime? deleted_at { get; set; }
        public Guid? deleted_by { get; set; }

        public virtual UserAccountInformation Information { get; set; }
    }
}