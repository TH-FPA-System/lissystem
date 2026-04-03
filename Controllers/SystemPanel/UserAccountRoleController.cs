using lissystem.App_Attributes;
using lissystem.App_Interface;
using lissystem.App_Services;
using lissystem.Models;
using lissystem.Models.Form;
using Newtonsoft.Json;
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
    [RoutePrefix("cpanel/system/uaccount/role")]
    public class UserAccountRoleController : LISController
    {
        private UserAccountRoleService roleService;

        public UserAccountRoleController()
        {
            roleService = new UserAccountRoleService();
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
        /// Partial data form view (Ajax request).
        /// </summary>
        /// <param name="role_id"></param>
        /// <returns>Partial VIEW</returns>
        [HttpGet]
        [Route("ajax/data/view"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataViewInfo(string role_id)
        {
            FormSystemAccountRoleView model = new FormSystemAccountRoleView();

            model.Role = await BaseDB.UserAccountRoles
                .FirstOrDefaultAsync(p => p.role_id == role_id);

            return PartialView(model);
        }

        /// <summary>
        /// Partial add new data form view (AJax request).
        /// </summary>
        /// <returns>Partial VIEW</returns>
        [HttpGet]
        [Route("ajax/data/add"), LISAjaxRequest]
        public ActionResult AjaxDataAddNew()
        {
            FormSystemAccountRoleView model = new FormSystemAccountRoleView();
            model.Role = new UserAccountRole();
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

            var queryData = await BaseDB.UserAccountRoles
                .Select(role => new
                {
                    role.role_id,
                    role.description,
                    role.disabled,
                    role.allow_update,
                    user_inrole = BaseDB.UserAccountInformations.Where(p => p.role_id == role.role_id).Count(),
                    allow_delete = role.allow_update && BaseDB.UserAccountInformations.Where(p => p.role_id == role.role_id).Count() == 0
                }).OrderBy(p => p.role_id).ToArrayAsync();

            int recordsTotal = queryData.Count();
            return JsonForDataTable(requestIndraw, queryData, recordsTotal);
        }

        /// <summary>
        /// Add a new data request form (Ajax request).
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON</returns>
        [HttpPost]
        [Route("ajax/data/role/add"), LISAjaxRequest]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AjaxDataAccountRoleAdd(FormSystemAccountRoleView model)
        {
            var formData = new
            {
                role_id = model.Role.role_id.Trim(),
                description = model.Role.description.Trim(),
                disabled = model.Role.disabled
            };

            if (string.IsNullOrEmpty(formData.role_id) || string.IsNullOrEmpty(formData.description))
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Bad request", System.Net.HttpStatusCode.BadRequest);

            // Check account role available.
            bool isRoleAvailable = await roleService.IsRoleAvailable(formData.role_id);
            if (!isRoleAvailable)
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Role id is unavailable", System.Net.HttpStatusCode.NotAcceptable);

            UserAccountRole role = new UserAccountRole()
            {
                role_id = formData.role_id,
                description = formData.description,
                disabled = formData.disabled
            };

            bool roleAdd = await roleService.AddNewRole(role);
            if (roleAdd)
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            else
                return JsonResult(LISGlobal.AJAX_RESULT_NOTINSERT, "Can't add new role", System.Net.HttpStatusCode.NotModified);
        }

        /// <summary>
        /// Delete a data request (Ajax request).
        /// </summary>
        /// <param name="role_id"></param>
        /// <returns>JSON</returns>
        [HttpPost]
        [Route("ajax/data/role/delete"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataAccountRoleDelete(string role_id)
        {
            bool delete = await roleService.DeleteRole(role_id);
            if (delete)
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            else
                return JsonResult(LISGlobal.AJAX_RESULT_NOTDELETE, "", System.Net.HttpStatusCode.NotModified);
        }

        /// <summary>
        /// Update a data form request (Ajax request).
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON</returns>
        [HttpPost]
        [Route("ajax/data/role/update"), LISAjaxRequest]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AjaxDataAccountRole_Update(FormSystemAccountRoleView model)
        {
            var formData = new
            {
                role_id = model.Role.role_id,
                description = model.Role.description,
                disabled = model.Role.disabled
            };

            if (string.IsNullOrEmpty(formData.role_id) || string.IsNullOrEmpty(formData.description))
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Bad request", System.Net.HttpStatusCode.BadRequest);

            // Update User account role data.
            var role = await BaseDB.UserAccountRoles
                .FirstOrDefaultAsync(p => p.role_id == formData.role_id);
            if (role != null)
            {
                role.description = formData.description;
                role.disabled = formData.disabled;

                BaseDB.UpdateEntityModel(role, true);
                int update = await BaseDB.SaveChangeByTransAsync();
                if (update <= 0)
                {
                    return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "Can't update role data", System.Net.HttpStatusCode.NotModified);
                }
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            }
            else
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "Can't update account role", System.Net.HttpStatusCode.NotFound);

        }

        /// <summary>
        /// Data member inrole list (Ajax request).
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>JSON</returns>
        [HttpGet]
        [Route("ajax/data/role/{roleId}/members"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataRoleMember(string roleId)
        {
            NameValueCollection requestQuery = Request.QueryString;
            long requestIndraw = long.TryParse(requestQuery.Get("_"), out long idraw) ? idraw : 0;

            if (string.IsNullOrEmpty(roleId))
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid paramter", System.Net.HttpStatusCode.BadRequest);
            }

            var members = await BaseDB.UserAccountInformations
                .Where(p => p.role_id == roleId)
                .Select(p => new
                {
                    p.account_id,
                    p.code,
                    p.display_name,
                    p.role_id
                }).OrderBy(p => p.display_name).ToListAsync();

            return JsonForDataTable(requestIndraw, members, members.Count());
        }

        /// <summary>
        /// Delete member inrole (Ajax request).
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>JSON</returns>
        [HttpPost]
        [Route("ajax/data/role/{roleId}/member/remove"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataRoleRemoveMember(string roleId)
        {
            NameValueCollection requestForm = Request.Form;
            var formRequest = new
            {
                roleId = roleId,
                userId = Guid.TryParse(requestForm.Get("userId"), out Guid uid) ? uid : (Guid?)null
            };

            if (string.IsNullOrEmpty(formRequest.roleId))
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid paramter", System.Net.HttpStatusCode.BadRequest);
            }

            var userAccount = await BaseDB.UserAccountInformations
                .Where(p => p.role_id == formRequest.roleId && p.account_id == formRequest.userId)
                .FirstOrDefaultAsync();

            if (userAccount == null)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "Not found data user in role", System.Net.HttpStatusCode.NotModified);
            }
            else
            {
                // Set to default guest role.
                userAccount.role_id = "GST";
                BaseDB.UpdateEntityModel(userAccount, true);
                int update = await BaseDB.SaveChangeByTransAsync();
                if (update <= 0)
                {
                    return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "Can't update data user in role", System.Net.HttpStatusCode.NotModified);
                }
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            }
        }

        /// <summary>
        /// Data modules inrole list (Ajax request).
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>JSON</returns>
        [HttpGet]
        [Route("ajax/data/role/{roleId}/modules"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataRoleModule(string roleId)
        {
            NameValueCollection requestQuery = Request.QueryString;
            long requestIndraw = long.TryParse(requestQuery.Get("_"), out long idraw) ? idraw : 0;

            if (string.IsNullOrEmpty(roleId))
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid paramter", System.Net.HttpStatusCode.BadRequest);
            }

            var modules = await (
                from m in BaseDB.SysModules
                join smr in BaseDB.SysModuleRoles.Where(p => p.role_id == roleId) on m.module_id equals smr.module_id into moduleRoleGroup
                from smr in moduleRoleGroup.DefaultIfEmpty()
                select new RoleModules
                {
                    role_id = smr.role_id,
                    module_id = m.module_id,
                    module_section = m.module_section,
                    module_description = m.module_description,
                    canaccess = smr.canaccess,
                    canwrite = smr.canwrite,
                    candelete = smr.candelete,
                    IsGranted = !string.IsNullOrEmpty(smr.role_id)
                }).ToListAsync();

            return JsonForDataTable(requestIndraw, modules, modules.Count());
        }

        /// <summary>
        /// Grant module access permission to the role (Ajax request).
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>JSON</returns>
        [HttpPost]
        [Route("ajax/data/role/{roleId}/module/grant"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataRoleModuleGrant(string roleId)
        {
            NameValueCollection requestForm = Request.Form;
            var moduleGrant = new
            {
                role_id = roleId,
                module_id = Guid.TryParse(requestForm.Get("module_id"), out Guid moduleId) ? moduleId : Guid.Empty,
                IsGrant = bool.TryParse(requestForm.Get("IsGranted"), out bool grant) ? grant : false,
            };

            if (moduleGrant.module_id == Guid.Empty)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid paramter", System.Net.HttpStatusCode.BadRequest);
            }

            // Grant module
            SysModuleRole grantModule;

            if (moduleGrant.IsGrant)
            {
                // New grant
                grantModule = new SysModuleRole
                {
                    role_id = moduleGrant.role_id,
                    module_id = moduleGrant.module_id,
                    canaccess = true,
                    canread = true,
                    created_at = DateTime.Now,
                    created_by = LISRequest.CurrentAccountId
                };
                BaseDB.SysModuleRoles.Add(grantModule);
                int letGrant = await BaseDB.SaveChangeByTransAsync();
                if (letGrant == 1)
                {
                    return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
                }
            }
            else
            {
                // Ungrant
                grantModule = await BaseDB.SysModuleRoles
                    .Where(p => p.role_id == moduleGrant.role_id && p.module_id == moduleGrant.module_id)
                    .FirstOrDefaultAsync();
                if (grantModule != null)
                {
                    BaseDB.SysModuleRoles.Remove(grantModule);
                    int ungrant = await BaseDB.SaveChangeByTransAsync();
                    if (ungrant == 1)
                    {
                        return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
                    }
                }
            }

            return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "", System.Net.HttpStatusCode.NotModified);
        }

        /// <summary>
        /// Grant module actions permission to the role (Ajax request).
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>JSON</returns>
        [HttpPost]
        [Route("ajax/data/role/{roleId}/module/grant/actions"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataRoleModuleActionsGrant(string roleId)
        {
            NameValueCollection requestForm = Request.Form;
            var actionGrant = new { 
                module_id = Guid.TryParse(requestForm.Get("module_id"), out Guid moduleId) ? moduleId : Guid.Empty,
                canwrite = bool.TryParse(requestForm.Get("canwrite"), out bool grantWrite) ? grantWrite : false,
                candelete = bool.TryParse(requestForm.Get("candelete"), out bool grantDelete) ? grantDelete : false
            };

            if (actionGrant.module_id == Guid.Empty)
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid paramter", System.Net.HttpStatusCode.BadRequest);
            }

            // Action module grant
            SysModuleRole grantActionModule = await BaseDB.SysModuleRoles
                .Where(p => p.role_id == roleId && p.module_id == actionGrant.module_id)
                .FirstOrDefaultAsync();
            if (grantActionModule != null)
            {
                grantActionModule.canwrite = actionGrant.canwrite;
                grantActionModule.candelete = (grantActionModule.canwrite && actionGrant.candelete);
                grantActionModule.full_control = (grantActionModule.canwrite && grantActionModule.candelete);

                BaseDB.UpdateEntityModel(grantActionModule, true);
                int grant = await BaseDB.SaveChangeByTransAsync();
                if (grant == 1)
                    return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            }

            return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "", System.Net.HttpStatusCode.NotModified);
        }
    }
}