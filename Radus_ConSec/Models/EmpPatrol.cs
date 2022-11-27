using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Radus_ConSec.Models
{
    public class EmpPatrol
    {
        public List<PATROLITEM> patrolitems { get; set; } = new List<PATROLITEM>();
        public List<PATROL> patrols { get; set; } = new List<PATROL>();
        public List<EMPLOYEE> employees { get; set; } = new List<EMPLOYEE>();
        public List<EMPLOYEEPATROL> empPatrol { get; set; } = new List<EMPLOYEEPATROL>();
        public int empID { get; set; }
        public int patrolID { get; set; }
        public float resumptionTime { get; set; }
        public string timeType { get; set; }
        public List<SelectListItem> lst { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> lst2 { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> lstTime { get; set; } = new List<SelectListItem>();
    }
}