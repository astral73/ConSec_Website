using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Radus_ConSec.Models;

namespace Radus_ConSec.Controllers
{
    public class GuvenlikApiController : ApiController
    {
        // Bu obje bizim database'e bağlanmamızı sağlıyor.
        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        // Kullanılmıyor!!!
        private static bool IsValidEmailAddress(string emailAddress)
        {
            return new System.ComponentModel.DataAnnotations
                                .EmailAddressAttribute()
                                .IsValid(emailAddress);
        }
        // Kullanılmıyor!!!

        // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/guvenlikapi/Get)
        [Route("api/guvenlikapi/Get")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            // Güvenlik bilgilerini burada bir listeye çeviriyoruz.
            var emps = db.EMPLOYEE.ToList();

            // Eğer bu liste boşsa bir bilgi bulunamadı hatası dönüyoruz.
            if (emps == null)
            {
                return NotFound();
            }

            else
            {
                // Güvenlik bilgilernin kategorilere ayrılıp tutulması için listeler.
                List<int> employeeIDs = new List<int>();
                List<string> employeeMails = new List<string>();
                List<string> employeePasswords = new List<string>();
                List<string> employeeNames = new List<string>();
                List<string> employeeLastnames = new List<string>();
                List<DateTime> employeeInsertDates = new List<DateTime>();
                List<DateTime> employeeEditDates = new List<DateTime>();

                // Foreach döngüsüyle listelere bilgileri yolluyoruz.
                foreach (var emp in emps)
                {
                    employeeIDs.Add(emp.EMPLOYEEID);
                    employeeMails.Add(emp.EMPLOYEEEMAIL);
                    employeePasswords.Add(emp.EMPLOYEEPASSWORD);
                    employeeNames.Add(emp.EMPLOYEENAME);
                    employeeLastnames.Add(emp.EMPLOYEELASTNAME);
                    employeeInsertDates.Add(emp.EMPLOYEEINSERTDATE);
                    employeeEditDates.Add(emp.EMPLOYEEEDITDATE);
                }

                // Son olarak bu listeleri JSON formatında iletiyoruz.
                return Json(new 
                { 
                    employeeIDs = employeeIDs,
                    employeeMails = employeeMails,
                    employeePasswords = employeePasswords,
                    employeeNames = employeeNames,
                    employeeLastnames = employeeLastnames,
                });
            }
        }

        [HttpPost]
        [Route("api/guvenlikapi/Post")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/guvenlikapi/Post)
        public IHttpActionResult Post([FromBody]Security security)
        {
            // Aldığımız verileri veritabanına kaydetmemiz için kullandığımız değişken.
            int empID;

            // Eğer modelimiz bu işleme uygun değilse hata döndürüyoruz.
            if (!ModelState.IsValid)
            {
                return BadRequest("Hatalı veri!");
            }

            try
            {
                // Database'deki en yüksek değerdeki güvenlik ID'sini almaya çalışıyoruz.
                empID = db.EMPLOYEE.Max(r => r.EMPLOYEEID);
            }
            catch (Exception x)
            {
                // Alamazsak güvenlik ID'si otomatik olarak 0 oluyor.
                empID = 0;
            }

            try
            {
                // Veri tabanı için bir değişken oluşturup bilgilerimizi veritabanına atıyoruz.
                db.EMPLOYEE.Add(new EMPLOYEE()
                {
                    EMPLOYEEEMAIL = security.email,
                    EMPLOYEEPASSWORD = security.password,
                    EMPLOYEENAME = security.name,
                    EMPLOYEELASTNAME = security.lastname,
                    EMPLOYEEINSERTDATE = DateTime.Now,
                    EMPLOYEEEDITDATE = DateTime.Now,
                    EMPLOYEEID = empID + 1
                });

                // Veritabanını kaydediyoruz.
                db.SaveChanges();
            }

            // Eğer veri tabanında atma işleminde bir hata olursa hata mesajı döndürüyoruz.
            catch (Exception x) 
            { 
                return BadRequest(x.Message); 
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Güvenlik ekleme işlemi başarılı!");
        }

        [HttpDelete]
        [Route("api/guvenlikapi/Delete")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/guvenlikapi/Delete)
        public IHttpActionResult Delete([FromBody] Security security)
        {
            // Gelen ID'ye ait güvenliği buluyoruz.
            var emp = db.EMPLOYEE.Find(security.id);

            // Eğer öyle bir güvenlik yoksa bulunamadı hatasını döndürüyoruz.
            if (emp == null)
            {
                return NotFound();
            }

            // Bu güvenliğe bir devriye atanmış mı diye kontrol ediyoruz.
            var empPat = db.EMPLOYEEPATROL.Find(security.id);

            // Eğer atanmışsa bu güvenliği silemeyiz. Hata dönüyoruz.
            if (empPat != null)
            {
                return BadRequest("Rota grubu atanmış güvenliği silemezsiniz!");
            }

            // Bir sıkıntı yoksa silme işlemini yapıp kaydediyoruz.
            try
            {
                db.EMPLOYEE.Remove(emp);
                db.SaveChanges();
            }

            catch(Exception x) 
            { 
                return BadRequest(x.Message);
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Güvenlik silme işlemi başarılı!");
        }

        [HttpPut]
        [Route("api/guvenlikapi/Put")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/guvenlikapi/Put)
        public IHttpActionResult Put([FromBody]Security security)
        {
            var idInt = Convert.ToInt32(security.id); // Android uygulamasından gelen veriyi kullanılabilir hale getiriyoruz.
            var emp = db.EMPLOYEE.Find(idInt); // Bu ID'ye ait güvenliği buluyoruz.

            // Eğer öyle bir güvenlik yoksa bulunamadı hatasını döndürüyoruz.
            if (emp == null)
            {
                return NotFound();
            }

            // Bulduğumuz güvenliğe ait bilgileri güncellemeye çalışıyoruz.
            try
            {
                db.EMPLOYEE.Where(m => m.EMPLOYEEID == idInt).FirstOrDefault().EMPLOYEEEMAIL = security.email;
                db.EMPLOYEE.Where(m => m.EMPLOYEEID == idInt).FirstOrDefault().EMPLOYEEPASSWORD = security.password;
                db.EMPLOYEE.Where(m => m.EMPLOYEEID == idInt).FirstOrDefault().EMPLOYEENAME = security.name;
                db.EMPLOYEE.Where(m => m.EMPLOYEEID == idInt).FirstOrDefault().EMPLOYEELASTNAME = security.lastname;
                db.EMPLOYEE.Where(m => m.EMPLOYEEID == idInt).FirstOrDefault().EMPLOYEEEDITDATE = DateTime.Now;

                db.SaveChanges();
            }

            // Eğer bir hata olursa hata mesajını dönüyoruz.
            catch (Exception x) 
            { 
                return BadRequest(x.Message); 
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Güvenlik güncelleme işlemi başarılı!");
        }
    }
}
