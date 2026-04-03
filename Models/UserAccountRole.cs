using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class UserAccountRole
    {
        [Key]
        [MaxLength(4)]
        public string role_id { get; set; }
        [Required, MaxLength(32)]
        public string description { get; set; }
        public bool disabled { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool allow_update { get; set; } = true;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }

        public virtual ICollection<UserAccountInformation> Users { get; set; }

        public UserAccountRole() {
            Users = new List<UserAccountInformation>();
        }
    }
}