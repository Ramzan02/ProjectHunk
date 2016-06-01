using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projectVault.Models;
using System.Net.Mail;

namespace projectVault.Controllers
{
    public class HomeController : Controller
    {
        Database1Entities de = new Database1Entities();
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult home()
        {
            if (Session["uid"] != null)
                return View();
            return RedirectToAction("login");
        }
        public ActionResult myproject()
        {
            if (Session["uid"] != null)
                return View();
            return RedirectToAction("login");
        }
        public ActionResult about()
        {
            return View();
        }
        public ActionResult addpro()
        {
            if (Session["uid"] != null)
                return View();
            return RedirectToAction("login");
        }
        public ActionResult contact()
        {
            if (Session["uid"] != null)
                return View();
            return RedirectToAction("login");
        }
        public ActionResult login()
        {
            return View();
        }

        public ActionResult loginRedir()
        {
            string email = Request["email"];
            if (Request["email"] == "admin@proj.com" && Request["passwd"] == "apple")
                return Redirect("/Admin/main");
            try
            {
                var a = de.Users.First(s => s.U_Email == email);
            }
            catch 
            {
                return RedirectToAction("login");
            }
            var a2 = de.Users.First(s => s.U_Email == email);
            if (a2.U_Email == Request["email"] && a2.U_Password == Request["passwd"])
            {
                Session["uid"] = a2.U_Id;
                return RedirectToAction("home");
            }
            else
                return RedirectToAction("login");
        }

        public void sendMail(string name, string email)
        {
            try
            {
                string from = "ramzananjum347@gmail.com";
                string username = "ramzananjum347@gmail.com";
                string password = "apple.Ramzan";
                string to = email;
                MailAddressCollection TO_addressList = new MailAddressCollection();

                foreach (var curr_address in to.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    MailAddress mytoAddress = new MailAddress(curr_address, "Custom display name");
                    TO_addressList.Add(mytoAddress);
                }

                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                mail.Body = "Dear " + name + "!!\n" + "Congrats! You Are Now Registered!" + "\n" + "www.projecthunk.com";
                mail.From = new MailAddress(from);

                foreach (var id in TO_addressList)
                {
                    mail.To.Add(id);
                }

                mail.Subject = "verification";
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                smtpServer.EnableSsl = true;
                smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
            }

        }

        public ActionResult signupredir()
        {
            string name = Request["name"];
            string email = Request["email"];
            string pass = Request["password"];
            string cp = Request["cp"];
            string country = Request["country"];
            bool check = false;
            sendMail(name, email);
            foreach (var x in de.Users)
            {
                if (x.U_Email == email)
                    check = true;
            }
            if (pass == cp && check ==false)
            {
                User us = new User();
                us.U_Name = name;
                us.U_Email = email;
                us.U_Password = pass;
                us.U_Country = country;
                de.Users.Add(us);
                de.SaveChanges();
                var a2 = de.Users.First(s => s.U_Email == email);
                if (a2.U_Email == email)                
                    Session["uid"] = a2.U_Id;
                return RedirectToAction("home");              
                
            }
            else
                return RedirectToAction("signup");
        }
        public ActionResult project(int id)
        {
            ViewBag.var = id;
            if (Session["uid"] != null)
                return View();
            return RedirectToAction("login");           
        }
        public ActionResult project_desc(int id)
        {
            var a = de.Projects.First(s => s.P_Id == id);
            if (Session["uid"] != null)
                return View(a);
            return RedirectToAction("login");
        }
        public ActionResult signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult muproject()
        {
            if (Session["uid"] == null)                
                return RedirectToAction("login");
            Image p = new Image();
            Project proj = new Project();
            proj.P_Intro = Request["intro"];
            proj.P_Name = Request["name"];
            proj.P_Result = Request["conclu"];
            proj.P_Sol = Request["body"];
            proj.P_Type = Request["type"];
            int id = Convert.ToInt32(Session["uid"]);
            proj.U_Id = id;
            de.Projects.Add(proj);
            de.SaveChanges();

            var obj = from a in de.Projects
                      where a.P_Name == proj.P_Name
                      select a;
            int pid = 0;
            foreach (var f in obj)
            {
                pid = f.P_Id;
            }
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                string unique = DateTime.Now.Ticks.ToString();
                string simg = @"~\Files\" + unique + ".jpg";
                p.U_path = @"/Files/" + unique + ".jpg";
                p.P_Id = pid;
                de.Images.Add(p);
                de.SaveChanges();
                file.SaveAs(Server.MapPath(simg));
            }


            
            return RedirectToAction("myproject");
        }
        public ActionResult delpro(int id)
        {
            if (Session["uid"] == null)
                return RedirectToAction("login");
            var s = from a in de.Images
                    where a.P_Id == id
                    select a;
            foreach(var b in s)
            {
                de.Images.Remove(b);
            }
            
            de.SaveChanges();
            var c = de.Projects.First(e => e.P_Id == id);
            int id1 = c.U_Id;
            de.Projects.Remove(c);
            de.SaveChanges();
            return RedirectToAction("myproject/"+id1);
        }
        public ActionResult editinfo(int id)
        {
            if (Session["uid"] != null)
                return View(de.Users.First(s => s.U_Id == id));
            return RedirectToAction("login");
        }
        public ActionResult Editpro(int id)
        {
            if (Session["uid"] != null)
                return View(de.Projects.First(s => s.P_Id == id));
            return RedirectToAction("login");
        }
        public ActionResult updateuser(int id)
        {
            var a = de.Users.First(s => s.U_Id == id);
            a.U_Name = Request["name"];
            a.U_Email = Request["email"];
            a.U_Country = Request["country"];
            de.SaveChanges();
            return RedirectToAction("home");
        }
        public ActionResult editproject(int id)
        {
            var a = de.Projects.First(s => s.P_Id == id);
            a.P_Name = Request["name"];
            a.P_Intro = Request["intro"];
            a.P_Sol = Request["body"];
            a.P_Result = Request["conclu"];
            de.SaveChanges();
            return RedirectToAction("project_desc/"+a.P_Id);
        }
        public ActionResult logout()
        {
            Session["uid"] = null;
            return RedirectToAction("index");
        }

        public ActionResult _header()
        {
            return PartialView();
        }

        public ActionResult _regheader()
        {
            return PartialView();
        }

    }
}
