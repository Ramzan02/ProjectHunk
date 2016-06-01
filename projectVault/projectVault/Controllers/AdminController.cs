using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projectVault.Models;
namespace projectVault.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        Database1Entities db = new Database1Entities();
        public ActionResult main()
        {
            return View();
        }
        public ActionResult addnewuser()
        {
            return View();
        }
        public ActionResult viewuser()
        {
            return View(db.Users.ToList());
        }
        public ActionResult viewproject()
        {
            return View(db.Projects.ToList());
        }
        public ActionResult adduser()
        {
            string name = Request["name"];
            string email = Request["email"];
            string pass = Request["password"];
            string country = Request["country"];

            User us = new User();
            us.U_Name = name;
            us.U_Email = email;
            us.U_Password = pass;
            us.U_Country = country;
            db.Users.Add(us);
            db.SaveChanges();

            return RedirectToAction("main");
        }
        public ActionResult Edituser(int id)
        {
            return View(db.Users.First(s => s.U_Id == id));
        }
        public ActionResult updateuser(int id)
        {
            var a = db.Users.First(s => s.U_Id == id);
            a.U_Name = Request["name"];
            a.U_Email = Request["email"];
            a.U_Country = Request["country"];
            db.SaveChanges();
            return RedirectToAction("viewuser");
        }
        public ActionResult Deleteuser(int id)
        {
            var s = db.Users.First(x => x.U_Id == id);
            db.Users.Remove(s);
            db.SaveChanges();
            return RedirectToAction("viewuser");
        }
        public ActionResult Deleteproject(int id)
        {
            var s = from a in db.Images
                    where a.P_Id == id
                    select a;
            foreach (var b in s)
            {
                db.Images.Remove(b);
            }

            db.SaveChanges();
            var z = db.Projects.First(x => x.P_Id == id);
            db.Projects.Remove(z);
            db.SaveChanges();
            return RedirectToAction("viewproject");
        }

    }
}
