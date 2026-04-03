using lissystem.App_DataContext;
using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lissystem
{
    public static class LISGlobal
    {
        public const string LIS_AUTH_ENCKEY = "L1$$y$t3m";
        public const string LIS_AUTH_CACHEKEY = "L1$C@ch";

        public const string LIS_KEY_LOGINMODLE_TEMP = "LOGINMODLE";
        public const string LIS_KEY_LOGIN_TEMP = "LOGINTEMP";
        public enum LIS_LOGIN_ERROR
        {
            INVALID,
            INFOMATION_NOTFOUND,
            DISABLED,
            CANTLOGIN
        }

        public const string AJAX_RESULT_OK = "OK";
        public const string AJAX_RESULT_ERROR = "ERROR";
        public const string AJAX_RESULT_NOTINSERT = "NOTINSERT";
        public const string AJAX_RESULT_NOTUPDATE = "NOTUPDATE";
        public const string AJAX_RESULT_NOTDELETE = "NOTDELETE";
        public const string AJAX_RESULT_NOTSELECT = "NOTSELECT";
        public const string AJAX_RESULT_NOTACCEPT = "NOTACCEPT";

        public static BaseDBContext BaseDB { get; set; }
        public static ThailisDBContext ThailisDB { get; set; }


        public static DateTime LISMinDate { get { return new DateTime(1991, 1, 1); } }
        public static DateTime LISMinDateTime { get { return new DateTime(1991, 1, 1, 0, 0, 0); } }
        public static DateTime LISMaxDate { get { return new DateTime(2099, 12, 31); } }
        public static DateTime LISMaxDateTime { get { return new DateTime(2099, 12, 31, 23, 59, 59); } }

        public static SysModuleRole CurrentModuleRole { get; set; }

        /// <summary>
        /// System module section select list items.
        /// </summary>
        public static IEnumerable<SelectListItem> SystemModuleSectionSelectList
        {
            get
            {
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.Add(new SelectListItem { Value = "", Text = "--- Select Section ---" });
                selectListItems.Add(new SelectListItem { Value = "lis_module", Text = "LIS" });
                selectListItems.Add(new SelectListItem { Value = "planner_module", Text = "Planner" });
                selectListItems.Add(new SelectListItem { Value = "production_module", Text = "Production" });
                selectListItems.Add(new SelectListItem { Value = "factory_module", Text = "Factory" });
                selectListItems.Add(new SelectListItem { Value = "quality_module", Text = "Quality" });
                selectListItems.Add(new SelectListItem { Value = "management_module", Text = "Management" });
                selectListItems.Add(new SelectListItem { Value = "other_module", Text = "Other" });
                return selectListItems;
            }
        }

        /// <summary>
        /// Sale order status select list items.
        /// </summary>
        public static List<SelectListItem> SaleOrderStatusSelectList
        {
            get
            {
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.Add(new SelectListItem { Value = "H", Text = "Hold" });
                selectListItems.Add(new SelectListItem { Value = "A", Text = "Activate" });
                return selectListItems;
            }
        }
    }
}