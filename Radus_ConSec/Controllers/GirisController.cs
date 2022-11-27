using System.Linq;
using System.Web.Mvc;
using Radus_ConSec.Models;
using System.Web.Security;

namespace Radus_ConSec.Controllers
{
    public class GirisController : Controller
    {
        private static bool IsValidEmailAddress(string emailAddress)
        {
            return new System.ComponentModel.DataAnnotations
                                .EmailAddressAttribute()
                                .IsValid(emailAddress);
        }

        public ActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Giris(Admin adm)
        {
            if (ModelState.IsValid)
            {
                using(RDS_ConSecEntities1 db = new RDS_ConSecEntities1())
                {
                    var obj = db.ADMIN.Where(a => a.ADMINEMAIL.Equals(adm.AdminEmail) && a.ADMINPASSWORD.Equals(adm.AdminPassword)).FirstOrDefault();
                    if (obj != null && IsValidEmailAddress(adm.AdminEmail) == true)
                    {
                        Session["AdminID"] = obj.ADMINID.ToString();
                        Session["AdminName"] = obj.ADMINNAME.ToString();

                        FormsAuthentication.SetAuthCookie(adm.AdminEmail, false);

                        if (Session["AdminID"] != null)
                        {
                            return RedirectToAction("Anasayfa", "Yonetici");
                        }
                    }
                }
            }
            return View(adm);
        }
    }
}