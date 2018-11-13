using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NorthWindWeek5.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace NorthWindWeek5.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                // return a list of discounts
                DateTime now = DateTime.Now;
                return View(db.Discounts.Where(s => s.StartTime <= now && s.EndTime >
                now).ToList().Take(3));
            }
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction(actionName: "SignedOut", controllerName: "Home");
        }

        public ActionResult SignedOut()
        {
            return View();
        }

    }
}