using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Models.Form.Planner.OrderScheduler
{
    public class OrderSchForm
    {
        public string filterOrderNo { get; set; }
        public string filterProductClass { get; set; }
        public string filterToolClass { get; set; }
        public string filterShipTo { get; set; }
        public string filterStatus { get; set; }
        public bool filterOnlyNotCompleted { get; set; } = true;
        public DateTime beginRequestDate { get; set; }
        public DateTime endRequestDate { get; set; }

        public List<SelectListItem> ProductClasses { get; set; }
        public List<SelectListItem> ToolClasses { get; set; }
        public List<SelectListItem> ShipToCustomers { get; set; }
        public List<SelectListItem> OrderStatus
        {
            get
            {
                return LISGlobal.SaleOrderStatusSelectList;
            }
        }
    }
}