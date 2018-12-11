using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthWindWeek5.Models;
using NorthWindWeek5.Security;

namespace NorthWindWeek5.Controllers
{
    public class CartController : Controller
    {
        [HttpPost]
        public JsonResult AddToCart(CartDTO cartDTO, int? discount)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            if (discount == null)
            {
                Cart sc = new Cart();
                sc.ProductID = cartDTO.ProductID;
                sc.CustomerID = cartDTO.CustomerID;
                sc.Quantity = cartDTO.Quantity;
                using (NorthwindEntities db = new NorthwindEntities())
                {

                    // if there is a duplicate product id in cart, simply update the quantity
                    if (db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID ==
                   sc.CustomerID).Any())
                    {
                        // this product is already in the customer's cart,
                        // update the existing cart item's quantity
                        Cart cart = db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID ==
                       sc.CustomerID).FirstOrDefault();
                        cart.Quantity += sc.Quantity;
                        sc = new Cart()
                        {
                            CartID = cart.CartID,
                            ProductID = cart.ProductID,
                            CustomerID = cart.CustomerID,
                            Quantity = cart.Quantity
                        };
                    }
                    else
                    {
                        // this product is not in the customer's cart, add the product
                        db.Carts.Add(sc);
                    }
                    db.SaveChanges();
                    //System.Threading.Thread.Sleep(1500);

                }

                return Json(sc, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Cart sc = new Cart();
                sc.ProductID = cartDTO.ProductID;
                sc.CustomerID = cartDTO.CustomerID;
                sc.Quantity = cartDTO.Quantity;
                sc.Discount = discount;
                using (NorthwindEntities db = new NorthwindEntities())
                {

                    // if there is a duplicate product id in cart, simply update the quantity
                    if (db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID ==
                   sc.CustomerID).Any())
                    {
                        // this product is already in the customer's cart,
                        // update the existing cart item's quantity
                        Cart cart = db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID ==
                       sc.CustomerID).FirstOrDefault();
                        cart.Quantity += sc.Quantity;
                        sc = new Cart()
                        {
                            CartID = cart.CartID,
                            ProductID = cart.ProductID,
                            CustomerID = cart.CustomerID,
                            Quantity = cart.Quantity,
                            Discount = cart.Discount
                        };
                    }
                    else
                    {
                        // this product is not in the customer's cart, add the product
                        db.Carts.Add(sc);
                    }
                    db.SaveChanges();
                    //System.Threading.Thread.Sleep(1500);

                }

                return Json(sc, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult AddDiscount(List<Cart> carts, int code)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        Response.StatusCode = 400;
        //        return Json(new { }, JsonRequestBehavior.AllowGet);
        //    }
        //    Discount discount;
        //    using(NorthwindEntities db = new NorthwindEntities())
        //    {
        //        foreach (Cart c in carts)
        //        {
        //                var results = db.Discounts.Where(d => d.ProductID == c.ProductID).Select(d => d.Code).FirstOrDefault();

        //        }
        //    }
            
        //        return Json(discount, JsonRequestBehavior.AllowGet);
        //    }

        [Authorize]
        public ActionResult Cart()
        {
            return View();           
        }

        public ActionResult CheckOut()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                //Database calls go here
                var id = UserAccount.GetUserId();

                Order o = new Order();

                var carts = db.Carts.Where( c => c.CustomerID == id);

                var userInfo = db.Customers.Where(c => c.CustomerID == id).FirstOrDefault();

                o.CustomerID = userInfo.CustomerID;
                o.OrderDate = DateTime.Now;
                o.ShipAddress = userInfo.Address;
                o.ShipName = userInfo.CompanyName;
                o.ShipCity = userInfo.City;
                o.ShipRegion = userInfo.Region;
                o.ShipPostalCode = userInfo.PostalCode;
                o.ShipCountry = userInfo.Country;

                db.Orders.Add(o);

                db.SaveChanges();

                var orderID = o.OrderID;

                foreach (Cart c in carts)
                {
                    Order_Detail od = new Order_Detail();
                    od.OrderID = orderID;
                    od.ProductID = Convert.ToInt32(c.ProductID);
                    var price = db.Products
                        .Where(p => p.ProductID == c.ProductID)
                        .Select(p => p.UnitPrice)
                        .FirstOrDefault();
                    od.UnitPrice = Convert.ToDecimal(price);
                    od.Discount = 0;
                    od.Quantity = Convert.ToInt16(c.Quantity);

                    db.Order_Details.Add(od);

                    db.Carts.Remove(c);
                }

                db.SaveChanges();

            }
            return RedirectToAction(actionName: "Purchased", controllerName: "Cart");
        }

        public ActionResult Purchased()
        {
            return View();
        }

        public JsonResult Remove(int id)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            Cart sc;
            using (NorthwindEntities db = new NorthwindEntities())
            {
                sc = db.Carts.Where(c => c.CartID == id).FirstOrDefault();

                db.Carts.Remove(sc);

                db.SaveChanges();
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}