using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Radus_ConSec.Models;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace Radus_ConSec.Controllers
{
    [Authorize]
    public class YoneticiController : Controller
    {
        public List<int> routePatrol = new List<int>();

        private RDS_ConSecEntities1 db = new RDS_ConSecEntities1();

        private static bool IsValidEmailAddress(string emailAddress)
        {
            return new System.ComponentModel.DataAnnotations
                                .EmailAddressAttribute()
                                .IsValid(emailAddress);
        }

        // Main Page Controller Functions

        public ActionResult Anasayfa()
        {
            return View();
        }

        public ActionResult Cikis()
        {
            Session["AdminID"] = null;
            Session["AdminName"] = null;
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Giris", "Giris");
        }

        // BEGIN: Functions For Employees (Security Guards)

        [HttpGet]
        public ActionResult Guvenlik()
        {
            var data = db.EMPLOYEE.ToList();
            ViewBag.empDetails = data;
            return View();
        }

        public ActionResult GuvenlikEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuvenlikEkle(Employee employee)
        {

            try
            {
                employee.EmployeeID = db.EMPLOYEE.Max(r => r.EMPLOYEEID);
            }
            catch(Exception x)
            {
                employee.EmployeeID = 1;
            }

            if (ModelState.IsValid)
            {
                var emp = new EMPLOYEE
                {
                    EMPLOYEEID = employee.EmployeeID + 1,
                    EMPLOYEENAME = employee.EmployeeName,
                    EMPLOYEELASTNAME = employee.EmployeeLastname,
                    EMPLOYEEEMAIL = employee.EmployeeEmail,
                    EMPLOYEEPASSWORD = employee.EmployeePassword,
                    EMPLOYEEEDITDATE = DateTime.Now,
                    EMPLOYEEINSERTDATE = DateTime.Now
                };
                try
                {
                    db.EMPLOYEE.Add(emp);
                    db.SaveChanges();
                }
                catch(Exception x)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                return RedirectToAction("Guvenlik", "Yonetici");
            }
            return View(employee);
        }

        [HttpPost]
        public JsonResult GuvenlikSil(int? id)
        {
            if (id == null)
            {
                return Json(false);
            }

            var emp = db.EMPLOYEE.Find(id);

            if (emp == null)
            {
                return Json("Güvenlik bulunamadı!");
            }

            var empPat = db.EMPLOYEEPATROL.Find(id);

            if(empPat != null)
            {
                return Json("Rota grubu atanmış güvenliği silemezsiniz!");
            }

            try
            {
                db.EMPLOYEE.Remove(emp);
                db.SaveChanges();
            }

            catch(Exception x)
            {
                return Json(x);
            }

            return Json(true);
        }

        [HttpGet]
        public ActionResult GuvenlikDuzenle(int id)
        {
            var Employee = db.EMPLOYEE.Where(m => m.EMPLOYEEID == id).FirstOrDefault();
            return View(Employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuvenlikDuzenle(int? id, Employee employee)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var emp = db.EMPLOYEE.Find(id);

                if (emp != null)
                {
                    db.EMPLOYEE.Where(m => m.EMPLOYEEID == id).FirstOrDefault().EMPLOYEENAME = employee.EmployeeName;
                    db.EMPLOYEE.Where(m => m.EMPLOYEEID == id).FirstOrDefault().EMPLOYEELASTNAME = employee.EmployeeLastname;
                    db.EMPLOYEE.Where(m => m.EMPLOYEEID == id).FirstOrDefault().EMPLOYEEEMAIL = employee.EmployeeEmail;
                    db.EMPLOYEE.Where(m => m.EMPLOYEEID == id).FirstOrDefault().EMPLOYEEPASSWORD = employee.EmployeePassword;
                    db.EMPLOYEE.Where(m => m.EMPLOYEEID == id).FirstOrDefault().EMPLOYEEEDITDATE = DateTime.Now;
                    db.SaveChanges();
                    return RedirectToAction("Guvenlik", "Yonetici");
                }
            }

            catch(Exception x)
            {

            }

            return View(employee);
        }

        // END: Functions For Employees (Security Guards)

        
        // BEGIN: Functions For Routes

        [HttpGet]
        public ActionResult Rotalar()
        {
            var data = db.ROUTE.ToList();
            ViewBag.routeDetails = data;
            return View();
        }

        public ActionResult RotaEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RotaEkle(Route route)
        {
            string rt_code = "";

            byte[] arr;
            try
            {
                route.RouteID = db.ROUTE.Max(r => r.ROUTEID);
            }
            catch(Exception ex)
            {
                route.RouteID = 0;
            }

            try
            {
                if (route.RouteID + 1 >= 0 && route.RouteID + 1 < 10)
                {
                    rt_code = "Str" + " - " + "00" + (route.RouteID + 1).ToString();
                }

                if (route.RouteID + 1 >= 10 && route.RouteID + 1 < 100)
                {
                    rt_code = "Str" + " - " + "0" + (route.RouteID + 1).ToString();
                }

                if (route.RouteID + 1 >= 100)
                {
                    rt_code = "Str" + " - " + (route.RouteID + 1).ToString();
                }

                route.RouteCode = rt_code;

                string fileName = (route.RouteID + 1).ToString() + ".png";
                string filePath = "~/QR_Codes/" + fileName;
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCode = qrGenerator.CreateQrCode((route.RouteID + 1) + "\n" + rt_code, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap bitMap = new QRCode(qrCode).GetGraphic(20))
                {
                    MemoryStream ms = new MemoryStream();
                    bitMap.Save(Server.MapPath(filePath), ImageFormat.Png);

                    arr = ImageExtensions.ToByteArray(bitMap, ImageFormat.Bmp);
                }

                if (ModelState.IsValid)
                {
                    var rt = new ROUTE
                    {
                        ROUTEID = route.RouteID + 1,
                        ROUTENAME = route.RouteName,
                        ROUTEDESC = route.RouteDesc,
                        ROUTEEDITDATE = DateTime.Now,
                        ROUTEINSERTDATE = DateTime.Now,
                        ROUTEQR = arr,
                        ROUTECODE = rt_code
                    };
                    db.ROUTE.Add(rt);
                    db.SaveChanges();
                    return RedirectToAction("Rotalar", "Yonetici");
                }
            }
            catch(Exception x)
            {

            }

            return View(route);
        }

        [HttpGet]
        public ActionResult RotaDuzenle(int id)
        {
            var route = db.ROUTE.Where(r => r.ROUTEID == id).FirstOrDefault();
            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RotaDuzenle(int? id, Route route)
        {
            byte[] arr;

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var rt = db.ROUTE.Find(id);

                string rt_code = "";

                if (id >= 0 && id < 10)
                {
                    rt_code = "Str" + " - " + "00" + id.ToString();
                }

                if (id >= 10 && id < 100)
                {
                    rt_code = "Str" + " - " + "0" + id.ToString();
                }

                if (id >= 100)
                {
                    rt_code = "Str" + " - " + id.ToString();
                }

                if (rt != null)
                {
                    db.ROUTE.Where(m => m.ROUTEID == id).FirstOrDefault().ROUTEDESC = route.RouteDesc;
                    db.ROUTE.Where(m => m.ROUTEID == id).FirstOrDefault().ROUTENAME = route.RouteName;
                    db.ROUTE.Where(m => m.ROUTEID == id).FirstOrDefault().ROUTEEDITDATE = DateTime.Now;

                    string fileName = (route.RouteID).ToString() + ".png";
                    string filePath = "~/QR_Codes/" + fileName;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCode = qrGenerator.CreateQrCode((route.RouteID) + "\n" + rt_code, QRCodeGenerator.ECCLevel.Q);
                    using (Bitmap bitMap = new QRCode(qrCode).GetGraphic(20))
                    {
                        MemoryStream ms = new MemoryStream();
                        bitMap.Save(Server.MapPath(filePath), ImageFormat.Png);

                        arr = ImageExtensions.ToByteArray(bitMap, ImageFormat.Bmp);
                    }

                    db.ROUTE.Where(m => m.ROUTEID == id).FirstOrDefault().ROUTEQR = arr;
                    db.SaveChanges();
                    return RedirectToAction("Rotalar", "Yonetici");
                }
            }
            catch(Exception x)
            {

            }

            return View(route);
        }

        [HttpPost]
        public JsonResult RotaSil(int? id)
        {
            if (id == null)
            {
                return Json(false);
            }

            var rt = db.ROUTE.Find(id);

            if(rt == null)
            {
                return Json("Rota bulunamadı!");
            }

            var rtDev = db.PATROLITEM.FirstOrDefault(m => m.ROUTEID == id);

            if(rtDev != null)
            {
                return Json("Rota grubu içindeki rotayı silemezsiniz!");
            }

            try
            {
                db.ROUTE.Remove(rt);
                db.SaveChanges();
            }

            catch(Exception x)
            {
                return Json(x);
            }


            return Json(true);
        }

        // END: Functions For Routes


        // BEGIN: Function for Admin Profiles

        [HttpGet]
        public ActionResult Profilim()
        {
            string idstr = Session["AdminID"].ToString();
            var x = db.ADMIN.FirstOrDefault(m => m.ADMINID.ToString() == idstr);

            if(x == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("ProfilDuzenle", "Yonetici", new { id = x.ADMINID });
        }

        [HttpGet]
        public ActionResult ProfilDuzenle(int id)
        {
            var adm = db.ADMIN.Where(r => r.ADMINID == id).FirstOrDefault();
            return View(adm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProfilDuzenle(int? id, Admin admin)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var ad = db.ADMIN.Find(id);

                if (ad != null)
                {
                    if(IsValidEmailAddress(admin.AdminEmail) == true)
                    {
                        db.ADMIN.Where(m => m.ADMINID == id).FirstOrDefault().ADMINEMAIL = admin.AdminEmail;
                    }

                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    db.ADMIN.Where(m => m.ADMINID == id).FirstOrDefault().ADMINPASSWORD = admin.AdminPassword;
                    db.ADMIN.Where(m => m.ADMINID == id).FirstOrDefault().ADMINNAME = admin.AdminName;
                    db.ADMIN.Where(m => m.ADMINID == id).FirstOrDefault().ADMINLASTNAME = admin.AdminLastname;
                    db.ADMIN.Where(m => m.ADMINID == id).FirstOrDefault().ADMINEDITDATE = DateTime.Now;

                    db.SaveChanges();
                    return RedirectToAction("Cikis", "Yonetici");
                }
            }
            catch(Exception x)
            {

            }

            return View(admin);
        }

        // END: Functions for Admin Profiles


        // BEGIN: Functions for Patrol Routes

        public static int count;
        public ActionResult Devriyeler()
        {
            var data = db.PATROL.ToList();
            var rldata = db.PATROLITEM.ToList();
            Session["rota"] = new List<string>();
            Session["devItem"] = new List<PATROLITEM>();
            count = 1;
            ViewBag.patrolDetails = data;
            ViewBag.rlDetails = rldata;
            return View();
        }

        public static int pid;
        
        public ActionResult DevriyeEkle()
        {
            DevriyeFormModel modelEkle = new DevriyeFormModel();

            try
            {
                modelEkle.patrolID = db.PATROL.Max(r => r.PATROLID);
            }
            catch
            {
                modelEkle.patrolID = 0;
            }

            pid = modelEkle.patrolID;

            Session["DFM"] = modelEkle;

            return View(modelEkle);
        }

        [ChildActionOnly]
        public ActionResult DevriyeRotaEkle()
        {
            (Session["DFM"] as DevriyeFormModel).routes = db.ROUTE.ToList();

            foreach (var e in (Session["DFM"] as DevriyeFormModel).routes)
            {
                (Session["DFM"] as DevriyeFormModel).lst.Add(new SelectListItem
                {
                    Value = e.ROUTEID.ToString(),
                    Text = e.ROUTENAME
                });
            }

            var x = (Session["DFM"] as DevriyeFormModel);

            return PartialView("_DevriyeEkle", x);
        }

        [HttpPost]
        public ActionResult DevriyeEkle(DevriyeFormModel model)
        {
            try
            {
                var pat = new PATROL
                {
                    PATROLID = pid + 1,
                    PATROLDESC = model.Patrol.PATROLDESC,
                    PATROLNAME = model.Patrol.PATROLNAME,
                    PATROLNUM = "PTR" + " - " + (pid + 1).ToString(),
                    PATROLEDITDATE = DateTime.Now,
                    PATROLINSERTDATE = DateTime.Now
                };

                db.PATROL.Add(pat);

                foreach(var i in (Session["devItem"] as List<PATROLITEM>))
                {
                    db.PATROLITEM.Add(i);
                }

                db.SaveChanges();
            }
            catch(Exception ex)
            {

            }

            count = 1;

            return RedirectToAction("Devriyeler", "Yonetici");
        }

        [HttpPost]
        public JsonResult DevriyeRotasiEkle(int rID)
        {
            try
            {
                var npi = new PATROLITEM
                {
                    PATROLID = pid + 1,
                    ROUTEID = rID,
                    ROUTEROW = count
                };

                count++;

                (Session["devItem"] as List<PATROLITEM>).Add(npi);

                (Session["rota"] as List<string>).Add(rID.ToString());
            }
            catch(Exception x)
            {
                return Json(false);
            }

            return Json(true);
        }

        public static int temp;
        [HttpGet]
        public ActionResult DevriyeDuzenle(int id)
        {
            DevriyeFormModel model = new DevriyeFormModel();

            model.patrolID = id;

            pid = model.patrolID;

            var dev = db.PATROL.Where(r => r.PATROLID == id).FirstOrDefault();
            var pitem = db.PATROLITEM.Where(r => r.PATROLID == id).ToList();

            temp = pitem.Count;

            model.Patrol = dev;
            model.PatrolItems = pitem;
            Session["DevriyeFormModel"] = model;

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult DevriyeRotaDuzenle(int id)
        {
            DevriyeFormModel model = new DevriyeFormModel();
            model.routes = db.ROUTE.ToList();

            model.patrolID = id;

            var pitem = db.PATROLITEM.Where(r => r.PATROLID == id).OrderBy(m => m.ROUTEROW).ToList();

            foreach(var i in pitem)
            {
                model.routeStr.Add(i.ROUTEID.ToString());
            }

            model.PatrolItems = pitem;

            foreach (var e in model.routes)
            {
                model.lst.Add(new SelectListItem
                {
                    Value = e.ROUTEID.ToString(),
                    Text = e.ROUTENAME
                });
            }

            return PartialView("_DevriyeDuzenle", model);
        }

        [HttpPost]
        public ActionResult DevriyeDuzenle(int? id, DevriyeFormModel model)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var dv = db.PATROL.Find(model.Patrol.PATROLID);

                if (dv != null)
                {
                    db.PATROL.Where(r => r.PATROLID == id).FirstOrDefault().PATROLNAME = model.Patrol.PATROLNAME;
                    db.PATROL.Where(r => r.PATROLID == id).FirstOrDefault().PATROLDESC = model.Patrol.PATROLDESC;
                    db.PATROL.Where(r => r.PATROLID == id).FirstOrDefault().PATROLEDITDATE = DateTime.Now;

                    db.SaveChanges();
                    return RedirectToAction("Devriyeler", "Yonetici");
                }
            }
            catch(Exception x)
            {

            }

            return View(model);
        }

        public static int ct;
        [HttpPost]
        public JsonResult DevriyeRotasiDuzenle(int rID)
        {
            temp = (Session["DevriyeFormModel"] as DevriyeFormModel).PatrolItems.Count;
            var bigModel = Session["DevriyeFormModel"] as DevriyeFormModel;

            PATROLITEM x = new PATROLITEM();
            x.PATROLID = bigModel.patrolID;
            x.ROUTEID = rID;
            x.ROUTEROW = temp + 1;
            db.PATROLITEM.Add(x);
            (Session["DevriyeFormModel"] as DevriyeFormModel).routeStr.Add(rID.ToString());

            db.SaveChanges();

            var dev = db.PATROL.Where(r => r.PATROLID == bigModel.patrolID).FirstOrDefault();
            var pitem = db.PATROLITEM.Where(r => r.PATROLID == bigModel.patrolID).ToList();

            (Session["DevriyeFormModel"] as DevriyeFormModel).Patrol = dev;
            (Session["DevriyeFormModel"] as DevriyeFormModel).PatrolItems = pitem;
            return Json(true);
        }

        [HttpPost]
        public JsonResult DevriyeRotasiSil(int? tid)
        {
            DevriyeFormModel model = new DevriyeFormModel();

            if (tid == null)
            {
                return Json(false);
            }

            try
            {
                db.PATROLITEM.RemoveRange(db.PATROLITEM.Where(x => x.PATROLID == tid));
                db.SaveChanges();

                model.patrolID = tid ?? 1;
                var dev = db.PATROL.Where(r => r.PATROLID == tid).FirstOrDefault();
                var pitem = db.PATROLITEM.Where(r => r.PATROLID == tid).ToList();

                (Session["DevriyeFormModel"] as DevriyeFormModel).Patrol = dev;
                (Session["DevriyeFormModel"] as DevriyeFormModel).PatrolItems = pitem;
                (Session["DevriyeFormModel"] as DevriyeFormModel).routeStr = null;

            }
            catch(Exception x)
            {
                return Json(false);
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult DevriyeSil(int? id)
        {
            if (id == null)
            {
                return Json(false);
            }

            var dev = db.PATROL.FirstOrDefault(m => m.PATROLID == id);

            if(dev == null)
            {
                return Json("Rota Grubu bulunamadı!");
            }

            var empDev = db.EMPLOYEEPATROL.FirstOrDefault(m => m.PATROLID == id);

            if(empDev != null)
            {
                return Json("Bir güvenliğe atanmış rota grubunu silemezsiniz!");
            }

            try
            {
                db.PATROL.Remove(dev);
                var x = db.PATROLITEM.Where(a => a.PATROLID == id).ToList();

                foreach(var pi in x)
                {
                    db.PATROLITEM.Remove(pi);
                }
                db.SaveChanges();
            }
            catch(Exception x)
            {
                return Json(x);
            }
            return Json(true);
        }

        // END: Functions for Patrol Routes

        // BEGIN: Functions for Assigning Patrols to Securities (Employees)

        [HttpGet]
        public ActionResult GuvenlikDevriyeleri()
        {
            EmpPatrol empPatrol = new EmpPatrol();

            foreach(var e in db.EMPLOYEEPATROL)
            {
                var employees = db.EMPLOYEE.Where(m => m.EMPLOYEEID == e.EMPLOYEEID).ToList();
                var patrol = db.PATROL.Where(m => m.PATROLID == e.PATROLID).ToList();
                var patItems = db.PATROLITEM.Where(m => m.PATROLID == e.PATROLID).ToList();

                foreach(var x in employees)
                {
                    empPatrol.employees.Add(x);
                }

                foreach (var x in patrol)
                {
                    empPatrol.patrols.Add(x);
                }

                foreach (var x in patItems)
                {
                    empPatrol.patrolitems.Add(x);
                }

                empPatrol.empPatrol.Add(e);
            }

            return View(empPatrol);
        }

        [HttpGet]
        public ActionResult AtamaYap()
        {

            EmpPatrol empPatrol = new EmpPatrol();

            var a = db.EMPLOYEE.ToList();
            var b = db.PATROL.ToList();
            var c = db.PATROLITEM.ToList();

            foreach(var x in b)
            {
                empPatrol.lst.Add(new SelectListItem
                {
                    Value = x.PATROLID.ToString(),
                    Text = x.PATROLNAME
                });
            }

            foreach(var y in a)
            {
                empPatrol.lst2.Add(new SelectListItem
                {
                    Value = y.EMPLOYEEID.ToString(),
                    Text = y.EMPLOYEENAME + " " + y.EMPLOYEELASTNAME
                });
            }

            List<string> times = new List<string>();
            times.Add("Dakika");
            times.Add("Saat");
            times.Add("Gün");

            foreach(var time in times)
            {
                empPatrol.lstTime.Add(new SelectListItem
                {
                    Value = time,
                    Text = time
                });
            }

            times.Clear();

            empPatrol.employees = a;
            empPatrol.patrols = b;
            empPatrol.patrolitems = c;

            return View(empPatrol);
        }

        [HttpPost]
        public ActionResult AtamaYap(EmpPatrol empPatrol)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (empPatrol.timeType == "Saat")
                    {
                        empPatrol.resumptionTime = empPatrol.resumptionTime * 60;
                    }

                    if (empPatrol.timeType == "Gün")
                    {
                        empPatrol.resumptionTime = empPatrol.resumptionTime * 24 * 60;
                    }

                    var atayici = new EMPLOYEEPATROL
                    {
                        PATROLID = empPatrol.patrolID,
                        EMPLOYEEID = empPatrol.empID,
                        RESUMPTIONTIME = empPatrol.resumptionTime
                    };

                    db.EMPLOYEEPATROL.Add(atayici);
                    db.SaveChanges();

                    empPatrol.empPatrol.Add(atayici);
                    return RedirectToAction("GuvenlikDevriyeleri", "Yonetici");
                }
            }
            catch(Exception x)
            {

            }

            return View(empPatrol);
        }

        [HttpGet]
        public ActionResult AtamaDuzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EmpPatrol empPatrol = new EmpPatrol();

            var a = db.EMPLOYEE.ToList();
            var b = db.PATROL.ToList();
            var c = db.PATROLITEM.ToList();

            foreach (var x in b)
            {
                empPatrol.lst.Add(new SelectListItem
                {
                    Value = x.PATROLID.ToString(),
                    Text = x.PATROLNAME
                });
            }

            foreach (var y in a)
            {
                empPatrol.lst2.Add(new SelectListItem
                {
                    Value = y.EMPLOYEEID.ToString(),
                    Text = y.EMPLOYEENAME + " " + y.EMPLOYEELASTNAME
                });
            }

            List<string> times = new List<string>();
            times.Add("Dakika");
            times.Add("Saat");
            times.Add("Gün");

            foreach (var time in times)
            {
                empPatrol.lstTime.Add(new SelectListItem
                {
                    Value = time,
                    Text = time
                });
            }

            times.Clear();

            empPatrol.employees = a;
            empPatrol.patrols = b;
            empPatrol.patrolitems = c;
            empPatrol.empID = id ?? 0;

            return View(empPatrol);
        }

        [HttpPost]
        public ActionResult AtamaDuzenle(int? id ,EmpPatrol empPatrol)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (empPatrol.timeType == "Saat")
                    {
                        empPatrol.resumptionTime = empPatrol.resumptionTime * 60;
                    }

                    if (empPatrol.timeType == "Gün")
                    {
                        empPatrol.resumptionTime = empPatrol.resumptionTime * 24 * 60;
                    }

                    db.EMPLOYEEPATROL.Where(m => m.EMPLOYEEID == empPatrol.empID).FirstOrDefault().PATROLID = empPatrol.patrolID;
                    db.EMPLOYEEPATROL.Where(m => m.EMPLOYEEID == empPatrol.empID).FirstOrDefault().RESUMPTIONTIME = empPatrol.resumptionTime;
                    db.SaveChanges();
                    return RedirectToAction("GuvenlikDevriyeleri", "Yonetici");
                }
            }
            catch(Exception x)
            {

            }
            return View(empPatrol);
        }

        [HttpPost]
        public JsonResult AtamayiSil(int? id)
        {
            if (id == null)
            {
                return Json(false);
            }

            try
            {
                var emppat = db.EMPLOYEEPATROL.Find(id);
                db.EMPLOYEEPATROL.Remove(emppat);
                db.SaveChanges();
            }
            catch(Exception x)
            {
                return Json(false);
            }
            
            return Json(true);
        }

        // END: Functions for Assigning Patrols to Securities (Employees)
    }
}