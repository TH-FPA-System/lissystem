using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Form.Planner.OrderScheduler
{
    public class OrderCancelFormRequest
    {
        public Order_Cancel[] data { get; set; }
    }

    public class Order_Cancel
    {
        public int order_no { get; set; }
        public int line_no { get; set; }
        public int split_no { get; set; }
        public int qty_reqd { get; set; }
        public int qty_togo { get; set; }
    }
}