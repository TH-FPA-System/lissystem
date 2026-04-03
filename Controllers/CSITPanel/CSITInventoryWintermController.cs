using lissystem.App_Attributes;
using lissystem.App_Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers.CSITPanel
{
    [LISAuth, RoutePrefix("cpanel/lis/csit-inventory/winterm")]
    public class CSITInventoryWintermController : LISController
    {
        public CSITInventoryWintermController() { 
        }

        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

    }
}