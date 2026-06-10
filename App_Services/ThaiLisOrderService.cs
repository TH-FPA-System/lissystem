using lissystem.App_DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using lissystem.Models.Data;
using System.Data.Entity;
using lissystem.Models.Thailis;
using lissystem.Models.Form.Planner.OrderScheduler;
using System.Data.Entity.Infrastructure;
using lissystem.App_Utilities;
namespace lissystem.App_Services
{
    public class ThaiLisOrderService
    {
        private ThailisDBContext db;
        public ThaiLisOrderService()
        {
            db = LISGlobal.ThailisDB;
        }

        /// <summary>
        /// Get all order visibilities.
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="partClass"></param>
        /// <param name="toolClass"></param>
        /// <param name="shipTo"></param>
        /// <param name="status"></param>
        /// <returns>IEnumerable DataSchduleOrderVisibility</returns>
        public async Task<IEnumerable<DataSchduleOrderVisibility>> AllVisibility(int orderNo = int.MinValue, string partClass = "", string toolClass = "", int shipTo = int.MinValue, string status = "",
            bool onlyNotCompleted = true, DateTime? beginDate = null, DateTime? endDate = null)
        {
            IQueryable<DataSchduleOrderVisibility> query =
                from sod in db.SalesOrderDetails
                    //where (sod.qty_reqd - sod.qty_in_progress) > 0
                join stc in db.ShipToCustomers on sod.ship_to equals stc.ship_to
                join p in db.Parts on sod.part equals p.part
                where db.PartToolClasses.Any(ptc => ptc.part == p.part)
                select new DataSchduleOrderVisibility
                {
                    order_no = sod.order_no,
                    line_no = sod.line_no,
                    split_no = sod.split_no,
                    part = sod.part,
                    part_class = p.part_class,
                    description = p.description,
                    date_reqd = sod.date_reqd,
                    sch_priority = sod.sch_priority,
                    sch_status = sod.sch_status,
                    route_priority = sod.route_priority,
                    route_status = sod.route_status,
                    qty_reqd = sod.qty_reqd,
                    qty_in_progress = sod.qty_in_progress,
                    qty_routed = sod.qty_routed,
                    destination_code = stc.destination_code,
                    ship_to = sod.ship_to,
                    shipment_no = sod.shipment_no,
                    order_type = sod.order_type,
                    line_type = sod.line_type,
                    serial = sod.serial,
                    order_company = sod.order_company,
                    last_maint_logon = sod.last_maint_logon
                };

            // Apply filter only not-completed
            if (onlyNotCompleted)
            {
                query = query.Where(p => (p.qty_reqd - p.qty_in_progress) > 0);
            }
            else
            {
                query = query.Where(p => p.qty_routed < p.qty_reqd || p.serial == "CANCELED");
            }

            // Apply filter if not empty.
            if (orderNo > int.MinValue)
            {
                query = query.Where(p => p.order_no == orderNo);
            }

            if (!string.IsNullOrEmpty(partClass))
            {
                query = query.Where(p => p.part_class == partClass);
            }

            // Apply filter if toolClass not empty.
            if (!string.IsNullOrEmpty(toolClass))
            {
                query = from q in query
                        join ptc in db.PartToolClasses on q.part equals ptc.part
                        where ptc.tool_class == toolClass
                        select q;
            }

            if (shipTo > int.MinValue)
            {
                query = query.Where(p => p.ship_to == shipTo);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.sch_status == status);
            }

            // Apply filter Date request (If not null).
            if (!beginDate.HasValue || beginDate < LISGlobal.LISMinDate)
                beginDate = LISGlobal.LISMinDate;
            if (!endDate.HasValue || endDate >  LISGlobal.LISMaxDate)
                endDate = LISGlobal.LISMaxDate;
            query = query.Where(p => p.date_reqd >= beginDate && p.date_reqd <= endDate);

