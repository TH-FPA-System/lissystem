using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Form.Planner.OrderScheduler
{
    public class OrderToGoAdjustmentRequest
    {
        public OrderSchForm orderSchForm { get; set; }
        public ToGoAdjustment_Order[] data { get; set; }
    }

    public class ToGoAdjustment_Order
    {
        public int order_no { get; set; }
        public int line_no { get; set; }
        public int split_no { get; set; }
        public int qty_reqd { get; set; }
        public int qty_inprogress { get; set; }
        public int qty_togo { get; set; }
    }
}