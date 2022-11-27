using System;
using System.Linq;
using System.Web.Http;


namespace Radus_ConSec.Controllers
{
    // Deprecated (Kullanılmıyor!!!)
    public class ProfilApiController : ApiController
    {
        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        private static bool IsValidEmailAddress(string emailAddress)
        {
            return new System.ComponentModel.DataAnnotations
                                .EmailAddressAttribute()
                                .IsValid(emailAddress);
        }

        [HttpPut]
        [Route("api/profilapi/Put/{admID:int}/{email}/{password}/{name}/{lastname}")]
        public IHttpActionResult Put([FromUri] int admID, [FromUri] string email, [FromUri] string password, [FromUri] string name, [FromUri] string lastname)
        {
            var profile = db.ADMIN.Find(admID);

            if (profile == null)
            {
                return NotFound();
            }

            try
            {
                if (IsValidEmailAddress(email) == true)
                {
                    db.ADMIN.Where(m => m.ADMINID == admID).FirstOrDefault().ADMINEMAIL = email;
                }

                else
                {
                    return BadRequest("E-Posta alanına girilen bilgi bir e-posta değildir!");
                }

                db.ADMIN.Where(m => m.ADMINID == admID).FirstOrDefault().ADMINPASSWORD = password;
                db.ADMIN.Where(m => m.ADMINID == admID).FirstOrDefault().ADMINNAME = name;
                db.ADMIN.Where(m => m.ADMINID == admID).FirstOrDefault().ADMINLASTNAME = lastname;
                db.ADMIN.Where(m => m.ADMINID == admID).FirstOrDefault().ADMINEDITDATE = DateTime.Now;

                db.SaveChanges();
            }

            catch(Exception x)
            {
                return BadRequest(x.Message);
            }

            return Ok("Profil güncelleme işlemi başarılı!");
        }
    }
}
