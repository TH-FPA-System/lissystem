using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace lissystem.App_Interface
{
    public class LISGlobalErrorHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            if (filterContext.ExceptionHandled)
                return;
            Exception exception = filterContext.Exception;
            string controllerRequest = filterContext.RouteData.GetRequiredString("controller");
            string actionRequest = filterContext.RouteData.GetRequiredString("action");

            string messageStr = "";
            string innerMessageStr = "";
            if (exception is SqlException)
                messageStr = "Database error";
            else
                messageStr = exception.Message;
            if (exception.InnerException != null)
            {
                if (exception.InnerException is SqlException)
                    innerMessageStr = "Database error";
                else
                    innerMessageStr = exception.InnerException.Message;
            }

            SystemError systemError = new SystemError
            {
                controller = controllerRequest,
                action = actionRequest,
                message = messageStr,
                innerMessage = innerMessageStr
            };

            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/SystemError/Error.cshtml",
                    ViewData = new ViewDataDictionary(systemError)
                };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        status = LISGlobal.AJAX_RESULT_ERROR,
                        message = messageStr,
                        data = systemError
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue
                };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}