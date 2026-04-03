using lissystem.App_Attributes;
using lissystem.App_Interface;
using lissystem.App_Services;
using lissystem.Models.Data;
using lissystem.Models.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers.CSITPanel
{
    [LISAuth, RoutePrefix("cpanel/lis/csit-inventory/printer")]
    public class CSITInventoryPrinterController : LISController
    {
        private InventoryService invService;

        private const int CATEGORY_ID = 1001;

        public CSITInventoryPrinterController()
        {
            invService = new InventoryService();
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Index()
        {
            ModelInventoryPrinter model = new ModelInventoryPrinter();
            model.SummaryDatas = await invService.GetInventorySummary(CATEGORY_ID);
            return View(model);
        }

        [HttpGet]
        [Route("management")]
        public async Task<ActionResult> Management()
        {
            ModelInventoryPrinter model = new ModelInventoryPrinter();
            model.SummaryDatas = await invService.GetInventorySummary(CATEGORY_ID);
            return View(model);
        }

        public PartialViewResult PartialInventorySmallBox(IEnumerable<DataInventorySummaryData> data)
        {
            return PartialView(data);
        }
    }
}