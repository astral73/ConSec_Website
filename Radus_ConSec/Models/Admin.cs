using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Radus_ConSec.Models
{
    public class Admin
    {
        public int AdminID { get; set; }
        [DisplayName("e - Posta")]
        public string AdminEmail { get; set; }
        [DisplayName("Şifre")]
        public string AdminPassword { get; set; }
        public string AdminName { get; set; }
        public string AdminLastname { get; set; }
        public DateTime AdminInsertDate { get; set; }
        public DateTime AdminEditDate { get; set; }
    }
}