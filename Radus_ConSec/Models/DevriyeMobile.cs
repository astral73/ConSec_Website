using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Radus_ConSec.Models
{
    public class DevriyeMobile
    {
        public string name { get; set; }
        public string desc { get; set; }
        public int id { get; set; }
        public List<List<int>> patrolItems { get; set; }
    }
}