            query = query.OrderByDescending(p => p.sch_priority).ThenBy(p => p.date_reqd);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Get sale order details from Split order reuqest.
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>IEnumerable ThaiLis_SalesOrderDetail</returns>
        public async Task<IEnumerable<ThaiLis_SalesOrderDetail>> GetByOrderSplitRequest(IEnumerable<OrderSplit_Order> orders, bool onlyNotCompleted = true)
        {
            var keyList = string.Join(", ", orders.Select(p => $"({p.order_no}, {p.line_no}, {p.split_no})").Distinct());
            string rawSQLQuery = $@"
                SELECT
                    sod.order_no, sod.order_type, sod.line_no, sod.order_company, sod.split_no, sod.line_type, 
                    sod.part, sod.date_reqd, sod.qty_reqd, sod.qty_in_progress, sod.qty_routed, sod.ship_to, sod.shipment_no, 
                    sod.sch_priority, sod.sch_status, sod.route_priority, sod.route_status, sod.serial, sod.last_maint, sod.last_maint_logon 
                FROM dbo.sales_order_detail sod
                WHERE
                    {(onlyNotCompleted ? "(sod.qty_reqd - sod.qty_in_progress) > 0 AND " : "")}
                    EXISTS (
                        SELECT 1 FROM (VALUES {keyList}) AS KeysToMatch(OrderNum, LineNum, SplitNum)
                        WHERE 
                            OrderNum = sod.order_no AND LineNum = sod.line_no AND SplitNum = sod.split_no
                    )
                ORDER BY sod.order_no, sod.line_no, sod.split_no ASC
                ";
            var saleOrders = await db.Database.SqlQuery<ThaiLis_SalesOrderDetail>(rawSQLQuery).ToListAsync();
            return saleOrders;
        }

        /// <summary>
        /// Get max split number of order line from Split order request.
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public async Task<List<DataSchduleOrderLineMaxSplitNumber>> GetMaxSplitNoOfOrderLine(IEnumerable<OrderSplit_Order> orders)
        {
            var keyList = string.Join(", ", orders.Select(p => $"({p.order_no}, {p.line_no})").Distinct());
            string rawSQLQuery = $@"
                SELECT
                	sod.order_no, sod.line_no,
                	MAX(sod.split_no) AS split_no
                FROM dbo.sales_order_detail sod
                WHERE
                	EXISTS (
                		SELECT 1 FROM (VALUES {keyList}) AS KeysToMatch(OrderNum, LineNum)
                		WHERE OrderNum = sod.order_no AND LineNum = sod.line_no
                	)
                GROUP BY sod.order_no, sod.line_no
                ";
            var saleOrders = await db.Database.SqlQuery<DataSchduleOrderLineMaxSplitNumber>(rawSQLQuery).ToListAsync();
            return saleOrders;
        }

        /// <summary>
        /// Get max split number of order line.
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <param name="LineNo"></param>
        /// <returns></returns>
        public async Task<short> GetMaxSplitNoOfOrderLine(int OrderNo, int LineNo)
        {
            short maxSplit = await db.SalesOrderDetails
                .Where(p => p.order_no == OrderNo && p.line_no == LineNo)
                .MaxAsync(p => p.split_no);
            return maxSplit;
        }

