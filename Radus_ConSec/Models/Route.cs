using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Radus_ConSec.Models
{
    public class Route
    {
        public int RouteID { get; set; }
        public string RouteName { get; set; }
        public string RouteDesc { get; set; }
        public DateTime RouteInsertDate { get; set; }
        public DateTime RouteEditDate { get; set; }
        public string RouteCode { get; set; }
    }
}