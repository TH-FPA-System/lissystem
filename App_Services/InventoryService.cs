using lissystem.App_DataContext;
using lissystem.Models.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace lissystem.App_Services
{
    public class InventoryService
    {
        private BaseDBContext db;

        public InventoryService()
        {
            db = new BaseDBContext();
        }

        /// <summary>
        /// Get summary data.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="subCategoryId"></param>
        /// <returns>IEnumerable DataInventorySummaryData</returns>
        public async Task<IEnumerable<DataInventorySummaryData>> GetInventorySummary(int categoryId, int subCategoryId = 0)
        {
            var query = from subcategory in db.InventorySubCategories
                        where subcategory.inv_category_id == categoryId

                        // Perform the LEFT JOIN by joining the subcategory to the entire Inventories table
                        join inventory in db.Inventories 
                        on new { subcategory.inv_category_id, subcategory.inv_subcategory_id, onhand = true } 
                        equals new { inventory.inv_category_id, inventory.inv_subcategory_id, inventory.onhand }
                        into inventoryGroup // 'inventoryGroup' is the group of all matching inventory items

                        // Now group the results by the subcategory again to get the count
                        from subcategoryGroup in inventoryGroup.DefaultIfEmpty()
                        .GroupBy(i => new {
                            Subcategory = subcategory,
                            CategoryId = subcategory.inv_category_id,
                            SubcategoryId = subcategory.inv_subcategory_id
                        })

                        // Final projection
                        select new DataInventorySummaryData
                        {
                            categoryId = subcategoryGroup.Key.CategoryId,
                            subCategoryId = subcategoryGroup.Key.SubcategoryId,
                            order_no = subcategoryGroup.Key.Subcategory.order_no,
                            subcategory_name = subcategoryGroup.Key.Subcategory.inv_subcategory_name,
                            subcategory_icon = subcategoryGroup.Key.Subcategory.inv_subcategory_icon,
                            safety_stock = subcategoryGroup.Key.Subcategory.safety_stock,

                            // Get the count. The count will be 0 if DefaultIfEmpty() returned null (no inventory)
                            stock_count = subcategoryGroup.Count(i => i != null)
                        };

            List<DataInventorySummaryData> result = await query.OrderBy(p => p.order_no).ToListAsync();
            return result;
        }
    }
}