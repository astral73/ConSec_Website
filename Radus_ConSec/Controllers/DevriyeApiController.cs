using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Radus_ConSec.Models;

namespace Radus_ConSec.Controllers
{
    public class DevriyeApiController : ApiController
    {
        // Bu obje bizim database'e bağlanmamızı sağlıyor.
        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/devriyeapi/Get)
        [Route("api/devriyeapi/Get")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            // Devriye bilgilerini burada bir listeye çeviriyoruz.
            var patrols = db.PATROL.ToList();

            // Eğer bu liste boşsa bir bilgi bulunamadı hatası dönüyoruz.
            if (patrols == null)
            {
                return NotFound();
            }

            // Devriye bilgilerinin kategorilere ayrılıp tutulması için listeler.
            List<int> patrolIDs = new List<int>();
            List<string> patrolNums = new List<string>();
            List<string> patrolNames = new List<string>();
            List<string> patrolDescs = new List<string>();

            // Foreach döngüsüyle listelere bilgileri yolluyoruz.
            foreach (var pt in patrols)
            {
                patrolIDs.Add(pt.PATROLID);
                patrolNums.Add(pt.PATROLNUM);
                patrolNames.Add(pt.PATROLNAME);
                patrolDescs.Add(pt.PATROLDESC);
            }

            // Son olarak bu listeleri JSON formatında iletiyoruz.
            return Json(new { 
                patrolIDs = patrolIDs,
                patrolNums = patrolNums,
                patrolNames = patrolNames,
                patrolDescs = patrolDescs,
            });
        }

        [HttpPost]
        [Route("api/devriyeapi/Post")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/devriyeapi/Post)
        public IHttpActionResult Post([FromBody] DevriyeMobile dev)
        {
            // Aldığımız verileri veritabanına kaydetmemiz için kullandığımız değişken.
            int patrolID;
            try
            {
                // Database'deki en yüksek değerdeki devriye ID'sini almaya çalışıyoruz.
                patrolID = db.PATROL.Max(r => r.PATROLID);
            }
            catch
            {
                // Alamazsak devriye ID'si otomatik olarak 0 oluyor.
                patrolID = 0;
            }

            try
            {
                // Veri tabanı için bir değişken oluşturup bilgilerimizi oraya atıyoruz.
                var patrol = new PATROL
                {
                    PATROLID = patrolID + 1,
                    PATROLNAME = dev.name,
                    PATROLDESC = dev.desc,
                    PATROLNUM = "PTR" + " - " + (patrolID + 1).ToString(),
                    PATROLINSERTDATE = DateTime.Now,
                    PATROLEDITDATE = DateTime.Now
                };

                // Devriyeyi veritabanına atıyoruz.
                db.PATROL.Add(patrol);

                // Devriye rotalarını da ayni şekilde veri tabanına atıyoruz.
                for(int i = 0; i < dev.patrolItems.Count; i++)
                {
                    var patrolItem = new PATROLITEM
                    {
                        PATROLID = patrolID + 1,
                        ROUTEID = (dev.patrolItems[i])[0],
                        ROUTEROW = (dev.patrolItems[i])[1],
                    };

                    db.PATROLITEM.Add(patrolItem);
                }

                // Veritabanını kaydediyoruz.
                db.SaveChanges();
            }

            // Eğer veri tabanında atma işleminde bir hata olursa hata mesajı döndürüyoruz.
            catch (Exception x)
            {
                return BadRequest(x.Message);
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Devriye ekleme işlemi başarılı!");
        }

        [HttpPut]
        [Route("api/devriyeapi/Put")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/devriyeapi/Put)
        public IHttpActionResult Put(DevriyeMobile dev)
        {
            // Bu ID'ye ait devriyeyi buluyoruz.
            var patrol = db.PATROL.Find(dev.id);

            // Eğer öyle bir devriye yoksa bulunamadı hatasını döndürüyoruz.
            if (patrol == null)
            {
                return NotFound();
            }

            // Bulduğumuz devriyeye ait bilgileri güncellemeye çalışıyoruz.
            try
            {
                db.PATROL.Where(r => r.PATROLID == dev.id).FirstOrDefault().PATROLNAME = dev.name;
                db.PATROL.Where(r => r.PATROLID == dev.id).FirstOrDefault().PATROLDESC = dev.desc;
                db.PATROL.Where(r => r.PATROLID == dev.id).FirstOrDefault().PATROLEDITDATE = DateTime.Now;

                for (int i = 0; i < dev.patrolItems.Count; i++)
                {
                    var patrolItem = new PATROLITEM
                    {
                        PATROLID = dev.id,
                        ROUTEID = (dev.patrolItems[i])[0],
                        ROUTEROW = (dev.patrolItems[i])[1],
                    };

                    db.PATROLITEM.Add(patrolItem);
                }

                db.SaveChanges();
            }

            // Eğer bir hata olursa hata mesajını dönüyoruz.
            catch (Exception x)
            {
                return BadRequest(x.Message);
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Devriye bilgileri düzenleme işlemi başarılı!");
        }

        [HttpDelete]
        [Route("api/devriyeapi/Delete")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/devriyeapi/Delete)
        public IHttpActionResult Delete([FromBody] DevriyeMobile dev)
        {
            // Gelen ID'ye ait devriyeyi buluyoruz.
            var patrol = db.PATROL.FirstOrDefault(m => m.PATROLID == dev.id);

            // Eğer öyle bir devriye yoksa bulunamadı hatasını döndürüyoruz.
            if (patrol == null)
            {
                return NotFound();
            }

            // Bu devriye bir güvenliğe atanmış mı diye kontrol ediyoruz.
            var assignedPatrol = db.EMPLOYEEPATROL.FirstOrDefault(m => m.PATROLID == dev.id);

            // Eğer atanmışsa bu devriyeyi silemeyiz. Hata dönüyoruz.
            if (assignedPatrol != null)
            {
                return BadRequest("Bir güvenliğe atanmış rota grubunu silemezsiniz!");
            }

            // Bir sıkıntı yoksa silme işlemini yapıp kaydediyoruz.
            try
            {
                db.PATROL.Remove(patrol);

                var patrolItems = db.PATROLITEM.Where(a => a.PATROLID == dev.id).ToList();

                if(patrolItems != null)
                {
                    foreach (var item in patrolItems)
                    {
                        db.PATROLITEM.Remove(item);
                    }
                }

                db.SaveChanges();
            }

            catch (Exception x)
            {
                return BadRequest(x.Message);
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Devriye silme işlemi başarılı!");
        }
    }
}
