using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthWindWeek5.Models;

namespace NorthWindWeek5.Controllers
{
    public class CartController : Controller
    {
        [HttpPost]
        public JsonResult AddToCart(CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
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
                db.SaveChanges();                //System.Threading.Thread.Sleep(1500);
            }
            return Json(sc, JsonRequestBehavior.AllowGet);
        }
    }
}