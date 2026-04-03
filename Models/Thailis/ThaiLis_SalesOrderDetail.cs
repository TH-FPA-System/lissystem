using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models.Thailis
{
    public class ThaiLis_SalesOrderDetail
    {
        [Key]
        [Column(Order = 0)]
        public int order_no { get; set; }
        [Key]
        [Column(Order = 1)]
        public int line_no { get; set; }
        [Key]
        [Column(Order = 2), MaxLength(2)]
        public string order_type { get; set; }
        [Key]
        [Column(Order = 3), MaxLength(5)]
        public string order_company { get; set; }
        [Key]
        [Column(Order = 4)]
        public short split_no { get; set; }
        [MaxLength(2)]
        public string line_type { get; set; }
        [MaxLength(16)]
        public string part { get; set; }
        public DateTime date_reqd { get; set; }
        public int qty_reqd { get; set; }
        public int qty_in_progress { get; set; }
        public int qty_routed { get; set; }
        public int ship_to { get; set; }
        public int shipment_no { get; set; }
        public short sch_priority { get; set; }
        [MaxLength(1)]
        public string sch_status { get; set; }
        public short route_priority { get; set; }
        [MaxLength(1)]
        public string route_status { get; set; }
        [MaxLength(16)]
        public string serial { get; set; }
        public DateTime last_maint { get; set; }
        [MaxLength(12)]
        public string last_maint_logon { get; set; }
    }

}