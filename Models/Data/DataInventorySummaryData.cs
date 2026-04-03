using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Data
{
    public class DataInventorySummaryData
    {
        public int categoryId { get; set; }
        public int subCategoryId { get; set; }
        public int order_no { get; set; }
        public string subcategory_name { get; set; }
        public string subcategory_icon { get; set; }
        public int safety_stock { get; set; }
        public int stock_count { get; set; }
    }
}