        /// <summary>
        /// Split orders
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> SplitOrderRequest(IEnumerable<OrderSplit_Order> orders)
        {
            var orderToSplit = await GetByOrderSplitRequest(orders);
            var maxSplitOfOrderLine = await GetMaxSplitNoOfOrderLine(orders);

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;
            List<ThaiLis_SalesOrderDetail> newSplitOrder = new List<ThaiLis_SalesOrderDetail>();

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    bool isOrderInProgress = false;
                    int currentOrderQTYRequest = 0;
                    int currentOrderSplitLoopIndex = 0;
                    short currentBatch = 0;

                    // Start process split the order.
                    foreach (var order in orderToSplit)
                    {
                        int originalOrderRequestQTY = order.qty_reqd;
                        DateTime originalOrderLastMaint = order.last_maint;
                        string originalOrderLastMaintLogon = order.last_maint_logon;
                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        currentOrderSplitLoopIndex = 0;

                        var requestSplit = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        if (requestSplit != null)
                        {
                            int qtyToSplit = requestSplit.split_batch_count;

                            isOrderInProgress = order.qty_in_progress > 0;


                            // Get last split number of current order line.
                            var lastSplit = maxSplitOfOrderLine
                                .Where(p => p.order_no == order.order_no && p.line_no == order.line_no)
                                .FirstOrDefault();

                            if (lastSplit == null)
                            {
                                maxSplitOfOrderLine.Add(new DataSchduleOrderLineMaxSplitNumber
                                {
                                    order_no = order.order_no,
                                    line_no = order.line_no,
                                    split_no = 1
                                });
                                currentBatch = 1;
                            }
                            else
                            {
                                if (lastSplit.split_no < 2)
                                    lastSplit.split_no = 1;
                                else
                                    lastSplit.split_no += 1;
                                currentBatch = lastSplit.split_no;
                            }

                            currentOrderQTYRequest = order.qty_reqd;

                            // Split order batch.
                            while (currentOrderQTYRequest > 0)
                            {
                                int currentBatchQTY = Math.Min(qtyToSplit, currentOrderQTYRequest);

                                if (currentOrderSplitLoopIndex > 0)
                                {
                                    var newBatch = new ThaiLis_SalesOrderDetail
                                    {
                                        order_no = order.order_no,
                                        order_type = order.order_type,
                                        line_no = order.line_no,
                                        order_company = order.order_company,
                                        split_no = currentBatch,
                                        line_type = order.line_type,
                                        part = order.part,
                                        date_reqd = order.date_reqd,
                                        qty_reqd = currentBatchQTY,
                                        qty_in_progress = 0,
                                        qty_routed = 0,
                                        ship_to = order.ship_to,
                                        shipment_no = order.shipment_no,
                                        sch_priority = order.sch_priority,
                                        sch_status = order.sch_status,
                                        route_priority = order.route_priority,
                                        route_status = order.route_status,
                                        serial = string.Empty,
                                        last_maint_logon = actionByName,
                                        last_maint = DateTime.Now,
                                    };

                                    newSplitOrder.Add(newBatch);
                                    currentOrderQTYRequest -= currentBatchQTY;
                                }
                                else
                                {
                                    db.SalesOrderDetails.Attach(order);

                                    if (isOrderInProgress)
                                        order.qty_reqd = Math.Max(currentBatchQTY, order.qty_in_progress);
                                    else
                                        order.qty_reqd = currentBatchQTY;

                                    order.last_maint_logon = actionByName;
                                    order.last_maint = DateTime.Now;

                                    db.UpdateEntityModel(order);
                                    currentOrderQTYRequest -= order.qty_reqd;
                                }


                                currentBatch++;
                                if (currentOrderQTYRequest > 0)
                                {
                                    lastSplit.split_no = currentBatch;
                                    currentOrderSplitLoopIndex++;
                                }
                            }

                            // Store order split history.
                            var splitHistory = new ThaiLis_SalesOrderDetailSplit
                            {
                                order_no = order.order_no,
                                order_type = order.order_type,
                                line_no = order.line_no,
                                order_company = order.order_company,
                                split_no = order.split_no,
                                line_type = order.line_type,
                                part = order.part,
                                date_reqd = order.date_reqd,
                                qty_reqd = originalOrderRequestQTY,
                                qty_in_progress = order.qty_in_progress,
                                qty_routed = order.qty_routed,
                                ship_to = order.ship_to,
                                shipment_no = order.shipment_no,
                                sch_priority = order.sch_priority,
                                sch_status = order.sch_status,
                                route_priority = order.route_priority,
                                route_status = order.route_status,
                                serial = order.serial,
                                last_maint_logon = originalOrderLastMaintLogon,
                                last_maint = originalOrderLastMaint,
                                split_per_batch = qtyToSplit,
                                split_batch_count = currentBatch - 1,
                                split_by = userAccount
                            };
                            db.SalesOrderDetailSplits.Add(splitHistory);

                            currentOrderSplitLoopIndex = 0;
                        }
                    }

                    // Add new order splitted.
                    db.SalesOrderDetails.AddRange(newSplitOrder);
                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update the order ToGo request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> ToGoAdjustmentOrderRequest(IEnumerable<ToGoAdjustment_Order> orders)
        {
            List<OrderSplit_Order> orderSplits = orders.Select(p => new OrderSplit_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no
            }).ToList();

            var orderToAdjustment = await GetByOrderSplitRequest(orderSplits, false);

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Start process the order adjustment.
                    foreach (var order in orderToAdjustment)
                    {
                        var requestAdjustment = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        int newQtyToGo = order.qty_reqd - requestAdjustment.qty_togo;
                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        if (requestAdjustment != null)
                        {
                            db.SalesOrderDetails.Attach(order);
                            order.qty_in_progress = newQtyToGo;
                            order.last_maint_logon = actionByName;
                            order.last_maint = DateTime.Now;

                            db.UpdateEntityModel(order);
                        }
                    }

                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update the order priority request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> UpdateOrderPriorityRequest(IEnumerable<Order_Priority> orders)
        {
            List<OrderSplit_Order> orderSplits = orders.Select(p => new OrderSplit_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no
            }).ToList();

            var orderToAdjustment = await GetByOrderSplitRequest(orderSplits, false);

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Start process the order priority update.
                    foreach (var order in orderToAdjustment)
                    {
                        var requestUpdate = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        if (requestUpdate != null)
                        {
                            db.SalesOrderDetails.Attach(order);
                            order.sch_priority = requestUpdate.priority;
                            order.last_maint_logon = actionByName;
                            order.last_maint = DateTime.Now;

                            db.UpdateEntityModel(order);
                        }
                    }

                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update the order status request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> UpdateOrderStatusRequest(IEnumerable<Order_Status> orders)
        {
            List<OrderSplit_Order> orderSplits = orders.Select(p => new OrderSplit_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no
            }).ToList();

            var orderToAdjustment = await GetByOrderSplitRequest(orderSplits, false);

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Start process the order status update.
                    foreach (var order in orderToAdjustment)
                    {
                        var requestUpdate = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        if (requestUpdate != null)
                        {
                            db.SalesOrderDetails.Attach(order);
                            order.sch_status = requestUpdate.status;
                            order.last_maint_logon = actionByName;
                            order.last_maint = DateTime.Now;

                            db.UpdateEntityModel(order);
                        }
                    }

                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update the order status request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> CancelOrderRequest(IEnumerable<Order_Cancel> orders)
        {
            List<OrderSplit_Order> orderSplits = orders.Select(p => new OrderSplit_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no
            }).ToList();

            var orderToAdjustment = await GetByOrderSplitRequest(orderSplits, false);

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Start process the order status update.
                    foreach (var order in orderToAdjustment)
                    {
                        var requestUpdate = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        if (requestUpdate != null)
                        {
                            db.SalesOrderDetails.Attach(order);
                            order.qty_in_progress = order.qty_reqd;
                            order.qty_routed = order.qty_reqd;
                            order.sch_status = "H";
                            order.route_status = "H";
                            order.serial = "CANCELED";
                            order.last_maint_logon = actionByName;
                            order.last_maint = DateTime.Now;

                            db.UpdateEntityModel(order);
                        }
                    }

                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get all order route visibilities.
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="partClass"></param>
        /// <param name="toolClass"></param>
        /// <param name="shipTo"></param>
        /// <param name="status"></param>
        /// <param name="onlyNotCompleted"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DataSchduleOrderVisibility>> AllRouteVisibility(int orderNo = int.MinValue, string partClass = "", string toolClass = "", int shipTo = int.MinValue, string status = "",
            bool onlyNotCompleted = true, DateTime? beginDate = null, DateTime? endDate = null)
        {
            IQueryable<DataSchduleOrderVisibility> query =
                from sod in db.SalesOrderDetails
                    //where (sod.qty_reqd - sod.qty_routed) > 0
                join stc in db.ShipToCustomers on sod.ship_to equals stc.ship_to
                join p in db.Parts on sod.part equals p.part
                where db.PartToolClasses.Any(ptc => ptc.part == p.part)
                select new DataSchduleOrderVisibility
                {
                    order_no = sod.order_no,
                    line_no = sod.line_no,
                    split_no = sod.split_no,
                    part = sod.part,
                    part_class = p.part_class,
                    description = p.description,
                    date_reqd = sod.date_reqd,
                    sch_priority = sod.sch_priority,
                    sch_status = sod.sch_status,
                    route_priority = sod.route_priority,
                    route_status = sod.route_status,
                    qty_reqd = sod.qty_reqd,
                    qty_in_progress = sod.qty_in_progress,
                    qty_routed = sod.qty_routed,
                    destination_code = stc.destination_code,
                    ship_to = sod.ship_to,
                    shipment_no = sod.shipment_no,
                    order_type = sod.order_type,
                    line_type = sod.line_type,
                    serial = sod.serial,
                    order_company = sod.order_company,
                    last_maint_logon = sod.last_maint_logon
                };

            // Apply filter only not-completed
            if (onlyNotCompleted)
            {
                query = query.Where(p => (p.qty_reqd - p.qty_routed) > 0);
            }
            else
            {
                query = query.Where(p => p.qty_routed < p.qty_reqd);
            }

            // Apply filter if not empty.
            if (orderNo > int.MinValue)
            {
                query = query.Where(p => p.order_no == orderNo);
            }

            if (!string.IsNullOrEmpty(partClass))
            {
                query = query.Where(p => p.part_class == partClass);
            }

            // Apply filter if toolClass not empty.
            if (!string.IsNullOrEmpty(toolClass))
            {
                query = from q in query
                        join ptc in db.PartToolClasses on q.part equals ptc.part
                        where ptc.tool_class == toolClass
                        select q;
            }

            if (shipTo > int.MinValue)
            {
                query = query.Where(p => p.ship_to == shipTo);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.route_status == status);
            }

