using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lissystem.Models.Ajax
{
    /*
     * https://datatables.net/manual/server-side
     */
    public class JsonDatatableResult
    {
        public long draw { get; set; } = 0;
        public int recordsTotal { get; set; } = 0;
        public object data { get; set; }
        public string error { get;set; } = "";
    }
}