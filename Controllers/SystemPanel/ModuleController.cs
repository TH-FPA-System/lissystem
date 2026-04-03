using lissystem.App_Attributes;
using lissystem.App_Interface;
using lissystem.App_Services;
using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers.SystemPanel
{
    [LISAuth]
    [RoutePrefix("cpanel/system/module")]
    public class ModuleController : LISController
    {
        private SysModuleService sysModuleService;

        public ModuleController()
        {
            sysModuleService = new SysModuleService();
        }

        /// <summary>
        /// First page view.
        /// </summary>
        /// <returns>VIEW</returns>
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Partial add new data form view (AJax request).
        /// </summary>
        /// <returns>Partial VIEW</returns>
        [HttpGet]
        [Route("ajax/data/add"), LISAjaxRequest]
        public ActionResult AjaxDataAddNew()
        {
            SysModule model = new SysModule();
            return PartialView(model);
        }

        /// <summary>
        /// Partial data form view (Ajax request).
        /// </summary>
        /// <param name="role_id"></param>
        /// <returns>Partial VIEW</returns>
        [HttpGet]
        [Route("ajax/data/view"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataView(Guid module_id)
        {
            SysModule model = await BaseDB.SysModules.FirstOrDefaultAsync(p => p.module_id == module_id);
            return PartialView(model);
        }

        /******************************************************************
         * Ajax to support DataTables
         ******************************************************************/

        /// <summary>
        /// Data list support DataTable (Ajax request).
        /// </summary>
        /// <returns>JSON</returns>
        [HttpGet]
        [Route("ajax/data/lists"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataLists()
        {
            NameValueCollection requestQuery = Request.QueryString;
            long requestIndraw = long.TryParse(requestQuery.Get("_"), out long idraw) ? idraw : 0;

            var lists = await BaseDB.SysModules
                .Select(p => new
                {
                    p.module_id,
                    p.module_section,
                    p.submodule_section,
                    p.module_name,
                    p.module_description,
                    p.disabled
                })
                .OrderBy(p => p.module_section)
                .ThenByDescending(p => p.module_name)
                .ToListAsync();

            return JsonForDataTable(requestIndraw, lists, lists.Count());
        }

        /// <summary>
        /// Add a new data request form (Ajax request).
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON</returns>
        [HttpPost]
        [Route("ajax/data/module/add"), LISAjaxRequest]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AjaxDataModuleAdd(SysModule model)
        {
            if (!ModelState.IsValid)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Bad request", System.Net.HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(model.module_section) || string.IsNullOrEmpty(model.module_name) || string.IsNullOrEmpty(model.module_description)
                || string.IsNullOrEmpty(model.controller_name) || string.IsNullOrEmpty(model.action_name))
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Bad request", System.Net.HttpStatusCode.BadRequest);
            }

            bool validate = await sysModuleService.ValidModuleForAddData(model);
            if (validate)
            {
                model.module_section = model.module_section.Trim();
                model.module_name = model.module_name.Trim();
                model.module_description = model.module_description.Trim();
                model.controller_name = model.controller_name.Trim();
                model.action_name = model.action_name.Trim();

                bool add = await sysModuleService.Add(model);
                if (add)
                    return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
                else
                    return JsonResult(LISGlobal.AJAX_RESULT_NOTINSERT, "Can't add new system module", System.Net.HttpStatusCode.NotModified);
            }
            else
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "System module is unavailable", System.Net.HttpStatusCode.NotAcceptable);
        }

        /// <summary>
        /// Update a data request form (Ajax request).
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON</returns>
        [HttpPost]
        [Route("ajax/data/module/update"), LISAjaxRequest]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AjaxDataModuleUpdate(SysModule model)
        {
            if (!ModelState.IsValid)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Bad request", System.Net.HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(model.module_section) || string.IsNullOrEmpty(model.module_name) || string.IsNullOrEmpty(model.module_description)
                || string.IsNullOrEmpty(model.controller_name) || string.IsNullOrEmpty(model.action_name))
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Bad request", System.Net.HttpStatusCode.BadRequest);
            }

            bool update = await sysModuleService.Update(model.module_id, model);
            if (update)
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            else
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "Can't update system module", System.Net.HttpStatusCode.NotModified);
        }
    }
}