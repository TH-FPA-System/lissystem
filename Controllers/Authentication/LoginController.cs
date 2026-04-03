using lissystem.App_Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using lissystem.Models.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using lissystem.App_DataContext;
using lissystem.Models;
using System.Data.Entity;
using System.Web.Security;
using lissystem.App_Services;
namespace lissystem.Controllers.Authentication
{
    public class LoginController : LISController
    {
        private UserAccountService userAccountService;

        public LoginController()
        {
            userAccountService = new UserAccountService();
        }

        [HttpGet]
        [Route("login")]
        public ActionResult Login(string RedirectToUrl = "")
        {
            FormLogin loginModel = TempData[LISGlobal.LIS_KEY_LOGINMODLE_TEMP] as FormLogin ?? new FormLogin();
            return View(loginModel);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            if (LISRequest.IsAuthenticate)
            {
                await userAccountService.SetSessionExpire(LISRequest.CurrentAccountId, LISRequest.GetRequestHostName());
            }

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> LoginProcess(FormLogin model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrEmpty(model.account_name) || string.IsNullOrEmpty(model.password))
            {
                TempData[LISGlobal.LIS_KEY_LOGINMODLE_TEMP] = model;
                return (!string.IsNullOrEmpty(model.RedirectToUrl) ? RedirectToAction("Login", new { RedirectToUrl = model.RedirectToUrl }) : RedirectToAction("Login"));
            }

            LISUserAccount_Validate validate = await userAccountService.UserAccountValidation(model.account_name, model.password);
            if (validate.account_id.HasValue)
            {
                model.password = string.Empty;
                UserAccountInformation information = await userAccountService.GetAccountInformationAsync(validate.account_id.Value);

                if (information != null && information.Role != null)
                {

                    if (!information.canlogin)
                    {
                        TempData[LISGlobal.LIS_KEY_LOGINMODLE_TEMP] = model;
                        TempData[LISGlobal.LIS_KEY_LOGIN_TEMP] = new FormLoginTemp { ErrorType = LISGlobal.LIS_LOGIN_ERROR.CANTLOGIN, ErrorMessage = "Your account has no permission to log in " };
                        return (!string.IsNullOrEmpty(model.RedirectToUrl) ? RedirectToAction("Login", new { RedirectToUrl = model.RedirectToUrl }) : RedirectToAction("Login"));
                    }
                    else if (!information.disabled && !information.Role.disabled)
                    {
                        await userAccountService.SetSessionExpire(LISRequest.CurrentAccountId, LISRequest.GetRequestHostName());
                        await userAccountService.StoreUserSession(validate.account_id.Value);

                        FormsAuthentication.SetAuthCookie(information.account_id.ToString(), model.rememberMe);
                        if (!string.IsNullOrEmpty(model.RedirectToUrl))
                        {
                            return Redirect(model.RedirectToUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "CPHome");
                        }
                    }
                    else
                    {
                        TempData[LISGlobal.LIS_KEY_LOGINMODLE_TEMP] = model;
                        TempData[LISGlobal.LIS_KEY_LOGIN_TEMP] = new FormLoginTemp { ErrorType = LISGlobal.LIS_LOGIN_ERROR.DISABLED, ErrorMessage = "Your account has disabled" };
                        return (!string.IsNullOrEmpty(model.RedirectToUrl) ? RedirectToAction("Login", new { RedirectToUrl = model.RedirectToUrl }) : RedirectToAction("Login"));
                    }    
                }
                else
                {
                    TempData[LISGlobal.LIS_KEY_LOGINMODLE_TEMP] = model;
                    TempData[LISGlobal.LIS_KEY_LOGIN_TEMP] = new FormLoginTemp { ErrorType = LISGlobal.LIS_LOGIN_ERROR.INFOMATION_NOTFOUND, ErrorMessage = "User account information not found" };
                    return (!string.IsNullOrEmpty(model.RedirectToUrl) ? RedirectToAction("Login", new { RedirectToUrl = model.RedirectToUrl }) : RedirectToAction("Login"));
                }

            }
            else
            {
                TempData[LISGlobal.LIS_KEY_LOGINMODLE_TEMP] = model;
                TempData[LISGlobal.LIS_KEY_LOGIN_TEMP] = new FormLoginTemp { ErrorType = LISGlobal.LIS_LOGIN_ERROR.INVALID, ErrorMessage = "Please check account name or password" };
                return (!string.IsNullOrEmpty(model.RedirectToUrl) ? RedirectToAction("Login", new { RedirectToUrl = model.RedirectToUrl }) : RedirectToAction("Login"));
            }
        }
    }
}