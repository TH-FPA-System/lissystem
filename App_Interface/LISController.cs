using lissystem.App_DataContext;
using lissystem.Models;
using lissystem.Models.Ajax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Services.Description;
using WebMarkupMin.AspNet4.Mvc;

namespace lissystem.App_Interface
{
    [CompressContent, MinifyHtml]
    public class LISController : Controller
    {
        public BaseDBContext BaseDB { get; private set; }
        public ThailisDBContext ThailisDB { get; private set; }

        public LISController()
        {
            BaseDB = new BaseDBContext();
            ThailisDB = new ThailisDBContext();

            LISGlobal.BaseDB = BaseDB;
            LISGlobal.ThailisDB = ThailisDB;
        }

        [Route("")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "CPHome");
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            return base.BeginExecute(requestContext, callback, state);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            //base.OnException(filterContext);
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
                errorStatus = (int)HttpStatusCode.InternalServerError,
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

        public JsonResult JsonResult(string status, string message, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return JsonResult(status, message, null, statusCode);
        }

        public JsonResult JsonResult(string status, object data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return JsonResult(status, "", data, statusCode);
        }

        public JsonResult JsonResult(string status, string message, object data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Response.StatusCode = (int)statusCode;
            var jsonData = new JsonResultMessage
            {
                status = status,
                message = message,
                data = data
            };
            return new LISLargeJsonResult(jsonData);
        }

        public JsonResult JsonForDataTable(JsonDatatableResult jsonDatatableResult)
        {
            return new LISLargeJsonResult(jsonDatatableResult);
        }

        public JsonResult JsonForDataTable(object data, int recordsTotal)
        {
            return new LISLargeJsonResult(new JsonDatatableResult
            {
                recordsTotal = recordsTotal,
                data = data,
            });
        }

        public JsonResult JsonForDataTable(long draw, object data, int recordsTotal)
        {
            return new LISLargeJsonResult(new JsonDatatableResult
            {
                draw = draw,
                recordsTotal = recordsTotal,
                data = data,
            });
        }

        public JsonResult JsonErrorForDataTable(string error)
        {
            return Json(new JsonDatatableResult
            {
                error = error
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonErrorForDataTable(long draw, string error)
        {
            return Json(new JsonDatatableResult
            {
                draw = draw,
                error = error
            }, JsonRequestBehavior.AllowGet);
        }

        public string JsonDataObjectSerialize(object data, JsonSerializerSettings serializerSettings)
        {
            return JsonConvert.SerializeObject(data, Formatting.None, serializerSettings);
        }

        public string JsonDataObjectSerialize(object data, ReferenceLoopHandling referenceLoop = ReferenceLoopHandling.Ignore)
        {
            return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = referenceLoop });
        }

        /*
         * To support server-side processing for DataTables
         * -----------------------------------------------------------------------------------------------------------------------------------------------------
         * var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var userAccounts = await UserAccountService.UserAccountAll();
            if (!string.IsNullOrEmpty(searchValue))
            {
                userAccounts = userAccounts.Where(m => m.account_name.Contains(searchValue)
                                        || m.Information.full_name.Contains(searchValue)
                                        || m.Information.email_address.Contains(searchValue)
                                        || m.Information.Rule.rule_name.Contains(searchValue)
                                        );
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                // Note: System.Linq.Dynamic library
                userAccounts = userAccounts.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir);
            }
            recordsTotal = userAccounts.Count();
            var data = userAccounts.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        */

        public void SetPreventBackButton()
        {
            ViewBag.PreventBackButtonService = true;
            TempData["PreventBackButtonService"] = true;
        }
    }
}