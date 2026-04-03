using lissystem.App_Interface;
using lissystem.App_Services;
using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers.SystemPanel
{
    public class LISPanelController : LISController
    {
        private UserAccountService UserAccountService;
        private SysModuleService SysModuleService;

        public LISPanelController()
        {
            UserAccountService = new UserAccountService();
            SysModuleService = new SysModuleService();
        }

        /// <summary>
        /// Partial Control panel Header&Side menu view layout.
        /// </summary>
        /// <returns>Partial VIEW</returns>
        [ChildActionOnly]
        public ActionResult HeaderAndSideMenu()
        {
            ControlPanelModel model = new ControlPanelModel();
            model.UserAccountInfo = UserAccountService.GetAccountInformation(LISRequest.CurrentAccountId);
            model.RoleModules = SysModuleService.GetRoleModuleForAccount(LISRequest.CurrentAccountId);
            return PartialView("_ControlPanelHeaderAndSideMenu", model);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            return PartialView("_ControlPanelFooter");
        }
    }
}