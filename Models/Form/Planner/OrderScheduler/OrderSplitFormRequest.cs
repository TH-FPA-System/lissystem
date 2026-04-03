using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Form.Planner.OrderScheduler
{
    public class OrderSplitFormRequest
    {
        public OrderSplit_Order[] data { get; set; }
    }

    public class OrderSplit_Order
    {
        public int order_no { get; set; }
        public int line_no { get; set; }
        public int split_no { get; set; }
        public int qty_reqd { get; set; }
        public int qty_togo { get; set; }
        public int split_batch_count { get; set; } = 1;
    }
}