using lissystem.App_Attributes;
using lissystem.App_Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers.CSITPanel
{
    [LISAuth, RoutePrefix("cpanel/lis/csit-inventory")]
    public class CSITInventoryController : LISController
    {
        public CSITInventoryController() { 
        }

        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

    }
}