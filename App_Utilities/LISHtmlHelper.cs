using lissystem.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace lissystem.App_Utilities
{
    public static class LISHtmlHelper
    {
        /// <summary>
        /// LIS Html ActionLink.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// LIS Html ModuleActionLink.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkUrl"></param>
        /// <param name="icon"></param>
        /// <param name="linkText"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISModuleLink(this HtmlHelper htmlHelper, string linkUrl, string icon, string linkText)
        {
            string moduleInnerLink = "";

            TagBuilder liTag = new TagBuilder("li");
            liTag.AddCssClass("nav-item");

            TagBuilder iTag = new TagBuilder("i");
            iTag.AddCssClass("bi " + icon);
            iTag.AddCssClass("nav-icon");
            if (!string.IsNullOrEmpty(icon))
                moduleInnerLink += iTag.ToString(TagRenderMode.Normal);

            TagBuilder pTag = new TagBuilder("p");
            pTag.InnerHtml = linkText;
            moduleInnerLink += pTag.ToString(TagRenderMode.Normal);

            TagBuilder aTag = new TagBuilder("a");
            aTag.Attributes.Add("href", linkUrl);
            aTag.AddCssClass("nav-link");
            aTag.InnerHtml = moduleInnerLink;

            liTag.InnerHtml = aTag.ToString(TagRenderMode.Normal);
            return MvcHtmlString.Create(liTag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// LIS Html ModuleActionLink.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="icon"></param>
        /// <param name="linkText"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISModuleLink(this HtmlHelper htmlHelper, string actionName, string controllerName, string icon, string linkText, object routeValues = null)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, RouteTable.Routes);
            string moduleLink = urlHelper.Action(actionName, controllerName, routeValues);
            return LISModuleLink(htmlHelper, moduleLink, icon, linkText);
        }

        /// <summary>
        /// LIS Html ModuleActionLink.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="RoleModules"></param>
        /// <param name="linkUrl"></param>
        /// <param name="icon"></param>
        /// <param name="linkText"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISModuleLink(this HtmlHelper htmlHelper, IEnumerable<ControlpanelRoleModule> RoleModules, string linkUrl, string icon, string linkText)
        {
            if (RoleModules.Count() == 0)
                return null;

            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, RouteTable.Routes);
            bool IsGranted = RoleModules.Any(p => urlHelper.Action(p.action_name, p.controller_name) == linkUrl);
            if (!IsGranted)
                return null;

            return LISModuleLink(htmlHelper, linkUrl, icon, linkText);
        }

        /// <summary>
        /// LIS Html ModuleActionLink.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="RoleModules"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="icon"></param>
        /// <param name="linkText"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISModuleLink(this HtmlHelper htmlHelper, IEnumerable<ControlpanelRoleModule> RoleModules, string actionName, string controllerName, string icon, string linkText, object routeValues = null)
        {
            if (RoleModules.Count() == 0) 
                return null;

            bool IsGranted = RoleModules.Any(p => p.action_name == actionName && p.controller_name == controllerName);
            if (!IsGranted)
                return null;

            return LISModuleLink(htmlHelper, actionName, controllerName, icon, linkText, routeValues);
        }

        /// <summary>
        /// LIS Html Sub ModuleActionLink
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="RoleModules"></param>
        /// <param name="subModule"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="icon"></param>
        /// <param name="linkText"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static MvcHtmlString LISSubModuleLink(this HtmlHelper htmlHelper, IEnumerable<ControlpanelRoleModule> RoleModules, string subModule, string actionName, string controllerName, string icon, string linkText, object routeValues = null)
        {
            if (RoleModules.Count() == 0)
                return null;
            bool IsGranted = RoleModules.Any(p => p.submodule_section == subModule && p.action_name == actionName && p.controller_name == controllerName);
            if (!IsGranted)
                return null;

            return LISModuleLink(htmlHelper, actionName, controllerName, icon, linkText, routeValues);
        }

        /// <summary>
        /// LIS Html TextBoxFor model binding.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var member = expression.Body as MemberExpression;
            var htmlAttr = (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes);

            var required = member.Member.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            if (required != null)
            {
                htmlAttr.Add("required", "required");
            }

            var maxLength = member.Member.GetCustomAttributes(typeof(MaxLengthAttribute), false).FirstOrDefault();
            if (maxLength != null)
            {
                htmlAttr.Add("maxlength", (maxLength as MaxLengthAttribute).Length);
            }

            object htmlAttrVal;
            if (htmlAttr.TryGetValue("readonly", out htmlAttrVal))
            {
                if (htmlAttrVal.GetType() == typeof(bool) && !(bool)htmlAttrVal)
                {
                    htmlAttr.Remove("readonly");
                }
            }

            if (htmlAttr.TryGetValue("disabled", out htmlAttrVal))
            {
                if (htmlAttrVal.GetType() == typeof(bool) && !(bool)htmlAttrVal)
                {
                    htmlAttr.Remove("disabled");
                }
            }

            return htmlHelper.TextBoxFor(expression, htmlAttr);
        }

        /// <summary>
        /// LIS Html LISPasswordFor model binding.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var member = expression.Body as MemberExpression;
            var htmlAttr = (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes);

            
            var maxLength = member.Member.GetCustomAttributes(typeof(MaxLengthAttribute), false).FirstOrDefault();
            if (maxLength != null)
            {
                htmlAttr.Add("maxlength", (maxLength as MaxLengthAttribute).Length);
            }

            object htmlAttrVal;
            if (htmlAttr.TryGetValue("readonly", out htmlAttrVal))
            {
                if (htmlAttrVal.GetType() == typeof(bool) && !(bool)htmlAttrVal)
                {
                    htmlAttr.Remove("readonly");
                }
            }

            if (htmlAttr.TryGetValue("disabled", out htmlAttrVal))
            {
                if (htmlAttrVal.GetType() == typeof(bool) && !(bool)htmlAttrVal)
                {
                    htmlAttr.Remove("disabled");
                }
            }

            return htmlHelper.PasswordFor(expression, htmlAttr);
        }

        /// <summary>
        /// LIS Html TextCheckBoxFor model binding.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            var member = expression.Body as MemberExpression;
            var htmlAttr = (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes);

            object htmlAttrVal;
            if (htmlAttr.TryGetValue("disabled", out htmlAttrVal))
            {
                if (htmlAttrVal.GetType() == typeof(bool) && !(bool)htmlAttrVal)
                {
                    htmlAttr.Remove("disabled");
                }
            }

            return htmlHelper.CheckBoxFor(expression, htmlAttr);
        }

        /// <summary>
        /// LIS Html DropDownListFor model binding.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LISDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            var member = expression.Body as MemberExpression;
            var htmlAttr = (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes);

            object htmlAttrVal;
            if (htmlAttr.TryGetValue("disabled", out htmlAttrVal))
            {
                if (htmlAttrVal.GetType() == typeof(bool) && !(bool)htmlAttrVal)
                {
                    htmlAttr.Remove("disabled");
                }
            }

            return htmlHelper.DropDownListFor(expression, selectList, null, htmlAttributes);
        }

        /// <summary>
        /// LIS UrlHelper Generate Action to URL.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static string LISAction(this UrlHelper urlHelper, string actionName)
        {
            return urlHelper.Action(actionName);
        }

        /// <summary>
        /// LIS UrlHelper Generate Action to URL.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string LISAction(this UrlHelper urlHelper, string actionName, object routeValues)
        {
            return urlHelper.Action(actionName, routeValues);
        }

        /// <summary>
        /// LIS UrlHelper Generate Action to URL.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="actionName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string LISAction(this UrlHelper urlHelper, string actionName, RouteValueDictionary routeValues)
        {
            return urlHelper.Action(actionName, routeValues);
        }

        /// <summary>
        /// LIS UrlHelper Generate Action to URL.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public static string LISAction(this UrlHelper urlHelper, string actionName, string controllerName)
        {
            return urlHelper.Action(actionName, controllerName);
        }

        /// <summary>
        /// LIS UrlHelper Generate Action to URL.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string Action(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues)
        {
            return urlHelper.Action(actionName, controllerName, routeValues);
        }

        /// <summary>
        /// LIS UrlHelper Generate Action to URL.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static string Action(this UrlHelper urlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return urlHelper.Action(actionName, controllerName, routeValues);
        }
    }
}