using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Radus_ConSec.Models
{
    public class Patrol
    {
        public int PatrolID { get; set; }
        public int PatrolNum { get; set; }
        public string PatrolName { get; set; }
        public string PatrolDesc { get; set; }
        public DateTime PatrolInsertDate { get; set; }
        public DateTime PatrolEditDate { get; set; }
    }
}