            // Apply filter Date request (If not null).
            if (!beginDate.HasValue || beginDate < LISGlobal.LISMinDate)
                beginDate = LISGlobal.LISMinDate;
            if (!endDate.HasValue || endDate >  LISGlobal.LISMaxDate)
                endDate = LISGlobal.LISMaxDate;
            query = query.Where(p => p.date_reqd >= beginDate && p.date_reqd <= endDate);

            query = query.OrderByDescending(p => p.sch_priority).ThenBy(p => p.date_reqd);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Get sale order route details from Split order reuqest.
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>IEnumerable ThaiLis_SalesOrderDetail</returns>
        public async Task<IEnumerable<ThaiLis_SalesOrderDetail>> GetByOrderRouteSplitRequest(IEnumerable<OrderSplit_Order> orders, bool onlyNotCompleted = true)
        {
            var keyList = string.Join(", ", orders.Select(p => $"({p.order_no}, {p.line_no}, {p.split_no})").Distinct());
            string rawSQLQuery = $@"
                SELECT
                    sod.order_no, sod.order_type, sod.line_no, sod.order_company, sod.split_no, sod.line_type, 
                    sod.part, sod.date_reqd, sod.qty_reqd, sod.qty_in_progress, sod.qty_routed, sod.ship_to, sod.shipment_no, 
                    sod.sch_priority, sod.sch_status, sod.route_priority, sod.route_status, sod.serial, sod.last_maint, sod.last_maint_logon
                FROM dbo.sales_order_detail sod
                WHERE
                    {(onlyNotCompleted ? "(sod.qty_reqd - sod.qty_routed) > 0 AND " : "")}
                    EXISTS (
                        SELECT 1 FROM (VALUES {keyList}) AS KeysToMatch(OrderNum, LineNum, SplitNum)
                        WHERE 
                            OrderNum = sod.order_no AND LineNum = sod.line_no AND SplitNum = sod.split_no
                    )
                ORDER BY sod.order_no, sod.line_no, sod.split_no ASC
                ";
            var saleOrders = await db.Database.SqlQuery<ThaiLis_SalesOrderDetail>(rawSQLQuery).ToListAsync();
            return saleOrders;
        }

