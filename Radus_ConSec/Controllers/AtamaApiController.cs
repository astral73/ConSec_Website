using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Radus_ConSec.Models;

namespace Radus_ConSec.Controllers
{
    public class AtamaApiController : ApiController
    {
        // Bu obje bizim database'e bağlanmamızı sağlıyor.
        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/atamaapi/Get)
        [Route("api/atamaapi/Get")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            // Atama bilgilerini burada bir listeye çeviriyoruz.
            var assignedPatrols = db.EMPLOYEEPATROL.ToList();

            // Eğer bu liste boşsa bir bilgi bulunamadı hatası dönüyoruz.
            if (assignedPatrols == null)
            {
                return NotFound();
            }

            // Atama bilgilerinin kategorilere ayrılıp tutulması için listeler.
            List<int> employeeIDs = new List<int>();
            List<int> patrolIDs = new List<int>();
            List<double> resumptionTimes = new List<double>();

            // Foreach döngüsüyle listelere bilgileri yolluyoruz.
            foreach (var asPat in assignedPatrols)
            {
                employeeIDs.Add(asPat.EMPLOYEEID);
                patrolIDs.Add(asPat.PATROLID);
                resumptionTimes.Add(asPat.RESUMPTIONTIME);
            }

            // Son olarak bu listeleri JSON formatında iletiyoruz.
            return Json(new { 
                employeeIDs = employeeIDs,
                patrolIDs = patrolIDs,
                resumptionTimes = resumptionTimes
            });
        }

        [HttpPost]
        [Route("api/atamaapi/Post")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/atamaapi/Post)
        public IHttpActionResult Post(Assignment assign)
        {
            // Bu ID'ye ait atama var mı diye bakıyoruz.
            var temp = db.EMPLOYEEPATROL.Find(assign.empID);
            var resTime = Int32.Parse(assign.resumptionTime);

            // Eğer varsa hata mesajını döndürüyoruz.
            if(temp != null)
            {
                return BadRequest("Güvenliğe daha önce bir atama yapılmış");
            }

            // Gelen zaman tipine göre atama zamanını belirliyoruz. Sonrasında veriyi veri tabanına atıp kaydediyoruz.
            try
            {
                if (assign.timeType == "Saat")
                {
                    resTime = resTime * 60;
                }

                if (assign.timeType == "Gün")
                {
                    resTime = resTime * 24 * 60;
                }

                var assignment = new EMPLOYEEPATROL
                {
                    EMPLOYEEID = assign.empID,
                    PATROLID = assign.patrolID,
                    RESUMPTIONTIME = resTime
                };

                db.EMPLOYEEPATROL.Add(assignment);
                db.SaveChanges();
            }

            // Eğer veri tabanında atma işleminde bir hata olursa hata mesajı döndürüyoruz.
            catch (Exception x)
            {
                return BadRequest(x.ToString());
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Yeni atama işlemi başarıyla tamamlandı!");
        }

        [HttpPut]
        [Route("api/atamaapi/Put")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/atamaapi/Put)
        public IHttpActionResult Put([FromBody] Assignment assign)
        {
            // Bu ID'ye ait atamayı buluyoruz.
            var assignment = db.EMPLOYEEPATROL.Find(assign.empID);

            var resTime = Int32.Parse(assign.resumptionTime);

            // Eğer öyle bir atama yoksa bulunamadı hatasını döndürüyoruz.
            if (assignment == null)
            {
                return NotFound();
            }

            // Gelen zaman tipine göre atama zamanını belirliyoruz. Sonrasında bu atamaya ait bilgileri güncellemeye çalışıyoruz.
            try
            {
                if (assign.timeType == "Saat")
                {
                    resTime = resTime * 60;
                }

                if (assign.timeType == "Gün")
                {
                    resTime = resTime * 24 * 60;
                }

                db.EMPLOYEEPATROL.Where(m => m.EMPLOYEEID == assign.empID).FirstOrDefault().PATROLID = assign.patrolID;
                db.EMPLOYEEPATROL.Where(m => m.EMPLOYEEID == assign.empID).FirstOrDefault().RESUMPTIONTIME = resTime;
                db.SaveChanges();
            }

            // Eğer bir hata olursa hata mesajını dönüyoruz.
            catch (Exception x)
            {
                return BadRequest(x.ToString());
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Atama düzenleme işlemi başarılı!");
        }

        [HttpDelete]
        [Route("api/atamaapi/Delete")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/atamaapi/Delete)
        public IHttpActionResult Delete([FromBody] Assignment assign)
        {
            // Gelen ID'ye ait atamayı buluyoruz.
            var assignment = db.EMPLOYEEPATROL.Find(assign.empID);

            // Eğer öyle bir atama yoksa bulunamadı hatasını döndürüyoruz.
            if (assignment == null)
            {
                return NotFound();
            }

            // Bir sıkıntı yoksa silme işlemini yapıp kaydediyoruz.
            try
            {
                db.EMPLOYEEPATROL.Remove(assignment);
                db.SaveChanges();
            }

            catch (Exception x)
            {
                return BadRequest(x.ToString());
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Atama silme işlemi başarılı!");
        }
    }
}
