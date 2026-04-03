using lissystem.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Page
{
    public class ModelInventoryPrinter
    {
        public IEnumerable<DataInventorySummaryData> SummaryDatas { get; set; }
    }
}