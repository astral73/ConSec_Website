using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Radus_ConSec.Models
{
    public class DevriyeFormModel
    {
        public PATROL Patrol { get; set; } = new PATROL();
        public int routeID { get; set; }
        public int patrolID { get; set; }
        public List<string> routeStr { get; set; } = new List<string>();
        public string routeName { get; set; }
        public List<SelectListItem> lst { get; set; } = new List<SelectListItem>();
        public List<PATROLITEM> PatrolItems { get; set; } = new List<PATROLITEM>();
        public List<ROUTE> routes { get; set; } = new List<ROUTE>();
    }
}