using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Radus_ConSec.Models
{
    public class Employee
    {
        [DisplayName("Güvenlik IDsi")]
        public int EmployeeID { get; set; }
        
        [DisplayName("Güvenlik İsmi")]
        public string EmployeeName { get; set; }
        
        [DisplayName("Güvenlik Soyismi")]
        public string EmployeeLastname { get; set; }

        public DateTime EmployeeInsertDate { get; set; }

        public DateTime EmployeeEditDate { get; set; }

        [DisplayName("Güvenlik e - Postası")]
        public string EmployeeEmail { get; set; }
        
        [DisplayName("Güvenlik Şifresi")]
        public string EmployeePassword { get; set; }
    }
}