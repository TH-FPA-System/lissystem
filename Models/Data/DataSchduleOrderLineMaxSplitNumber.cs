using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lissystem.Models.Data
{
    public class DataSchduleOrderLineMaxSplitNumber
    {
        public int order_no { get; set; }
        public int line_no { get; set; }
        public short split_no { get; set; }
    }
}