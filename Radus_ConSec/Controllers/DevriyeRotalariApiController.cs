using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Radus_ConSec.Models;

namespace Radus_ConSec.Controllers
{
    public class DevriyeRotalariApiController : ApiController
    {
        // Bu obje bizim database'e bağlanmamızı sağlıyor.
        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        [HttpGet]
        [Route("api/devriyerotalariapi/Get")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/devriyerotalariapi/Get)
        public IHttpActionResult Get()
        {
            // Devriyelerin içindeki rotaların bilgilerini burada bir listeye çeviriyoruz.
            var patrolItems = db.PATROLITEM.ToList();

            // Eğer bu liste boşsa bir bilgi bulunamadı hatası dönüyoruz.
            if (patrolItems == null)
            {
                return NotFound();
            }

            // Devriye rotalarının bilgilerinin kategorilere ayrılıp tutulması için listeler.
            List<string> patrolitems = new List<string>();
            string items;

            // Foreach döngüsüyle listelere bilgileri yolluyoruz.
            foreach (var pit in patrolItems)
            {
                items = pit.PATROLID.ToString() + ", " + pit.ROUTEID.ToString() + ", " + pit.ROUTEROW.ToString();

                patrolitems.Add(items);
            }

            // Son olarak bu listeleri JSON formatında iletiyoruz.
            return Json(new { 
                patrolitems = patrolitems
            });
        }

        [HttpDelete]
        [Route("api/devriyerotalariapi/Delete")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/devriyerotalariapi/Delete)
        public IHttpActionResult Delete([FromBody] PatrolItems dev)
        {
            // Gelen ID'ye ait devriye rotalarını buluyoruz.
            var pit = db.PATROLITEM.Where(m => m.PATROLID == dev.patrolID);

            // Eğer liste boşsa bulunamadı hatasını döndürüyoruz.
            if (pit == null)
            {
                return NotFound();
            }

            // Bir sıkıntı yoksa silme işlemini yapıp kaydediyoruz.
            try
            {
                db.PATROLITEM.RemoveRange(db.PATROLITEM.Where(x => x.PATROLID == dev.patrolID));
                db.SaveChanges();
            }
            catch (Exception x)
            {
                return BadRequest(x.Message);
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Rota silme işlemi başarılı!");
        }
    }
}
