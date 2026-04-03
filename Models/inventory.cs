using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid inv_id { get; set; }
        public int inv_category_id { get; set; }
        public int inv_subcategory_id { get; set; }
        [MaxLength(32)]
        public string brand { get; set; }
        [MaxLength(64)]
        public string model { get; set; }
        [MaxLength(64)]
        public string serial_no { get; set; }
        [MaxLength(15)]
        public string ip_address { get; set; }
        [MaxLength(17)]
        public string mac_address { get; set; }
        [MaxLength(32)]
        public string device_name { get; set; }
        [MaxLength(64)]
        public string mer_no { get; set; }
        [MaxLength(64)]
        public string po_no { get; set; }
        [MaxLength(64)]
        public string invoice_no { get; set; }
        [MaxLength(32)]
        public string asset_no { get; set; }
        public bool onhand { get; set; } = true;
        [MaxLength(128)]
        public string location { get; set; }
        [MaxLength(128)]
        public string location_area { get; set; }
        public DateTime? received_at { get; set; }
        public DateTime? installation_at { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? created_at { get; set; }
        public Guid? created_by { get; set; }
        public DateTime? updated_at { get; set; }
        public Guid? updated_by { get; set; }
    }
}