using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lissystem.Models.Form
{
    public class FormSystemAccountView
    {
        public UserAccount UserAccount { get; set; }
        public UserAccountPasswordUpdate UserAccountSecurity { get; set; }

        public IEnumerable<SelectListItem> RoleSelectListItem { get; set; }

        public FormSystemAccountView() {
            RoleSelectListItem = new List<SelectListItem>();
        }
    }
}