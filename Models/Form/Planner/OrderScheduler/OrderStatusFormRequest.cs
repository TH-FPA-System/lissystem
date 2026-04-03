using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Models.Form.Planner.OrderScheduler
{
    public class OrderStatusRequest
    {
        public Order_Status[] data { get; set; }

        public List<SelectListItem> statusSelectListItem
        {
            get
            {
                return LISGlobal.SaleOrderStatusSelectList;
            }
        }
    }

    public class Order_Status
    {
        public int order_no { get; set; }
        public int line_no { get; set; }
        public int split_no { get; set; }
        public int qty_reqd { get; set; }
        public int qty_togo { get; set; }
        public string status { get; set; }
    }
}