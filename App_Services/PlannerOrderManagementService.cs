using lissystem.App_DataContext;
using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace lissystem.App_Services
{
    public class PlannerOrderManagementService
    {
        private BaseDBContext db;
        public PlannerOrderManagementService()
        {
            db = new BaseDBContext();
        }

        /// <summary>
        /// Get Order split product except listใ
        /// </summary>
        /// <returns>IEnumerable PlannerOrderSplitProdExcept</returns>
        public async Task<IEnumerable<PlannerOrderSplitProdExcept>> GetOrderSplitProductExcepts()
        {
            var query = from q in db.PlannerOrderSplitProdClassExcepts
                        select q;
            return await query.OrderBy(p => p.product_class).ThenBy(p => p.product_subclass).ToListAsync();
        }
    }
}