using lissystem.App_Attributes;
using lissystem.App_Interface;
using lissystem.App_Services;
using lissystem.Models.Form.Planner.OrderScheduler;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers.PlannerPanel
{
    [LISAuth, RoutePrefix("cpanel/planner/route-management")]
    public class PlannerRouteManagementController : LISController
    {
        private ThaiLisOrderService orderService;

        public PlannerRouteManagementController()
        {
            orderService = new ThaiLisOrderService();
        }

        /// <summary>
        /// First page view.
        /// </summary>
        /// <returns>VIEW</returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Index()
        {
            OrderSchForm model = new OrderSchForm();

            var productClass = await BaseDB.ProductClasses.ToListAsync();
            var toolClass = await ThailisDB.ToolClasses.ToListAsync();
            var shipToCustomer = await ThailisDB.ShipToCustomers.ToListAsync();

            model.ProductClasses = productClass.Select(p => new SelectListItem { Value = p.product_class, Text = p.description }).ToList();
            model.ToolClasses = toolClass.Select(p => new SelectListItem { Value = p.tool_class, Text = p.tool_class_desc }).ToList();
            model.ShipToCustomers = shipToCustomer.Select(p => new SelectListItem
            {
                Value = p.ship_to.ToString(),
                Text = string.Format("({0}) {1} - {2}", p.ship_to, p.destination_code, p.customer_name)
            }).ToList();

            return View(model);
        }

        /// <summary>
        /// Partial form to support order ToGo adjustment (Request with AJAX).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, LISAuth(RequiredUpdatePermission = true)]
        [Route("form/order-togo/adjustment"), LISAjaxRequest]
        public async Task<ActionResult> FormOrderToGoAdjustmentAction(OrderSplitFormRequest model)
        {
            var requestSplits = model.data.ToList().Distinct();
            var orderSelected = await orderService.GetByOrderRouteSplitRequest(requestSplits, false);

            var formData = new OrderToGoAdjustmentRequest();
            formData.data = orderSelected.Select(p => new ToGoAdjustment_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no,
                qty_reqd = p.qty_reqd,
                qty_inprogress = p.qty_routed,
                qty_togo = p.qty_reqd - p.qty_routed
            }).ToArray();
            return PartialView(formData);
        }

        /// <summary>
        /// Partial form to support order priority update (Request with AJAX).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, LISAuth(RequiredUpdatePermission = true)]
        [Route("form/order-priority"), LISAjaxRequest]
        public async Task<ActionResult> FormOrderPriorityAction(OrderSplitFormRequest model)
        {
            var requestSplits = model.data.ToList().Distinct();
            var orderSelected = await orderService.GetByOrderRouteSplitRequest(requestSplits, false);

            var formData = new OrderPriorityRequest();
            formData.data = orderSelected.Select(p => new Order_Priority
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no,
                qty_reqd = p.qty_reqd,
                qty_togo = p.qty_reqd - p.qty_routed,
                priority = p.route_priority
            }).ToArray();
            return PartialView(formData);
        }

        /// <summary>
        /// Partial form to support order status update (Request with AJAX).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, LISAuth(RequiredUpdatePermission = true)]
        [Route("form/order-status"), LISAjaxRequest]
        public async Task<ActionResult> FormOrderStatusAction(OrderSplitFormRequest model)
        {
            var requestSplits = model.data.ToList().Distinct();
            var orderSelected = await orderService.GetByOrderRouteSplitRequest(requestSplits, false);

            var formData = new OrderStatusRequest();

            formData.data = orderSelected.Select(p => new Order_Status
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no,
                qty_reqd = p.qty_reqd,
                qty_togo = p.qty_reqd - p.qty_routed,
                status = p.sch_status
            }).ToArray();
            return PartialView(formData);
        }

        /// <summary>
        /// Partial form to support order allocate serials (Request with AJAX).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, LISAuth(RequiredUpdatePermission = true)]
        [Route("form/order-allocate"), LISAjaxRequest]
        public async Task<ActionResult> FormOrderAllocateAction(OrderSplitFormRequest model)
        {
            var requestSplits = model.data.ToList().Distinct();
            var orderSelected = await orderService.GetByOrderRouteSplitRequest(requestSplits, false);

            var formData = new OrderAllocateRequest();

            formData.data = orderSelected.Select(p => new Allocate_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no,
                qty_reqd = p.qty_reqd,
                qty_inprogress = p.qty_reqd - p.qty_routed,
                serial = p.serial
            }).ToArray();
            return PartialView(formData);
        }

        /******************************************************************
         * Ajax to support DataTables
         ******************************************************************/

        /// <summary>
        /// Data list support DataTable (Ajax request).
        /// </summary>
        /// <returns>JSON</returns>
        [HttpGet]
        [Route("ajax/data/list"), LISAjaxRequest]
        public async Task<JsonResult> AjaxDataList()
        {
            // Request parameters
            NameValueCollection requestQuery = Request.QueryString;
            long requestIdRaw = long.TryParse(requestQuery.Get("_"), out long idraw) ? idraw : 0;
            int requestOrderNo = int.TryParse(requestQuery.Get("orderno"), out int order_no) ? order_no : int.MinValue;
            string requestPartClass = requestQuery.Get("partclass") ?? string.Empty;
            string requestToolClass = requestQuery.Get("toolclass") ?? string.Empty;
            int requestShipToNo = int.TryParse(requestQuery.Get("shipto"), out int shiptoNo) ? shiptoNo : int.MinValue;
            string requestStatus = requestQuery.Get("status") ?? string.Empty;
            bool requestOnlyNotCompleted = bool.TryParse(requestQuery.Get("only_notcompleted"), out bool onlyNotCompleted) ? onlyNotCompleted : true;
            DateTime requestBeginDate = DateTime.TryParse(requestQuery.Get("begindate"), out DateTime beginDate) ? beginDate : new DateTime(2000, 1, 1);
            DateTime requestEndDate = DateTime.TryParse(requestQuery.Get("enddate"), out DateTime endDate) ? endDate : LISGlobal.LISMaxDate;

            // Get order list.
            var data = await orderService.AllRouteVisibility(
                requestOrderNo,
                requestPartClass,
                requestToolClass,
                requestShipToNo,
                requestStatus,
                onlyNotCompleted,
                requestBeginDate,
                requestEndDate
                );

            return JsonForDataTable(requestIdRaw, data, data.Count());
        }

        /// <summary>
        /// Adjustment the order ToGo quantity request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON Result</returns>
        [HttpPost, LISAuth(RequiredUpdatePermission = true)]
        [ValidateAntiForgeryToken]
        [Route("request/order-togo/adjustment"), LISNoCache, LISAjaxRequest]
        public async Task<ActionResult> RequestOrderToGoAdjustment(OrderToGoAdjustmentRequest model)
        {
            SetPreventBackButton();
            var requestOrderToAdjustment = model.data.ToList().Distinct();
            if (requestOrderToAdjustment.Count() == 0)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid request parameter", System.Net.HttpStatusCode.NotAcceptable);
            }

            var adjustment = await orderService.ToGoAdjustmentOrderRouteRequest(requestOrderToAdjustment);
            if (adjustment)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            }
            else
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "", System.Net.HttpStatusCode.NotModified);
            }
        }

        /// <summary>
        /// Update the order priority request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON Result</returns>
        [HttpPost, LISAuth(RequiredUpdatePermission = true)]
        [ValidateAntiForgeryToken]
        [Route("request/order-priority"), LISNoCache, LISAjaxRequest]
        public async Task<ActionResult> RequestOrderPriorityUpdate(OrderPriorityRequest model)
        {
            SetPreventBackButton();
            var requestOrderToAdjustment = model.data.ToList().Distinct();
            if (requestOrderToAdjustment.Count() == 0)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid request parameter", System.Net.HttpStatusCode.NotAcceptable);
            }

            var update = await orderService.UpdateOrderRoutePriorityRequest(requestOrderToAdjustment);
            if (update)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            }
            else
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "", System.Net.HttpStatusCode.NotModified);
            }
        }

        /// <summary>
        /// Update the order status request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON Result</returns>
        [HttpPost, LISAuth(RequiredUpdatePermission = true)]
        [ValidateAntiForgeryToken]
        [Route("request/order-status"), LISNoCache, LISAjaxRequest]
        public async Task<ActionResult> RequestOrderStatusUpdate(OrderStatusRequest model)
        {
            SetPreventBackButton();
            var requestOrderToUpdate = model.data.ToList().Distinct();
            if (requestOrderToUpdate.Count() == 0)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid request parameter", System.Net.HttpStatusCode.NotAcceptable);
            }

            var update = await orderService.UpdateOrderRouteStatusRequest(requestOrderToUpdate);
            if (update)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            }
            else
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "", System.Net.HttpStatusCode.NotModified);
            }
        }

        /// <summary>
        /// Update the order allocate request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON Result</returns>
        [HttpPost, LISAuth(RequiredUpdatePermission = true)]
        [ValidateAntiForgeryToken]
        [Route("request/order-allocate"), LISNoCache, LISAjaxRequest]
        public async Task<ActionResult> RequestOrderAllocateUpdate(OrderAllocateRequest model)
        {
            SetPreventBackButton();
            var requestOrderToUpdate = model.data.ToList().Distinct();
            if (requestOrderToUpdate.Count() == 0)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid request parameter", System.Net.HttpStatusCode.NotAcceptable);
            }

            var update = await orderService.AllocateOrderRouteRequest(requestOrderToUpdate);
            if (update)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            }
            else
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "", System.Net.HttpStatusCode.NotModified);
            }
        }
    }
}