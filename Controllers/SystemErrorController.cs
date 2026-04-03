using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Controllers
{
    [RoutePrefix("error")]
    public class SystemErrorController : Controller
    {
        [Route("")]
        public ActionResult Error(string aspxerrorpath = "")
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
                Server.ClearError();
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
                message = messageStr,
                innerMessage = innerMessageStr
            };

            return View(systemError);
        }

        [Route("notfound")]
        public ActionResult Error404(string aspxerrorpath = "")
        {
            SystemError systemError = new SystemError
            {
                errorStatus = (int)HttpStatusCode.NotFound,
            };
            return View("Error", systemError);
        }

        [Route("general")]
        public ActionResult Error500(string aspxerrorpath = "")
        {
            SystemError systemError = new SystemError
            {
                errorStatus = (int)HttpStatusCode.InternalServerError,
                message = "....",
                innerMessage = "------"
            };
            return View("Error", systemError);
        }
    }
}