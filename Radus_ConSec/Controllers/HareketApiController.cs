using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Radus_ConSec.Models;

namespace Radus_ConSec.Controllers
{
    public class HareketApiController : ApiController
    {
        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        // Bu obje bizim database'e bağlanmamızı sağlıyor.
        [HttpGet]
        [Route("api/hareketapi/Get")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/hareketapi/Get)
        public IHttpActionResult Get()
        {
            // Hareket bilgilerini eskide yeniye sıralı şekilde listeye alıyoruz.
            var movement = db.MOVEMENTS.OrderBy(m => m.TRANSTIME).ToList();

            // Eğer hiçbir bilgi yoksa bulunamadı mesajını dönüyoruz.
            if (movement == null)
            {
                return NotFound();
            }

            // Hareket bilgilerinin kategorilere ayrılıp tutulması için listeler.
            List<int> empIDs = new List<int>();
            List<int> routeIDs = new List<int>();
            List<int> patrolIDs = new List<int>();
            List<string> transDates = new List<string>();

            // Foreach döngüsüyle listelere bilgileri yolluyoruz.
            foreach (var m in movement)
            {
                empIDs.Add(m.EMPLOYEEID);
                routeIDs.Add(m.ROUTEID);
                patrolIDs.Add(m.PATROLID);
                transDates.Add(m.TRANSTIME.ToString());
            }

            // Son olarak bu listeleri JSON formatında iletiyoruz.
            return Json(new
            {
                empIDs = empIDs,
                routeIDs = routeIDs,
                patrolIDs = patrolIDs,
                transDates = transDates,
            });
        }

        [HttpPost]
        [Route("api/hareketapi/Post")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/hareketapi/Post)
        public IHttpActionResult Post(Movement move)
        {
            // Veri tabanından bilgileri alıyoruz.
            var mv = db.MOVEMENTS.Where(m => m.EMPLOYEEID == move.empID).OrderByDescending(m => m.TRANSTIME).FirstOrDefault();

            // Eğer son veri ile arasında 30 dakika yoksa kaydı yapmıyoruz.
            if (mv != null && (DateTime.Now - mv.TRANSTIME).TotalMinutes < 0.25)
            {
                return BadRequest("Yeni QR kodu okutmanız için en az 30 dakika beklemeniz gerekmektedir!");
            }


            try
            {
                // Veri tabanı için bir değişken oluşturup bilgilerimizi oraya atıyoruz.
                var movement = new MOVEMENTS
                {
                    EMPLOYEEID = move.empID,
                    ROUTEID = move.routeID,
                    PATROLID = move.patrolID,
                    TRANSTIME = DateTime.Now
                };

                // Hareket verilerini veritabanına atıp kaydediyoruz.
                db.MOVEMENTS.Add(movement);
                db.SaveChanges();
            }

            // Eğer veri tabanında atma işleminde bir hata olursa hata mesajı döndürüyoruz.
            catch (Exception x)
            {
                return BadRequest(x.Message);
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Hareket ekleme işlemi başarılı!");
        }
    }
}
