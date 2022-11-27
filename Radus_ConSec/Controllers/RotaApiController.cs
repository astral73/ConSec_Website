using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Radus_ConSec.Models;

namespace Radus_ConSec.Controllers
{
    public class RotaApiController : ApiController
    {
        // Bu obje bizim database'e bağlanmamızı sağlıyor.
        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/rotaapi/Get)
        [Route("api/rotaapi/Get")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            // Rota bilgilerini burada bir listeye çeviriyoruz.
            var routes = db.ROUTE.ToList();

            // Eğer bu liste boşsa bir bilgi bulunamadı hatası dönüyoruz.
            if (routes == null)
            {
                return NotFound();
            }

            else
            {
                // Rota bilgilerinin kategorilere ayrılıp tutulması için listeler.
                List<int> routeIDs = new List<int>();
                List<string> routeDescs = new List<string>();
                List<string> routeNames = new List<string>();
                List<string> routeCodes = new List<string>();

                // Foreach döngüsüyle listelere bilgileri yolluyoruz.
                foreach (var rt in routes)
                {
                    routeIDs.Add(rt.ROUTEID);
                    routeDescs.Add(rt.ROUTEDESC);
                    routeNames.Add(rt.ROUTENAME);
                    routeCodes.Add(rt.ROUTECODE);
                }

                // Son olarak bu listeleri JSON formatında iletiyoruz.
                return Json(new {
                    routeIDs = routeIDs,
                    routeDescs = routeDescs,
                    routeNames = routeNames,
                    routeCodes = routeCodes
                });
            }
        }

        [HttpPost]
        [Route("api/rotaapi/Post")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/rotaapi/Post)
        public IHttpActionResult Post([FromBody] RotaModel rota)
        {
            // Aldığımız verileri veritabanına kaydetmemiz için kullandığımız değişkenler.
            string rt_code = "";

            int routeID;

            byte[] arr;

            // Eğer modelimiz bu işleme uygun değilse hata döndürüyoruz.

            if (!ModelState.IsValid)
            {
                return BadRequest("Hatalı veri!");
            }

            try
            {
                // Database'deki en yüksek değerdeki rota ID'sini almaya çalışıyoruz.
                routeID = db.ROUTE.Max(r => r.ROUTEID);
            }
            catch (Exception ex)
            {
                // Alamazsak rota ID'si otomatik olarak 0 oluyor.
                routeID = 0;
            }

            try
            {
                // Rota ID'sine uygun olarak bir de ona özel bir string kodu oluşturuyoruz. (Örneğin: Str - 001, Str - 010, Str - 100)
                if (routeID + 1 >= 0 && routeID + 1 < 10)
                {
                    rt_code = "Str" + " - " + "00" + (routeID + 1).ToString();
                }

                if (routeID + 1 >= 10 && routeID + 1 < 100)
                {
                    rt_code = "Str" + " - " + "0" + (routeID + 1).ToString();
                }

                if (routeID + 1 >= 100)
                {
                    rt_code = "Str" + " - " + (routeID + 1).ToString();
                }

                //Bu kod satırları bizim rota ID'si ve rota kodunu kullanarak bir QR kod üretiyor.

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCode = qrGenerator.CreateQrCode((routeID + 1) + "\n" + rt_code, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap bitMap = new QRCode(qrCode).GetGraphic(20))
                {
                    arr = ImageExtensions.ToByteArray(bitMap, ImageFormat.Bmp);
                }

                // Veri tabanı için bir değişken oluşturup bilgilerimizi oraya atıyoruz.
                var newRoute = new ROUTE
                {
                    ROUTEID = routeID + 1, // Her zaman max ID'nin bir fazlasını ID olarak atıyoruz.
                    ROUTENAME = rota.name,
                    ROUTEDESC = rota.desc,
                    ROUTEEDITDATE = DateTime.Now,
                    ROUTEINSERTDATE = DateTime.Now,
                    ROUTEQR = arr,
                    ROUTECODE = rt_code,
                };

                // Rotayı veritabanına atıp kaydediyoruz.
                db.ROUTE.Add(newRoute);
                db.SaveChanges();
            }

            // Eğer veri tabanında atma işleminde bir hata olursa hata mesajı döndürüyoruz.
            catch (Exception x)
            {
                return BadRequest(x.Message);
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Rota ekleme işlemi başarılı!");
        }

        [HttpPut]
        [Route("api/rotaapi/Put")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/rotaapi/Put)
        public IHttpActionResult Put([FromBody] RotaModel rota)
        {
            var idInt = Convert.ToInt32(rota.id); // Android uygulamasından gelen veriyi kullanılabilir hale getiriyoruz.
            var route = db.ROUTE.Find(idInt); // Bu ID'ye ait rotayı buluyoruz.

            // Eğer öyle bir rota yoksa bulunamadı hatasını döndürüyoruz.
            if (route == null)
            {
                return NotFound();
            }

            // Bulduğumuz rotaya ait bilgileri güncellemeye çalışıyoruz.
            try
            {
                db.ROUTE.Where(m => m.ROUTEID == rota.id).FirstOrDefault().ROUTENAME = rota.name;
                db.ROUTE.Where(m => m.ROUTEID == rota.id).FirstOrDefault().ROUTEDESC = rota.desc;
                db.ROUTE.Where(m => m.ROUTEID == rota.id).FirstOrDefault().ROUTEEDITDATE = DateTime.Now;

                db.SaveChanges();
            }

            // Eğer bir hata olursa hata mesajını dönüyoruz.
            catch (Exception x)
            {
                return BadRequest(x.Message);
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Rota güncelleme işlemi başarılı!");
        }

        [HttpDelete]
        [Route("api/rotaapi/Delete")] // Bu bizim api route'umuz. Bunu kullanarak isteğimize göre sonucu alabiliyoruz. (Örneğin: http://localhost:port/api/rotaapi/Delete)
        public IHttpActionResult Delete([FromBody] RotaModel rota)
        {
            // Gelen ID'ye ait rotayı buluyoruz.
            var route = db.ROUTE.Find(rota.id);

            // Eğer öyle bir rota yoksa bulunamadı hatasını döndürüyoruz.
            if (route == null)
            {
                return NotFound();
            }

            // Bu rota bir devriyenin içinde var mı diye kontrol ediyoruz.
            var routePatItem = db.PATROLITEM.FirstOrDefault(m => m.ROUTEID == rota.id);

            // Eğer varsa bu rotayı silemeyiz. Hata dönüyoruz.
            if (routePatItem != null)
            {
                return BadRequest("Rota grubu içindeki rotayı silemezsiniz!");
            }

            // Bir sıkıntı yoksa silme işlemini yapıp kaydediyoruz.
            try
            {
                db.ROUTE.Remove(route);
                db.SaveChanges();
            }
            catch(Exception x) 
            { 
                return BadRequest(x.Message); 
            }

            // Bitme mesajımız ile fonksiyonu bitiriyoruz. (200 OK)
            return Ok("Rota silme işlemi başarılı!");
        }
    }
}
