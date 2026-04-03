using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class inventory_subcategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int inv_subcategory_id { get; set; }
        public int inv_category_id { get; set; }
        [Required, MaxLength(64)]
        public string inv_subcategory_name { get; set; }
        [MaxLength(32)]
        public string inv_subcategory_icon { get; set; }
        public int safety_stock { get; set; } = 0;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int order_no { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? updated_by { get; set; }

        public virtual inventory_category Category { get; set; }
    }
}