using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthWindWeek5.Models;
using NorthWindWeek5.Security;
using System.Web.Security;
using System.Net;

namespace NorthWindWeek5.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        [Authorize]
        public ActionResult Account()
        {
            if (Request.Cookies["role"].Value != "customer")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.CustomerId = UserAccount.GetUserId();
            using (NorthwindEntities db = new NorthwindEntities())
            {
                Customer customer = db.Customers.Find(UserAccount.GetUserId());

                CustomerEdit EditCustomer = new CustomerEdit()
                {
                    CompanyName = customer.CompanyName,
                    ContactName = customer.ContactName,
                    ContactTitle = customer.ContactTitle,
                    Address = customer.Address,
                    City = customer.City,
                    Region = customer.Region,
                    PostalCode = customer.PostalCode,
                    Country = customer.Country,
                    Phone = customer.Phone,
                    Fax = customer.Fax,
                    Email = customer.Email
                };
                return View(EditCustomer);
            }

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Account([Bind(Include = "CompanyName,ContactName," +
            "ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax,Email")]
            CustomerEdit UpdatedCustomer)
        {
            if (Request.Cookies["role"].Value != "customer")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (NorthwindEntities db = new NorthwindEntities())
            {
                if (ModelState.IsValid)
                {
                    Customer customer = db.Customers.Find(UserAccount.GetUserId());

                    if (customer.CompanyName.ToLower() != UpdatedCustomer.CompanyName.ToLower())
                    {
                        if (db.Customers.Any(c => c.CompanyName == UpdatedCustomer.CompanyName))
                        {
                            ModelState.AddModelError("CompanyName", "Duplicate");
                            return View(UpdatedCustomer);
                        }
                        else
                        {
                            customer.CompanyName = UpdatedCustomer.CompanyName;
                        }
                    }

                    customer.Address = UpdatedCustomer.Address;
                    customer.City = UpdatedCustomer.City;
                    customer.ContactName = UpdatedCustomer.ContactName;
                    customer.ContactTitle = UpdatedCustomer.ContactTitle;
                    customer.Country = UpdatedCustomer.Country;
                    customer.Email = UpdatedCustomer.Email;
                    customer.Fax = UpdatedCustomer.Fax;
                    customer.Phone = UpdatedCustomer.Phone;
                    customer.PostalCode = UpdatedCustomer.PostalCode;
                    customer.Region = UpdatedCustomer.Region;

                    db.SaveChanges();
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                return View(UpdatedCustomer);
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "CompanyName,Password,ContactName," +
            "ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax,Email")]
            Customer customer)
        {
            customer.UserGuid = System.Guid.NewGuid();
            customer.Password = UserAccount.HashSHA1(customer.Password + customer.UserGuid);
            using (NorthwindEntities db = new NorthwindEntities())
            {
                if (db.Customers.Any(c => c.CompanyName == customer.CompanyName))
                {
                    return View();
                }
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }

        }

        public ActionResult SignIn()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                ViewBag.CustomerId = new SelectList(db.Customers
                    .OrderBy(c => c.CompanyName), "CustomerId", "CompanyName")
                    .ToList();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "CustomerId, Password")]
            CustomerSignIn customerSignIn, string ReturnUrl)
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                if (ModelState.IsValid)
                {


                    Customer customer = db.Customers.Find(customerSignIn.CustomerId);
                    string str = UserAccount.HashSHA1(customerSignIn.Password + customer.UserGuid);

                    if (str == customer.Password)
                    {
                        FormsAuthentication.SetAuthCookie(customer.CustomerID.ToString(), false);
                        HttpCookie myCookie = new HttpCookie("role");
                        myCookie.Value = "customer";
                        Response.Cookies.Add(myCookie);
                        if (ReturnUrl != null)
                        {
                            return Redirect(ReturnUrl);
                        }
                        return RedirectToAction(actionName: "Index", controllerName: "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Incorrect Password");
                    }
                }
                ViewBag.CustomerId = new SelectList(db.Customers
                    .OrderBy(c => c.CompanyName), "CustomerId", "CompanyName")
                    .ToList();
                return View();


            }
        }
    }
}