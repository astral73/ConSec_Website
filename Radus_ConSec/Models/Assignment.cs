using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Radus_ConSec.Models
{
    public class Assignment
    {
        public int patrolID { get; set; }
        public int empID { get; set; }
        public string resumptionTime { get; set; }
        public string timeType { get; set; }
    }
}