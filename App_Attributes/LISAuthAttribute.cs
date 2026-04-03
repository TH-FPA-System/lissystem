using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace lissystem.App_Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LISAuthAttribute : AuthorizeAttribute
    {
        public string UnAuthorizeView { get; set; }
        public bool IgnoredModuleAuth { get; set; } = false;
        public bool RequiredUpdatePermission { get; set; } = false;
        public bool RequiredDeletePermission { get; set; } = false;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            LISAuthorizeRequest(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuth = base.AuthorizeCore(httpContext);
            return isAuth;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string redirectToUrl = filterContext.HttpContext.Request.RawUrl;
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Login",
                    RedirectToUrl = redirectToUrl
                })
            );
        }

        internal void LISAuthorizeRequest(AuthorizationContext filterContext)
        {
            if (!LISRequest.IsAuthenticate)
            {
                string redirectToUrl = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Login",
                        RedirectToUrl = redirectToUrl
                    })
                );
            }
            else
            {
                bool IsModuleIgnored = this.IgnoredModuleAuth;
                bool IsRequiresUpdate = this.RequiredUpdatePermission;
                bool isRequiresDelete = this.RequiredDeletePermission;

                var authAttr = filterContext.ActionDescriptor.GetCustomAttributes(typeof(LISAuthAttribute), true).Cast<LISAuthAttribute>();
                foreach (var attr in authAttr)
                {
                    IsModuleIgnored = attr.IgnoredModuleAuth || IsModuleIgnored;
                    IsRequiresUpdate = attr.RequiredUpdatePermission || IsRequiresUpdate;
                    isRequiresDelete = attr.RequiredDeletePermission || isRequiresDelete;
                }

                var routeRequest = filterContext.RouteData;
                string controllerRequest = routeRequest.GetRequiredString("controller");
                string actionRequest = routeRequest.GetRequiredString("action");

                Guid accountId = LISRequest.CurrentAccountId;
                var accountInfo = LISRequest.CurrentAccountInfo;

                if (!IsModuleIgnored)
                {
                    if (accountInfo.role_id != "ADM")
                    {
                        SysModuleRole modulePermission = modulePermission = LISRequest.GetModulePermission(controllerRequest, actionRequest); ;
                        LISGlobal.CurrentModuleRole = modulePermission;

                        if (LISGlobal.CurrentModuleRole != null)
                        {
                            bool isCanUpdate = LISGlobal.CurrentModuleRole.canwrite;
                            bool isCanDelete = LISGlobal.CurrentModuleRole.candelete;

                            if (actionRequest != "Index")
                            {
                                if ((RequiredUpdatePermission && !isCanUpdate) || (RequiredDeletePermission && !isCanDelete))
                                {
                                    if (!filterContext.HttpContext.Request.IsAjaxRequest())
                                    {
                                        if (!string.IsNullOrEmpty(UnAuthorizeView))
                                            filterContext.Result = new ViewResult { ViewName = UnAuthorizeView };
                                        else
                                            filterContext.Result = new ViewResult { ViewName = "Module_NoPermissionToAcccess" };
                                    }
                                    else
                                    {
                                        filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                                        filterContext.HttpContext.Response.StatusDescription = "Unauthorized to access.";
                                        filterContext.HttpContext.Response.End();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                                if (!string.IsNullOrEmpty(UnAuthorizeView))
                                    filterContext.Result = new ViewResult { ViewName = UnAuthorizeView };
                                else
                                    filterContext.Result = new ViewResult { ViewName = "Module_NoPermissionToAcccess" };
                            else
                            {
                                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                                filterContext.HttpContext.Response.StatusDescription = "Unauthorized to access.";
                                filterContext.HttpContext.Response.End();
                            }
                        }
                    }
                    else
                    {
                        LISGlobal.CurrentModuleRole = new SysModuleRole
                        {
                            canread = true,
                            canwrite = true,
                            candelete = true,
                            full_control = true
                        };
                    }
                }
                else
                {
                    SysModuleRole modulePermission = new SysModuleRole
                    {
                        canaccess = true,
                        canread = true
                    };
                    LISGlobal.CurrentModuleRole = modulePermission;
                }
            }
        }

        internal void AjaxAuthorize(AuthorizationContext filterContext)
        {
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    bool isAuth = filterContext.HttpContext.User.Identity.IsAuthenticated;
                    if (!isAuth)
                    {
                        filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                        filterContext.HttpContext.Response.StatusDescription = "You don't have permission to access.";
                        filterContext.HttpContext.Response.End();
                    }
                    else
                    {
                        filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        filterContext.HttpContext.Response.StatusDescription = "Unauthorized to access.";
                        filterContext.HttpContext.Response.End();
                    }
                }
            }
        }
    }
}