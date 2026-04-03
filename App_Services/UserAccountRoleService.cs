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
    public class UserAccountRoleService
    {
        private BaseDBContext db;

        public UserAccountRoleService()
        {
            db = LISGlobal.BaseDB;
        }

        /// <summary>
        /// Check account role id is available.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> IsRoleAvailable(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return false;

            bool accountNameExists = await db.UserAccountRoles.AnyAsync(p => p.role_id == roleId.Trim());
            return !accountNameExists;
        }

        /// <summary>
        /// Add new account role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> AddNewRole(UserAccountRole role)
        {
            if (role == null)
                return false;

            // New Account role
            role.created_by = LISRequest.CurrentAccountId;
            db.UserAccountRoles.Add(role);
            // Save
            int save = await db.SaveChangeByTransAsync();

            if (save > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> DeleteRole(string role_id)
        {
            if (string.IsNullOrEmpty(role_id))
                return false;

            UserAccountRole role = await db.UserAccountRoles
                .Where(p => 
                    p.allow_update && p.role_id == role_id.Trim() 
                    && db.UserAccountInformations.Where(member => member.role_id == p.role_id).Count() == 0)
                .FirstOrDefaultAsync();

            if (role == null)
                return false;

            db.UserAccountRoles.Remove(role);
            int save = await db.SaveChangeByTransAsync();
            return save == 1;
        }
    }
}