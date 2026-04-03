using lissystem.App_Services;
using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;


namespace lissystem
{
    public static class LISRequest
    {
        /// <summary>
        /// Get curent http context.
        /// </summary>
        public static HttpContext CurrentHttpContext
        {
            get
            {
                return HttpContext.Current;
            }
        }

        /// <summary>
        /// Get current user principal.
        /// </summary>
        public static IPrincipal CurrentUser
        {
            get
            {
                return HttpContext.Current.User;
            }
        }

        /// <summary>
        /// Get authentication state.
        /// </summary>
        public static bool IsAuthenticate
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// Get current authentication user id.
        /// </summary>
        public static Guid CurrentAccountId
        {
            get
            {
                if (IsAuthenticate)
                {
                    return new Guid(HttpContext.Current.User.Identity.Name);
                }
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Get curent authentication user infomation.
        /// </summary>
        public static UserAccountInformation CurrentAccountInfo
        {
            get
            {
                if (IsAuthenticate)
                {
                    string cacheKey = $"{LISGlobal.LIS_AUTH_CACHEKEY}_{CurrentAccountId}";

                    UserAccountInformation info = HttpContext.Current.Cache.Get(cacheKey) as UserAccountInformation;
                    if (info != null)
                    {
                        return info;
                    }

                    info = LISGlobal.BaseDB.UserAccountInformations
                        .Where(p => p.account_id == CurrentAccountId)
                        .Include(p => p.UserAccount)
                        .Include(p => p.Role)
                        .FirstOrDefault();

                    if (info != null)
                    {
                        HttpContext.Current.Cache.Insert(cacheKey, info, null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration);
                        return info;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Get current authentication user code.
        /// </summary>
        public static string CurrentAccountCode
        {
            get
            {
                if (IsAuthenticate)
                {
                    return CurrentAccountInfo.code;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Get permission of system module.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns>SysModuleRole</returns>
        public static SysModuleRole GetModulePermission(string controller, string action)
        {
            if (IsAuthenticate)
            {
                if (action == "Index")
                    return new SysModuleService().GetModulePermissionForAccount(CurrentAccountId, controller, action);
                else
                    return new SysModuleService().GetModulePermissionForAccount(CurrentAccountId, controller);
            }
            return null;
        }

        /// <summary>
        /// Get current request IP Address.
        /// </summary>
        /// <returns>Request IP Address</returns>
        public static string GetRequestIPAddress()
        {
            HttpRequest request = HttpContext.Current?.Request;
            if (request != null)
            {
                string requestIP = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(requestIP))
                {
                    string[] ipAddresses = requestIP.Split(',');
                    return ipAddresses[0].Trim();
                }
                else
                {
                    return request.UserHostAddress;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Get current request Host name
        /// </summary>
        /// <returns>Request Hostname</returns>
        public static string GetRequestHostName()
        {
            HttpRequest request = HttpContext.Current?.Request;
            if (request != null)
            {
                try
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(request.UserHostName);
                    return hostEntry.HostName;
                }
                catch (Exception ex)
                {
                    try
                    {
                        return GetRequestIPAddress();
                    }
                    catch (Exception sEx)
                    {
                        return "Unknown";
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Get current request browser capabilities
        /// </summary>
        /// <returns>HttpBrowserCapabilities</returns>
        public static HttpBrowserCapabilities GetRequestBrowserCap()
        {
            HttpRequest request = HttpContext.Current?.Request;
            if (request != null)
            {
                return request.Browser;
            }
            return null;
        }

        /// <summary>
        /// Get current request browser engine
        /// </summary>
        /// <returns>Browser engine string</returns>
        public static string GetRequestBrowserEngine()
        {
            HttpRequest request = HttpContext.Current?.Request;
            if (request != null)
            {
                HttpBrowserCapabilities browser = request.Browser;
                string resultStr = string.Format("{0}-{1},{2}",
                    browser.Browser,
                    browser.MajorVersion + "." + browser.MinorVersion,
                    browser.Platform);
                return resultStr;
            }
            return string.Empty;
        }
    }
}