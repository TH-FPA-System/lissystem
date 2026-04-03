using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Data
{
    public class DataSchduleOrderVisibility
    {
        public int order_no { get; set; }
        public int line_no { get; set; }
        public int split_no { get; set; }
        public string order_type { get; set; }
        public string line_type { get; set; }
        public string part { get; set; }
        public string part_class { get; set; }
        public string description { get; set; }
        public DateTime date_reqd { get; set; }
        public int sch_priority { get; set; }
        public string sch_status { get; set; }
        public int route_priority { get; set; }
        public string route_status { get; set; }
        public int qty_reqd { get; set; }
        public int qty_in_progress { get; set; }
        public int qty_routed { get; set; }
        public string destination_code { get; set; }
        public int ship_to { get; set; }
        public int shipment_no { get; set; }
        public string serial { get; set; }
        public string order_company { get; set; }
        public string last_maint_logon { get; set; }


        public int sch_qty_togo
        {
            get { return qty_reqd - qty_in_progress; }
        }

        public int route_qty_togo
        {
            get { return qty_reqd - qty_routed; }
        }

        public string ship_mode
        {
            get { return last_maint_logon.EndsWith(",AIR") ? "AIR" : ""; }
        }

        public string serial_allocated
        {
            get { return !string.IsNullOrEmpty(serial) && serial.Length == 9 ? "Y" : "N"; }
        }

        public string order_canceled
        { 
            get { return serial == "CANCELED" ? "Y" : "N"; }
        }
    }
}