        /// <summary>
        /// Update the order ToGo request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> ToGoAdjustmentOrderRouteRequest(IEnumerable<ToGoAdjustment_Order> orders)
        {
            List<OrderSplit_Order> orderSplits = orders.Select(p => new OrderSplit_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no
            }).ToList();

            var orderToAdjustment = await GetByOrderSplitRequest(orderSplits, false);

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Start process the order adjustment.
                    foreach (var order in orderToAdjustment)
                    {
                        var requestAdjustment = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        int newQtyToGo = order.qty_reqd - requestAdjustment.qty_togo;
                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        if (requestAdjustment != null)
                        {
                            db.SalesOrderDetails.Attach(order);
                            order.qty_routed = newQtyToGo;
                            order.last_maint_logon = actionByName;
                            order.last_maint = DateTime.Now;

                            db.UpdateEntityModel(order);
                        }
                    }

                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update the order priority request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> UpdateOrderRoutePriorityRequest(IEnumerable<Order_Priority> orders)
        {
            List<OrderSplit_Order> orderSplits = orders.Select(p => new OrderSplit_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no
            }).ToList();

            var orderToAdjustment = await GetByOrderRouteSplitRequest(orderSplits, false);

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Start process the order priority update.
                    foreach (var order in orderToAdjustment)
                    {
                        var requestUpdate = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        if (requestUpdate != null)
                        {
                            db.SalesOrderDetails.Attach(order);
                            order.route_priority = requestUpdate.priority;
                            order.last_maint_logon = actionByName;
                            order.last_maint = DateTime.Now;

                            db.UpdateEntityModel(order);
                        }
                    }

                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update the order status request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> UpdateOrderRouteStatusRequest(IEnumerable<Order_Status> orders)
        {
            List<OrderSplit_Order> orderSplits = orders.Select(p => new OrderSplit_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no
            }).ToList();

