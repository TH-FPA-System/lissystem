using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class PlannerOrderSplitProdExcept
    {
        [Key, Column(Order = 1)]
        [Required]
        [MaxLength(36)]
        public string product_class { get; set; }
        [Key, Column(Order = 2)]
        [Required]
        [MaxLength(32)]
        public string product_subclass { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }
    }
}