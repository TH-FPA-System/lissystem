using lissystem.App_DataContext;
using lissystem.Models;
using lissystem.Models.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace lissystem.App_Services
{
    public class UserAccountService
    {
        private BaseDBContext db;

        public UserAccountService()
        {
            db = LISGlobal.BaseDB;
        }

        /// <summary>
        /// User account validation
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<LISUserAccount_Validate> UserAccountValidation(string accountName, string password)
        {
            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(password))
            {
                return new LISUserAccount_Validate() { code = -1, status = "Invalid parameter" };
            }

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@accountName", accountName));
            sqlParameters.Add(new SqlParameter("@password", password));
            sqlParameters.Add(new SqlParameter("@lisSecretKey", LISGlobal.LIS_AUTH_ENCKEY));
            Guid account_id = await db.Database.SqlQuery<Guid>("SELECT account_id FROM dbo.user_accounts WHERE " +
                                    "account_name = @accountName AND CONVERT(NVARCHAR, DECRYPTBYPASSPHRASE(@lisSecretKey, [password])) = @password AND deleted = 0", sqlParameters.ToArray())
                                    .FirstOrDefaultAsync();
            if (account_id != Guid.Empty)
                return new LISUserAccount_Validate() { code = 1, status = "valid", account_id = account_id };
            else
                return new LISUserAccount_Validate() { code = 0, status = "Invalid" };
        }

        /// <summary>
        /// Get all user accounts
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserAccount>> UserAccountAll()
        {
            IEnumerable<UserAccount> accounts = await db.UserAccounts
                .Include(p => p.Information)
                .ToListAsync();
            return accounts;
        }

        /// <summary>
        /// Get user account information
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public UserAccountInformation GetAccountInformation(Guid AccountID)
        {
            UserAccountInformation info = db.UserAccountInformations
                .Where(p => p.account_id == AccountID)
                .Include(p => p.Role)
                .FirstOrDefault();
            return info;
        }

        /// <summary>
        /// Get user account information (Async)
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public async Task<UserAccountInformation> GetAccountInformationAsync(Guid AccountID)
        {
            UserAccountInformation info = await db.UserAccountInformations
                .Where(p => p.account_id == AccountID)
                .Include(p => p.Role)
                .FirstOrDefaultAsync();
            return info;
        }

        /// <summary>
        /// Check account name is available.
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public async Task<bool> IsAccountNameAvailable(string accountName)
        {
            if (string.IsNullOrEmpty(accountName))
                return false;
            bool accountNameExists = await db.UserAccounts.AnyAsync(p => p.account_name == accountName.Trim());
            return !accountNameExists;
        }

        /// <summary>
        /// Get all user account roles as SelectListItem.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SelectListItem>> AllSelectListRoles(bool ignoreDisabled = true)
        {
            SelectListItem firstEmptySelection = new SelectListItem() { Value = "", Text = "--- Select a Role ---" };
            var roleSelections = await db.UserAccountRoles
                .Where(p => (ignoreDisabled && !p.disabled || !ignoreDisabled))
                .Select(p => new SelectListItem { Value = p.role_id, Text = p.description })
                .OrderBy(p => p.Text)
                .ToArrayAsync();
            IEnumerable<SelectListItem> selectList = Enumerable.Repeat(firstEmptySelection, 1).Concat(roleSelections).ToArray();
            return selectList;
        }

        /// <summary>
        /// Change user account password.
        /// </summary>
        /// <param name="AccountID"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> ChangeAccountPasswordAsync(Guid AccountID, string newPassword)
        {
            if (AccountID == Guid.Empty || string.IsNullOrEmpty(newPassword.Trim()))
                return false;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@accountID", System.Data.SqlDbType.UniqueIdentifier) { Value = AccountID });
            parameters.Add(new SqlParameter("@newPassword", newPassword));
            parameters.Add(new SqlParameter("@lisSecretKey", LISGlobal.LIS_AUTH_ENCKEY));
            parameters.Add(new SqlParameter("@expiresDate", new DateTime(2099, 12, 31)));
            int change = await db.Database.ExecuteSqlCommandAsync("UPDATE dbo.user_accounts SET [password] = CONVERT(VARBINARY(128), ENCRYPTBYPASSPHRASE(@lisSecretKey, @newPassword)) " +
                                ", password_expires = @expiresDate " +
                                "WHERE account_id = @accountID", parameters.ToArray());
            return change == 1;
        }

        /// <summary>
        /// Add new user account.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<bool> AddNewUserAccount(UserAccount account)
        {
            if (account == null)
                return false;

            // New User account
            account.created_by = LISRequest.CurrentAccountId;
            account.password_expires = new DateTime(2099, 12, 31);
            db.UserAccounts.Add(account);
            // Save
            int save = await db.SaveChangeByTransAsync();

            if (save == 1 && account.account_id != Guid.Empty)
            {
                // Set User account password
                bool setAccountPwd = await this.ChangeAccountPasswordAsync(account.account_id, account.password);

                // New User account information
                account.Information.account_id = account.account_id;
                db.UserAccountInformations.Add(account.Information);
                // Save
                save = await db.SaveChangeByTransAsync();
                return save == 1;
            }
            else if (save == 2 && (account.account_id != Guid.Empty && account.Information.account_id != Guid.Empty))
            {
                // Set User account password
                bool setAccountPwd = await this.ChangeAccountPasswordAsync(account.account_id, account.password);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Delete user account.
        /// </summary>
        /// <param name="account_id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserAccount(Guid account_id)
        {
            UserAccount account = await db.UserAccounts.FirstOrDefaultAsync(p => p.account_id == account_id && !p.deleted);
            if (account != null)
            {
                account.deleted = true;
                account.deleted_at = db.ServerDateTime();
                if (LISRequest.IsAuthenticate)
                    account.deleted_by = LISRequest.CurrentAccountId;
                db.UpdateEntityModel(account);
                int save = await db.SaveChangesAsync();
                return save == 1;
            }
            return false;
        }

        /// <summary>
        /// Store user account session.
        /// </summary>
        /// <param name="account_id"></param>
        /// <returns></returns>
        public async Task<bool> StoreUserSession(Guid account_id)
        {
            UserAccountSession session = new UserAccountSession
            {
                account_id = account_id,
                host_name = LISRequest.GetRequestHostName(),
                ip_address = LISRequest.GetRequestIPAddress(),
                created_at = DateTime.Now,
                expires_at = LISGlobal.LISMaxDateTime,
                engine = LISRequest.GetRequestBrowserEngine()
            };
            db.UserAccountSessions.Add(session);
            int save = await db.SaveChangeByTransAsync();
            return save == 1;
        }

        /// <summary>
        /// Set expired user account session.
        /// </summary>
        /// <param name="account_id"></param>
        /// <param name="host_name"></param>
        /// <returns></returns>
        public async Task<bool> SetSessionExpire(Guid account_id, string host_name = "")
        {
            if (account_id == Guid.Empty && string.IsNullOrEmpty(host_name))
                return false;

            IQueryable<UserAccountSession> sessionQuery = from q in db.UserAccountSessions
                          where q.expires_at >= DateTime.Now
                          select q;

            if (account_id != Guid.Empty)
            {
                sessionQuery = sessionQuery.Where(p => p.account_id == account_id);
            }

            if (!string.IsNullOrEmpty(host_name))
            {
                sessionQuery = sessionQuery.Where(p => p.host_name == host_name);
            }
            
            var sessions = await sessionQuery.ToListAsync();
            if (sessions.Count() > 0)
            {
                foreach (var session in sessions)
                {
                    session.expires_at = DateTime.Now;
                    db.UpdateEntityModel(session);
                }
                int save = await db.SaveChangeByTransAsync();
                return save > 0;
            }
            return false;
        }

    }
}