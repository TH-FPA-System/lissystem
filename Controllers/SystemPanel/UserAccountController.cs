using lissystem.App_Attributes;
using lissystem.App_Interface;
using lissystem.App_Services;
using lissystem.Models;
using lissystem.Models.Form;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers.SystemPanel
{
    [LISAuth]
    [RoutePrefix("cpanel/system/uaccount")]
    public class UserAccountController : LISController
    {
        private UserAccountService userAccountService;

        public UserAccountController()
        {
            userAccountService = new UserAccountService();
        }

        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("ajax/data/view"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataViewInfo(Guid account_id)
        {
            FormSystemAccountView model = new FormSystemAccountView();

            model.UserAccount = await BaseDB.UserAccounts
                .Include(p => p.Information)
                .Include(p => p.Information.Role)
                .FirstOrDefaultAsync(p => p.account_id == account_id);

            model.RoleSelectListItem = await userAccountService.AllSelectListRoles();

            return PartialView(model);
        }

        [HttpGet]
        [Route("ajax/data/add"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataAddNew()
        {
            FormSystemAccountView model = new FormSystemAccountView();
            model.UserAccount = new UserAccount();
            model.UserAccount.Information = new UserAccountInformation();
            model.UserAccountSecurity = new UserAccountPasswordUpdate();

            model.RoleSelectListItem = await userAccountService.AllSelectListRoles();

            return PartialView(model);
        }

        /******************************************************************
         * Ajax to support DataTables
         ******************************************************************/

        [HttpGet]
        [Route("ajax/data/lists"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataLists()
        {
            NameValueCollection requestQuery = Request.QueryString;
            long requestIndraw = long.TryParse(requestQuery.Get("_"), out long idraw) ? idraw : 0;

            var queryData = await (
                from q in BaseDB.UserAccounts
                join info in BaseDB.UserAccountInformations on q.account_id equals info.account_id
                join role in BaseDB.UserAccountRoles on info.role_id equals role.role_id
                where !q.deleted
                select new
                {
                    q.account_id,
                    q.account_name,
                    info.code,
                    info.display_name,
                    info.disabled,
                    info.canlogin,
                    role = role.description,
                    allow_delete = (info.role_id != "ADM" && q.account_id != LISRequest.CurrentAccountId)
                }
                ).OrderBy(p => p.account_name).ToListAsync();

            int recordsTotal = queryData.Count();
            return JsonForDataTable(requestIndraw, queryData, recordsTotal);
        }

        [HttpPost]
        [Route("ajax/data/account/update"), LISAjaxRequest]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AjaxDataAccount_Update(FormSystemAccountView model)
        {
            var formData = new
            {
                account_id = model.UserAccount.account_id,
                code = model.UserAccount.Information.code.Trim(),
                displayName = model.UserAccount.Information.display_name.Trim(),
                role = model.UserAccount.Information.role_id,
                disabled = model.UserAccount.Information.disabled,
                canLogin = model.UserAccount.Information.canlogin
            };

            if (string.IsNullOrEmpty(formData.code) || string.IsNullOrEmpty(formData.displayName))
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Bad request", System.Net.HttpStatusCode.BadRequest);

            // Update User account information
            UserAccountInformation information = await BaseDB.UserAccountInformations
                .Where(p => p.account_id == model.UserAccount.account_id)
                .FirstOrDefaultAsync();

            if (information != null)
            {
                information.code = formData.code;
                information.display_name = formData.displayName;

                if (LISRequest.CurrentAccountId != information.account_id)
                {
                    information.disabled = formData.disabled;
                    information.canlogin = formData.canLogin;
                    information.role_id = formData.role;
                }

                BaseDB.UpdateEntityModel(information, true);
                int update = await BaseDB.SaveChangeByTransAsync();
                if (update > 0)
                {
                    return JsonResult("OK", "", System.Net.HttpStatusCode.OK);
                }
                else
                    return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "Can't Update account information", System.Net.HttpStatusCode.NotModified);
            }
            else
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "Can't update account information", System.Net.HttpStatusCode.NotFound);

        }

        [HttpPost]
        [Route("ajax/data/account/security/update"), LISAjaxRequest]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AjaxDataAccountSecurity_Update(FormSystemAccountView model)
        {
            var formData = new
            {
                account_id = model.UserAccount.account_id,
                password = model.UserAccountSecurity.password.Trim(),
                confirmPassword = model.UserAccountSecurity.confirmPassword.Trim()
            };

            if (formData.account_id == Guid.Empty || string.IsNullOrEmpty(formData.password) || string.IsNullOrEmpty(formData.confirmPassword) ||
                formData.password != formData.confirmPassword)
                return JsonResult("FAIL", "Bad request", System.Net.HttpStatusCode.BadRequest);

            bool updatePassword = await userAccountService.ChangeAccountPasswordAsync(formData.account_id, formData.password);
            if (updatePassword)
            {
                return JsonResult("OK", "", System.Net.HttpStatusCode.OK);
            }
            else
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTUPDATE, "Can't update account security password", System.Net.HttpStatusCode.NotModified);
            }
        }

        [HttpPost]
        [Route("ajax/data/account/add"), LISAjaxRequest]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AjaxDataAccountAdd(FormSystemAccountView model)
        {
            var formData = new
            {
                account_name = model.UserAccount.account_name.Trim(),
                account_pwd = model.UserAccount.password.Trim(),
                account_conformPwd = model.UserAccountSecurity.confirmPassword.Trim(),

                code = model.UserAccount.Information.code.Trim(),
                display_name = model.UserAccount.Information.display_name.Trim(),
                role_id = model.UserAccount.Information.role_id.Trim(),
                section_id = model.UserAccount.Information.section_id
            };

            // Validate request parameters.
            if (string.IsNullOrEmpty(formData.account_name) ||
                string.IsNullOrEmpty(formData.account_pwd) || formData.account_pwd != formData.account_conformPwd ||
                string.IsNullOrEmpty(formData.code) ||
                string.IsNullOrEmpty(formData.display_name) ||
                string.IsNullOrEmpty(formData.role_id))
            {
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Invalid request parameter", System.Net.HttpStatusCode.NotAcceptable);
            }

            // Check account name available.
            bool isUserAccountNameAvailable = await userAccountService.IsAccountNameAvailable(formData.account_name);
            if (!isUserAccountNameAvailable)
                return JsonResult(LISGlobal.AJAX_RESULT_NOTACCEPT, "Account name is unavailable", System.Net.HttpStatusCode.NotAcceptable);

            // New user account object
            UserAccount userAccount = new UserAccount()
            {
                // Set User account data.
                account_name = formData.account_name,
                password = formData.account_pwd,

                // Set User account information data.
                Information = new UserAccountInformation()
                {
                    code = formData.code,
                    display_name = formData.display_name,
                    role_id = formData.role_id,
                    section_id = formData.section_id,
                    canlogin = true,
                }
            };

            // Add new user account
            bool add = await userAccountService.AddNewUserAccount(userAccount);
            if (add)
                return JsonResult(LISGlobal.AJAX_RESULT_OK, userAccount.account_id);
            else
                return JsonResult(LISGlobal.AJAX_RESULT_NOTINSERT, "", System.Net.HttpStatusCode.NotModified);
        }

        [HttpPost]
        [Route("ajax/data/account/delete"), LISAjaxRequest]
        public async Task<ActionResult> AjaxDataAccountDelete(Guid account_id)
        {
            bool delete = await userAccountService.DeleteUserAccount(account_id);
            if (delete)
                return JsonResult(LISGlobal.AJAX_RESULT_OK, "");
            else
                return JsonResult(LISGlobal.AJAX_RESULT_NOTDELETE, "", System.Net.HttpStatusCode.NotModified);
        }

    }
}