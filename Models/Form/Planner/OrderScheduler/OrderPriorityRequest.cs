using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Form.Planner.OrderScheduler
{
    public class OrderPriorityRequest
    {
        public Order_Priority[] data { get; set; }
    }

    public class Order_Priority
    {
        public int order_no { get; set; }
        public int line_no { get; set; }
        public int split_no { get; set; }
        public int qty_reqd { get; set; }
        public int qty_togo { get; set; }
        public short priority { get; set; } = 10;
    }
}