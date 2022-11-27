using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.Json.Serialization;

namespace Radus_ConSec.Models
{
    public class Security
    {
        public string email { get; set; }

        public string password { get; set; }

        public string name { get; set; }

        public string lastname { get; set; }

        public int id { get; set; }
    }
}