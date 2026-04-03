using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class inventory_category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int inv_category_id { get; set; }
        [Required, MaxLength(64)]
        public string inv_category_name { get; set; }
        [MaxLength(32)]
        public string inv_category_icon { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int order_no { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? updated_by { get; set; }

        public virtual ICollection<inventory_subcategory> Subcategories { get; set; }
    }
}