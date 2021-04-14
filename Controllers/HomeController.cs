﻿using ASPDotNetShoppingCart.Data;
using ASPDotNetShoppingCart.Db;
using ASPDotNetShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbWebShop db;
        //private readonly ILogger<HomeController> _logger;


        public HomeController(DbWebShop db)
        {
            this.db = db;
        }

        //List<User> users = new List<User>();

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //    //this.appData = appData;
        //    //appData = new AppData();
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            User user = db.Users.FirstOrDefault(x => x.Username == username && x.Password == password);

            if (user == null)
            {
                ViewData["username"] = username;
                ViewData["errMsg"] = "No such user or incorrect password.";
                return View();
            }
            else
            {
                //store sessionID to database user table

                user.SessionId = Guid.NewGuid().ToString();
                db.SaveChanges();
                Response.Cookies.Append("sessionId", user.SessionId);
                return RedirectToAction("Products");
            }
        }

        public IActionResult Logout()
        {
            string sessionId = Request.Cookies["sessionId"];
            User user = db.Users.FirstOrDefault(x => x.SessionId == sessionId);
            if (user != null)
            {
                user.SessionId = null;
            }

            Response.Cookies.Delete("sessionId");

            return View("Login");
        }

        public IActionResult Products(string searchString)
        {
            List<Product> products = db.Products.ToList();

            ViewData["products"] = products;
            ViewData["CurrentFilter"] = searchString;


            if (!String.IsNullOrEmpty(searchString))
            {
                //create new list for filtered products 
                List<Product> filterPrd = new List<Product>();

                //for each product in appData.Products
                foreach (var p in products)
                {
                    //if Description or product name contains searched string
                    if (p.Description.ToLower().Contains(searchString.ToLower()) || p.ProductName.ToLower().Contains(searchString.ToLower()))
                    {
                        //add product to list of filtered products 
                        filterPrd.Add(p);
                    }
                }

                ViewData["products"] = filterPrd;
            }


            string sessionId = Request.Cookies["sessionId"];

            //for User
            if (sessionId != null)
            {
                User user = db.Users.FirstOrDefault(x => x.SessionId == sessionId);

                // This sessionId was bogus, send to Logout page (which will clear the sessionId so that it cannot be reused)
                if (user == null)
                {
                    return RedirectToAction("Logout", "Home");
                }

                else
                {
                    //Store sessionId in the ViewData dictionary with a key called "sessionId"
                    ViewData["sessionId"] = sessionId;
                    ViewData["username"] = user.Username;

                    //Get the cart that is tag to the user
                    Cart cart = db.Carts.FirstOrDefault(x => x.UserId == user.Id);

                    //if the cart is null, create cart for user
                    if (cart == null)
                    {
                        cart = new Cart();
                        cart.UserId = user.Id;
                        db.Add(cart);
                        db.SaveChanges();
                    }

                    ViewData["cart"] = cart;
                }
            }
            //for guest (no sessionId)
            else
            {
                string GsessionId = Request.Cookies["GsessionId"];

                //existing guest that come to the product page again
                if (GsessionId != null)
                {
                    Guest guest = db.Guests.FirstOrDefault(x => x.GsessionId == GsessionId);
                }
                else
                {
                    //new guest

                    GsessionId = Guid.NewGuid().ToString();

                    Guest guest = new Guest()
                    {
                        GsessionId = GsessionId
                    };
                    db.Add(guest);
                    db.SaveChanges();
                    Response.Cookies.Append("GsessionId", guest.GsessionId);
                }

                //Get the cart that is tag to the guest
                Cart guestCart = db.Carts.FirstOrDefault(x => x.GuestId == GsessionId);

                //if the cart is null, create cart for guest
                if (guestCart == null)
                {
                    guestCart = new Cart();
                    guestCart.GuestId = GsessionId;
                    db.Add(guestCart);
                    db.SaveChanges();
                }

                ViewData["cart"] = guestCart;
                ViewData["GsessionId"] = GsessionId;

            }
            return View();
        }
        public IActionResult Cart()
        {

            //string sessionId = Request.Cookies["sessionId"];

            //if (sessionId != null)
            //{
            //    User user = appData.Users.Find(x => x.SessionId == sessionId);

            //    // If user == null, this means that there is no such user with this valid sessionId
            //    // This sessionId was bogus, send to Logout page (which will clear the sessionId so that it cannot be reused)
            //    if (user == null)

            //        return RedirectToAction("Logout", "Home");

            //    // Store sessionId in the ViewData dictionary with a key called "sessionId"
            //    ViewData["sessionId"] = sessionId;
            //    ViewData["username"] = user.Username;
            //   // ViewData["cart"] = user.Usercart;
            //}
            //else
            //{
            //    sessionId = Request.Cookies["GSessionId"];

            //    Guest guest = appData.Guests.Find(x => x.GsessionId == sessionId);

            //    ViewData["GSessionId"] = guest.GsessionId;

            //    //ViewData["Guestcart"] = guest.Usercart;
            //}

            return View();
        }
        public IActionResult Purchases()
        {
            string[] imgs = { "/img/NET_Analytics.png",
            "/img/NET_Charts.png",
            "/img/NET_Machine_Learning.png"};

            string[] product = { "NET_Analytics",
            "NET_Charts",
            "NET_Machine_Learning"};

            string[] Description = { "Performs data mining and analytics easily in .NET.",
            "Brings powerful charting capabilities to your .NET applications.",
            "Supercharged .NET machine learning libraries."};

            string[] Quantity = { "3", "3", "3" };

            string[] ActivationCode = { "1", "2", "3" };



            ViewData["images"] = imgs;
            ViewData["Names"] = product;
            ViewData["Description"] = Description;
            ViewData["Quantity"] = Quantity;
            ViewData["AcCode"] = ActivationCode;

            //ViewData["products"] = appData.Products;

            string sessionId = Request.Cookies["sessionId"];

            // No sessionId
            if (sessionId == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                // Search for matching sessionId
                //User user = appData.Users.Find(x => x.SessionId == sessionId);

                // If user == null, this means that there is no such user with this valid sessionId
                // This sessionId was bogus, send to Logout page (which will clear the sessionId so that it cannot be reused)
                //if (user == null)
                //{
                //    return RedirectToAction("Logout", "Home");
                //}
                //else
                //{
                //    // Store sessionId in the ViewData dictionary with a key called "sessionId"
                //    ViewData["sessionId"] = sessionId;
                //   // ViewData["username"] = user.Username;

                //    //ViewData["cart"] = user.Cart;
                //}

            }

            return View();
        }


        public IActionResult AddToCart([FromBody] Product product)
        {
            int countItems = 0;

            //get the sessionid
            string sessionId = Request.Cookies["sessionId"];

            if (sessionId != null)
            {
                //get the user object
                User user = db.Users.FirstOrDefault(x => x.SessionId == sessionId);
                if (user == null)
                    return Json(new { success = false });   // error; no session

                else
                {
                    // get the user's cart object
                    Cart cart = db.Carts.FirstOrDefault(x => x.UserId == user.Id);

                    if (cart.CartItem.Count() == 0)
                    {
                        CartItem cartitem = new CartItem()
                        {
                            CartId = cart.CartId,
                            ProductId = product.Id,
                            Qty = 1
                        };
                        db.Add(cartitem);
                        db.SaveChanges();
                        countItems++;
                    }
                    else
                    {
                        //get total quantity
                        foreach (var item in cart.CartItem)
                        {
                            countItems += item.Qty;
                        }

                        bool match = false;
                        //loop thru the products
                        foreach (var item in cart.CartItem)
                        {
                            //add quantity if the product matches
                            if (item.ProductId == product.Id)
                            {
                                countItems++;
                                item.Qty++;
                                db.SaveChanges();
                                match = true;
                                break;
                            }
                        }
                        //add new products if not match
                        if (match == false)
                        {
                            CartItem cartitem = new CartItem()
                            {
                                CartId = cart.CartId,
                                ProductId = product.Id,
                                Qty = 1
                            };
                            db.Add(cartitem);
                            db.SaveChanges();
                            countItems++;
                        }
                    }
                    return Json(new { success = true, quantity = countItems });
                }
            }
            else
            {
                //get guest SessionId

                string Gsessionid = Request.Cookies["GsessionId"];

                if(Gsessionid != null)
                {
                    //get guest object
                    Guest guest = db.Guests.FirstOrDefault(x => x.GsessionId == Gsessionid);

                    // get the guest's cart object
                    Cart Guestcart = db.Carts.FirstOrDefault(x => x.GuestId == guest.GsessionId);

                    if(Guestcart.CartItem.Count == 0)
                    {
                        CartItem cartitem = new CartItem()
                        {
                            CartId = Guestcart.CartId,
                            ProductId = product.Id,
                            Qty = 1
                        };
                        db.Add(cartitem);
                        db.SaveChanges();
                        countItems++;
                    }
                    else
                    {
                        //get total quantity
                        foreach (var item in Guestcart.CartItem)
                        {
                            countItems += item.Qty;
                        }

                        bool match = false;
                        //loop thru the products
                        foreach (var item in Guestcart.CartItem)
                        {
                            //add quantity if the product matches
                            if (item.ProductId == product.Id)
                            {
                                countItems++;
                                item.Qty++;
                                db.SaveChanges();
                                match = true;
                                break;
                            }
                        }
                        //add new products if not match
                        if (match == false)
                        {
                            CartItem cartitem = new CartItem()
                            {
                                CartId = Guestcart.CartId,
                                ProductId = product.Id,
                                Qty = 1
                            };
                            db.Add(cartitem);
                            db.SaveChanges();
                            countItems++;
                        }
                    }
                    return Json(new { success = true, quantity = countItems });
                }
                else
                {
                    return Json(new { success = false });
                }
            }


           

        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
