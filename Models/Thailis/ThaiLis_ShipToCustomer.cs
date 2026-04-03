using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lissystem.Models.Thailis
{
    public class ThaiLis_ShipToCustomer
    {
        [Key]
        public int ship_to { get; set; }
        [MaxLength(30)]
        public string customer_name { get; set; }
        [MaxLength(35)]
        public string address1 { get; set; }
        [MaxLength(35)]
        public string address2 { get; set; }
        [MaxLength(35)]
        public string address3 { get; set; }
        [MaxLength(3)]
        public string destination_code { get; set; }
        [MaxLength(10)]
        public string country_code { get; set; }
        [MaxLength(1)]
        public string minimum_grade { get; set; }
        public DateTime create_dt { get; set; }
        public DateTime last_maint { get; set; }
        [MaxLength(20)]
        public string shipment_expedite_lead { get; set; }
    }
}