            var orderToAdjustment = await GetByOrderRouteSplitRequest(orderSplits, false);

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Start process the order status update.
                    foreach (var order in orderToAdjustment)
                    {
                        var requestUpdate = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        if (requestUpdate != null)
                        {
                            db.SalesOrderDetails.Attach(order);
                            order.route_status = requestUpdate.status;
                            order.last_maint_logon = actionByName;
                            order.last_maint = DateTime.Now;

                            db.UpdateEntityModel(order);
                        }
                    }

                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Allocate the order request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>bool (Success or Unsuccess)</returns>
        public async Task<bool> AllocateOrderRouteRequest(IEnumerable<Allocate_Order> orders)
        {
            List<OrderSplit_Order> orderSplits = orders.Select(p => new OrderSplit_Order
            {
                order_no = p.order_no,
                line_no = p.line_no,
                split_no = p.split_no
            }).ToList();

            var orderToAllocate = await GetByOrderRouteSplitRequest(orderSplits, false);
            var maxSplitOfOrderLine = await GetMaxSplitNoOfOrderLine(orderSplits.Select(p => new OrderSplit_Order { order_no = p.order_no, line_no = p.line_no }));

            string userAccount = LISRequest.CurrentAccountInfo.UserAccount.account_name;

            // Start database transaction.
            using (var transaction = db.Database.BeginTransaction())
            {
                short currentBatch = 0;

                try
                {
                    // Start process the order allocate.
                    foreach (var order in orderToAllocate)
                    {
                        int originalOrderRequestQTY = order.qty_reqd;
                        int originalOrderInProgressQTY = order.qty_in_progress;
                        int originalOrderRouteQTY = order.qty_routed;
                        DateTime originalOrderLastMaint = order.last_maint;
                        string originalOrderLastMaintLogon = order.last_maint_logon;
                        bool isAirShip = order.last_maint_logon.EndsWith(",AIR");
                        string actionByName = isAirShip ? string.Concat(userAccount.SafeSubString(0, 8), ",AIR") : userAccount.SafeSubString(0, 12);

                        var requestUpdate = orders
                            .Where(p => p.order_no == order.order_no && p.line_no == order.line_no && p.split_no == order.split_no)
                            .FirstOrDefault();

                        if (requestUpdate != null && requestUpdate.serial != order.serial)
                        {
                            db.SalesOrderDetails.Attach(order);
                            order.last_maint_logon = actionByName;
                            order.last_maint = DateTime.Now;

                            // Deallocate serial (If serial number is empty).
                            if (string.IsNullOrEmpty(requestUpdate.serial))
                            {
                                order.serial = string.Empty;
                                if (order.qty_in_progress > 0)
                                {
                                    order.qty_in_progress = order.qty_in_progress - 1;
                                }
                            }
                            // Allocate serial (Provide serial number to split the order).
                            else
                            {
                                // Validate allocate serial number (Allow serial status "P" only).
                                bool IsValidSerial = await db.SerialTracks.AnyAsync(p => p.serial == requestUpdate.serial && p.status == "P");
                                if (IsValidSerial)
                                {
                                    // Check serial number is already allocated.
                                    bool IsSerialHasAlreadyAllocated = await db.SalesOrderDetails.AnyAsync(p => p.serial == requestUpdate.serial);
                                    if (!IsSerialHasAlreadyAllocated)
                                    {
                                        // Check order has been route completed.
                                        if (order.qty_routed != order.qty_reqd)
                                        {
                                            // Split the order, If qty_reqd more than one.
                                            if (order.qty_reqd > 1)
                                            {

                                                if (order.qty_in_progress > 0)
                                                {
                                                    order.qty_in_progress = order.qty_in_progress - 1;
                                                }
                                                order.qty_reqd = order.qty_reqd - 1;

                                                // Get last split number of current order line.
                                                var lastSplit = maxSplitOfOrderLine
                                                    .Where(p => p.order_no == order.order_no && p.line_no == order.line_no)
                                                    .FirstOrDefault();

                                                if (lastSplit == null)
                                                {
                                                    maxSplitOfOrderLine.Add(new DataSchduleOrderLineMaxSplitNumber
                                                    {
                                                        order_no = order.order_no,
                                                        line_no = order.line_no,
                                                        split_no = 1
                                                    });
                                                    currentBatch = 1;
                                                }
                                                else
                                                {
                                                    if (lastSplit.split_no < 2)
                                                        lastSplit.split_no = 1;
                                                    else
                                                        lastSplit.split_no += 1;
                                                    currentBatch = lastSplit.split_no;
                                                }

                                                ThaiLis_SalesOrderDetail newSplitOrder = new ThaiLis_SalesOrderDetail()
                                                {
                                                    order_no = order.order_no,
                                                    order_type = order.order_type,
                                                    line_no = order.line_no,
                                                    order_company = order.order_company,
                                                    split_no = currentBatch,
                                                    line_type = order.line_type,
                                                    part = order.part,
                                                    date_reqd = order.date_reqd,
                                                    qty_reqd = 1,
                                                    qty_in_progress = 1,
                                                    qty_routed = 0,
                                                    ship_to = order.ship_to,
                                                    shipment_no = order.shipment_no,
                                                    sch_priority = order.sch_priority,
                                                    sch_status = order.sch_status,
                                                    route_priority = order.route_priority,
                                                    route_status = order.route_status,
                                                    serial = requestUpdate.serial,
                                                    last_maint_logon = actionByName,
                                                    last_maint = DateTime.Now
                                                };
                                                db.SalesOrderDetails.Add(newSplitOrder);

                                                // Store order split history.
                                                var splitHistory = new ThaiLis_SalesOrderDetailSplit
                                                {
                                                    order_no = order.order_no,
                                                    order_type = order.order_type,
                                                    line_no = order.line_no,
                                                    order_company = order.order_company,
                                                    split_no = order.split_no,
                                                    line_type = order.line_type,
                                                    part = order.part,
                                                    date_reqd = order.date_reqd,
                                                    qty_reqd = originalOrderRequestQTY,
                                                    qty_in_progress = originalOrderInProgressQTY,
                                                    qty_routed = originalOrderRouteQTY,
                                                    ship_to = order.ship_to,
                                                    shipment_no = order.shipment_no,
                                                    sch_priority = order.sch_priority,
                                                    sch_status = order.sch_status,
                                                    route_priority = order.route_priority,
                                                    route_status = order.route_status,
                                                    serial = order.serial,
                                                    last_maint_logon = originalOrderLastMaintLogon,
                                                    last_maint = originalOrderLastMaint,
                                                    split_per_batch = 1,
                                                    split_batch_count = 1,
                                                    split_serial = requestUpdate.serial,
                                                    split_by = userAccount
                                                };
                                                db.SalesOrderDetailSplits.Add(splitHistory);

                                            }
                                            else
                                            {
                                                order.qty_in_progress = Math.Max(order.qty_in_progress, 1);
                                                order.serial = requestUpdate.serial;
                                            }
                                        }
                                    }
                                }
                            }

                            db.UpdateEntityModel(order);
                        }
                    }

                    // Save all changes.
                    int saveSplitOrder = await db.SaveChangesAsync();
                    if (saveSplitOrder > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            return true;
        }

    }
}