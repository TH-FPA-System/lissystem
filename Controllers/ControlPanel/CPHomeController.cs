using lissystem.App_Attributes;
using lissystem.App_Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers.ControlPanel
{
    [RoutePrefix("cpanel")]
    [LISAuth(IgnoredModuleAuth = true)]
    public class CPHomeController : LISController
    {
        public CPHomeController() { 
        }

        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}