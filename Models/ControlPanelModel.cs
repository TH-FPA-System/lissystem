using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models
{
    public class ControlPanelModel
    {
        public UserAccountInformation UserAccountInfo { get; set; }
        public IEnumerable<ControlpanelRoleModule> RoleModules { get; set; }
        
        public ControlPanelModel() {
            RoleModules = new List<ControlpanelRoleModule>();
        }

        /// <summary>
        /// Is module section granted for role of access.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool IsModuleSectionGrant(string section)
        {
            return RoleModules.Any(p => p.module_section == section);
        }

        /// <summary>
        /// Is module granted for role of account.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool IsModuleGrant(string controller, string action)
        {
            return RoleModules.Any(p => p.controller_name == controller && p.action_name == action);
        }
    }

    public class ControlpanelRoleModule
    { 
        public Guid account_id { get; set; }
        public Guid module_id { get; set; }
        public string module_section { get; set; }
        public string submodule_section { get; set; }
        public string module_name { get; set; }
        public string module_description { get; set; }
        public string controller_name { get; set; }
        public string action_name { get; set; }
    }

}