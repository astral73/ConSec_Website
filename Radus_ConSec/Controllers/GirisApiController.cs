using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Radus_ConSec.Controllers
{
    public class GirisApiController : ApiController
    {
        // Bu obje bizim database'e bağlanmamızı sağlıyor.
        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/girisapi/Get)
        [Route("api/girisapi/Get")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            // Giriş bilgilerini burada bir listeye çeviriyoruz.
            var admins = db.ADMIN.ToList();
            var employees = db.EMPLOYEE.ToList();

            // Eğer bu listeler boşsa bir bilgi bulunamadı hatası dönüyoruz.
            if (admins == null)
            {
                return NotFound();
            }

            else
            {
                // Giriş bilgilernin kategorilere ayrılıp tutulması için listeler.
                List<string> admEmails = new List<string>();
                List<string> admPasswords = new List<string>();

                // Foreach döngüsüyle listelere bilgileri yolluyoruz.
                foreach (var admin in admins)
                {
                    admEmails.Add(admin.ADMINEMAIL);
                    admPasswords.Add(admin.ADMINPASSWORD);
                }

                // Giriş bilgilernin kategorilere ayrılıp tutulması için listeler.
                List<string> empEmails = new List<string>();
                List<string> empPasswords = new List<string>();

                // Foreach döngüsüyle listelere bilgileri yolluyoruz.
                foreach (var employee in employees)
                {
                    empEmails.Add(employee.EMPLOYEEEMAIL);
                    empPasswords.Add(employee.EMPLOYEEPASSWORD);
                }

                // Son olarak bu listeleri JSON formatında iletiyoruz.
                return Json(new
                {
                    adminEmails = admEmails,
                    adminPasswords = admPasswords,
                    employeeEmails = empEmails,
                    employeePasswords = empPasswords
                });
            }
        }
    }
}
