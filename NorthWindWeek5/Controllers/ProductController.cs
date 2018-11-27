using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthWindWeek5.Models;
using System.Net;

namespace NorthWindWeek5.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product/Category
        public ActionResult Category()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                return View(db.Categories.OrderBy(c => c.CategoryName).ToList());
            }
        }

        // GET: Product/Discount
        public ActionResult Discount()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                // return a list of discounts
                DateTime now = DateTime.Now;
                return View(db.Discounts.Where(s => s.StartTime <= now && s.EndTime >
                now).ToList());
            }
        }

        List<Item> Items = new List<Item>()
        {
            new Item{ Id = "100", Name = "Cheese Puff", Price = .19},
            new Item{ Id = "101", Name = "Lamp", Price = 2.19},
            new Item{ Id = "102", Name = "Fishing Pole", Price = 3.87},
            new Item{ Id = "103", Name = "Bacon", Price = 1.43},
            new Item{ Id = "104", Name = "Firecracket", Price = 2.12}
        };
        public ActionResult Unit2Project()
        {
            ViewBag.Items = Items;
            return View();
        }

        [HttpPost]
        public ActionResult Contact(FormCollection Form)
        {
            List<Order> Orders = new List<Order>();
            foreach (Item p in Items)
            {
                Int16 qty;
                if (Int16.TryParse(Form["p_" + p.Id], out qty))
                {
                    if (qty > 0)
                    {
                       //Orders.Add(new Order { Prod = p, Qty = qty });
                    }
                }
            }
            ViewBag.Orders = Orders;

            return View();
        }

        [HttpPost]
        public ActionResult ProcessOrder(FormCollection Form)
        {
            List<Order> Orders = new List<Order>();
            foreach (Item p in Items)
            {
                Int16 qty;
                if (Int16.TryParse(Form["p_" + p.Id], out qty))
                {
                    if (qty > 0)
                    {
                       //Orders.Add(new Order { Prod = p, Qty = qty });
                    }
                }
            }
            ViewBag.Orders = Orders;
            //ViewBag.Name = Form["name"];
            //ViewBag.Address = Form["address"];
            //ViewBag.City = Form["city"];
            //ViewBag.State = Form["state"];
            //ViewBag.Zip = Form["zip"];

            return View();
        }

        [HttpPost]
        public ActionResult DisplayOrder(FormCollection Form)
        {
            ViewBag.Name = Form["name"];
            ViewBag.Address = Form["address"];
            ViewBag.Orders = Form["orders"];
            ViewBag.Total = Form["total"];
            ViewBag.City = Form["city"];
            ViewBag.State = Form["state"];
            ViewBag.Zip = Form["zip"];
            ViewBag.Products = Form["prods"].Split(',');
            ViewBag.Qty = Form["qty"].Split(',');
            ViewBag.Total = Form["total"];

            return View();

            //return Form["prods"];
        }

        public ActionResult Product(int? id)
        {
            ViewBag.id = id;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    using (NorthwindEntities db = new NorthwindEntities())
                    {
                        ViewBag.Filter = db.Categories.Find(id).CategoryName;
                        return View(db.Products.Where(p => p.CategoryID == id && !p.Discontinued)
                            .OrderBy(p => p.ProductName).ToList());
                    }
                }
            }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchResult(FormCollection Form)
        {
            string SearchString = Form["SearchString"];
            ViewBag.Filter = "Product";
            ViewBag.SearchString = SearchString;
            if(SearchString != null)
            {
                using (NorthwindEntities db = new NorthwindEntities())
                {
                    return View("Product", db.Products.Where(p => p.ProductName.Contains(SearchString) && p.Discontinued == false).OrderBy(p => p.ProductName).ToList());
                }
            }
            else
            {
                return View();
            }
        }

        public JsonResult FilterProducts(int? id, string SearchString, decimal? PriceFilter)
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                if (PriceFilter == null) {
                    Response.StatusCode = 400;
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                }
                var Products = db.Products.Where(p => p.Discontinued == false).ToList();
                if (id != null)
                {
                    Products = Products.Where(p => p.CategoryID == id).ToList();
                }
                if (!String.IsNullOrEmpty(SearchString))
                {
                    Products = Products.Where(p => p.ProductName.Contains(SearchString)).ToList();
                }
                var ProductDTOs = Products.Where(p => p.UnitPrice >= PriceFilter).Select(p =>
                new {
                    p.ProductID,
                    p.ProductName,
                    p.QuantityPerUnit,
                    p.UnitPrice,
                    p.UnitsInStock
                }).OrderBy(p => p.ProductName).ToList();
                return Json(ProductDTOs, JsonRequestBehavior.AllowGet);
            }
        }
    }
}