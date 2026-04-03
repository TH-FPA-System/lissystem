using lissystem.App_DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace lissystem.App_Interface
{
    public class LISRoleProvider : RoleProvider
    {
        private readonly BaseDBContext BaseDB = new BaseDBContext();

        public override string ApplicationName { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            string[] roles = BaseDB.UserAccountRoles.Select(p => p.role_id).ToArray();
            return roles;
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] roles = new string[0];
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return roles;
            }
            return roles;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            string[] users = new string[0];
            return users;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool isUserInRole = (from user in BaseDB.UserAccounts
                                 join info in BaseDB.UserAccountInformations on user.account_id equals info.account_id
                                 join role in BaseDB.UserAccountRoles on info.role_id equals roleName
                                 where user.account_name == username
                                 select role.description
                                 ).Any();
            return isUserInRole;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            bool roleExist = BaseDB.UserAccountRoles.Where(p => p.role_id == roleName).Any();
            return roleExist;
        }
    }
}