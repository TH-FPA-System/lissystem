using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class UserAccountSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int session_id { get; set; }
        public Guid account_id { get; set; }
        [MaxLength(32)]
        public string host_name { get; set; }
        [MaxLength(15)]
        public string ip_address { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? expires_at { get; set; }
        public string engine { get; set; }
    }
}