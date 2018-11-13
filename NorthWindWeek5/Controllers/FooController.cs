using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NorthWindWeek5.Controllers
{
    public class FooController : Controller
    {
        // GET: Foo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Bar()
        {
            ViewBag.ChewingGum = "Double Bubble";
            return View();
        }
    }
}