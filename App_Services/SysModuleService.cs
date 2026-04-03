using lissystem.App_DataContext;
using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace lissystem.App_Services
{
    public class SysModuleService
    {
        private BaseDBContext db;

        public SysModuleService()
        {
            db = new BaseDBContext();
        }

        /// <summary>
        /// Get all system modules.
        /// </summary>
        /// <returns>System module lists</returns>
        public async Task<IEnumerable<SysModule>> GetAllSysModules()
        {
            IEnumerable<SysModule> modules = await db.SysModules.ToListAsync();
            return modules;
        }

        /// <summary>
        /// Validate data for add system module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns>bool (True if is avialable to add)</returns>
        public async Task<bool> ValidModuleForAddData(SysModule module)
        {
            if (module == null
                || string.IsNullOrEmpty(module.module_section) || string.IsNullOrEmpty(module.module_name) || string.IsNullOrEmpty(module.module_description)
                || string.IsNullOrEmpty(module.controller_name) || string.IsNullOrEmpty(module.action_name))
                return false;

            bool isAvailable = await db.SysModules
                .AnyAsync(p => p.module_section == module.module_section && p.module_name == module.module_name);
            return !isAvailable;
        }

        /// <summary>
        /// Add new system module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns>bool (True if has added)</returns>
        public async Task<bool> Add(SysModule module)
        {
            if (module == null
                || string.IsNullOrEmpty(module.module_section) || string.IsNullOrEmpty(module.module_name) || string.IsNullOrEmpty(module.module_description)
                || string.IsNullOrEmpty(module.controller_name) || string.IsNullOrEmpty(module.action_name))
                return false;

            module.created_by = LISRequest.CurrentAccountId;
            db.SysModules.Add(module);
            // Save
            int save = await db.SaveChangeByTransAsync();

            // Default grant to Administrator role.
            if (save == 1)
            {
                SysModuleRole moduleRole = new SysModuleRole() { 
                    role_id = "ADM",
                    module_id = module.module_id,
                    canaccess = true,
                    canread = true,
                    canwrite = true,
                    candelete = true,
                    full_control = true,
                    created_at = DateTime.Now,
                    created_by = LISRequest.CurrentAccountId
                };
                db.SysModuleRoles.Add(moduleRole);
                await db.SaveChangesAsync();

                return true;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Update system module data.
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="module"></param>
        /// <returns>bool (True if has updated)</returns>
        public async Task<bool> Update(Guid moduleId, SysModule module)
        {
            if (module == null
                || string.IsNullOrEmpty(module.module_section) || string.IsNullOrEmpty(module.module_name) || string.IsNullOrEmpty(module.module_description)
                || string.IsNullOrEmpty(module.controller_name) || string.IsNullOrEmpty(module.action_name))
                return false;

            SysModule sysModule = await db.SysModules.FirstOrDefaultAsync(p => p.module_id == moduleId);
            if (sysModule == null)
                return false;

            sysModule.module_section = module.module_section;
            sysModule.submodule_section = module.submodule_section;
            sysModule.module_name = module.module_name;
            sysModule.module_description = module.module_description;
            sysModule.controller_name = module.controller_name;
            sysModule.action_name = module.action_name;
            sysModule.disabled = module.disabled;

            db.UpdateEntityModel(sysModule, true);
            int update = await db.SaveChangeByTransAsync();
            return (update == 1);
        }

        /// <summary>
        /// Get roles of system module for account.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Control panel role module for account list</returns>
        public IEnumerable<ControlpanelRoleModule> GetRoleModuleForAccount(Guid accountId)
        {
            IEnumerable<ControlpanelRoleModule> modules = new List<ControlpanelRoleModule>();
            modules = (from u in db.UserAccountInformations
                       where u.account_id == accountId
                       join smr in db.SysModuleRoles on u.role_id equals smr.role_id
                       join sm in db.SysModules on new { smr.module_id, disabled = false } equals new { sm.module_id, sm.disabled }
                       select new ControlpanelRoleModule
                       {
                           account_id = u.account_id,
                           module_id = sm.module_id,
                           module_section = sm.module_section,
                           submodule_section = sm.submodule_section,
                           module_name = sm.module_name,
                           module_description = sm.module_description,
                           controller_name = sm.controller_name,
                           action_name = sm.action_name
                       }).ToList();
            return modules;
        }

        /// <summary>
        /// Get role permission of system for account.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public SysModuleRole GetModulePermissionForAccount(Guid accountId, string controller)
        {
            SysModuleRole result = null;

            var moduleRole = (from m in db.SysModules
                              where m.controller_name == controller && m.disabled == false
                              join r in db.SysModuleRoles on m.module_id equals r.module_id
                              join u in db.UserAccountRoles on new { r.role_id } equals new { u.role_id }
                              join uinfo in db.UserAccountInformations on new { u.role_id, account_id = accountId } equals new { uinfo.role_id, uinfo.account_id }
                              select r).FirstOrDefault();
            result = moduleRole;
            return result;
        }

        /// <summary>
        /// Get role permission of system module for account.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns>SysModuleRole</returns>
        public SysModuleRole GetModulePermissionForAccount(Guid accountId, string controller, string action)
        {
            SysModuleRole result = null;

            var moduleRole = (from m in db.SysModules
                              where m.controller_name == controller && m.action_name == action && m.disabled == false
                              join r in db.SysModuleRoles on m.module_id equals r.module_id
                              join u in db.UserAccountRoles on new { r.role_id } equals new { u.role_id }
                              join uinfo in db.UserAccountInformations on new { u.role_id, account_id = accountId } equals new { uinfo.role_id, uinfo.account_id }
                              select r).FirstOrDefault();
            result = moduleRole;
            return result;
        }

    }
}