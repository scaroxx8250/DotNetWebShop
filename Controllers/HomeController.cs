using ASPDotNetShoppingCart.Data;
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
                // check for gsessionid, instantiate guest.cart as cart object to assign to signed in user
                // db.guest.remove(cart)
                // db.users.add(cart)
                user.SessionId = Guid.NewGuid().ToString();
                db.SaveChanges();
                Response.Cookies.Append("sessionId", user.SessionId);
                db.SaveChanges();
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
            Cart cart = new Cart();
            
            User users = db.Users.FirstOrDefault(x => x.SessionId == Request.Cookies["sessionId"]);
            if (User != null)
            {
                cart = db.Carts.FirstOrDefault(x => x.UserId == users.Id);
                ViewData["Username"] = users.Username;
            }
            string GsessionId = Request.Cookies["GsessionId"];            
            if (GsessionId != null && users.SessionId == null) //Session ID provided, but user could not be found i.e. guest
            {
                Guest guests = db.Guests.FirstOrDefault(x => x.GsessionId == GsessionId);
                cart = db.Carts.FirstOrDefault(x => x.GuestId == guests.GsessionId);
            }

            ViewData["sessionId"] = Request.Cookies["sessionId"];
            ViewData["Cart"] = cart;


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

        public IActionResult Purchases()
        {
            // cart fed in as arg to get productid and qty
            Cart cart = new Cart();

            User users = db.Users.FirstOrDefault(x => x.SessionId == Request.Cookies["sessionId"]);
            if (User != null)
            {
                cart = db.Carts.FirstOrDefault(x => x.UserId == users.Id);
                ViewData["Username"] = users.Username;
                ViewData["UserId"] = users.Id;
            }

            // for guest users who directly use url to access purchases, redirect to login page.
            
            // create new row in purchasedhistory in DB
            DateTime localDT = DateTime.Now;
            PurchasedHistory newHistory = new PurchasedHistory
            {
                DateTime = localDT.Ticks,
                UserId = Convert.ToInt32(cart.UserId)
            };
            db.Add(newHistory);
            db.SaveChanges();

            // based on productid and qty, run generateActCode as required and save to PurchasedItems in DB
            int HistoryId = db.PurchasedHistories.OrderBy(x => x.Id).LastOrDefault().Id;
            foreach (CartItem x in cart.CartItem)
            {
                GenerateCode(x.ProductId, x.Qty, HistoryId);
            }

            // clear user cart on checkout.
            db.Carts.Remove(cart);
            db.SaveChanges();

            // extract and save purchased items into viewdata for view retrieval
            List<PurchasedHistory> Histories = db.PurchasedHistories.Where(history => history.UserId == users.Id).ToList<PurchasedHistory>();
            List<int> HistoryIds = new List<int>();
            foreach ( PurchasedHistory history in Histories)
            {
                HistoryIds.Add(history.Id);
            }
            List<PurchasedItems> AllItems = db.PurchasedItems.Where(item => HistoryIds.Contains(item.PurchasedHistoryId)).ToList<PurchasedItems>();
            ViewData["PurchasedItems"] = AllItems;

            List<Product> products = db.Products.ToList();
            ViewData["products"] = products;
            return View();
        }

        public void GenerateCode(int productId, int qty, int historyId)
        {
            for (int i = 0; i < qty; i++)
            {
                PurchasedItems newItem = new PurchasedItems
                {
                    ActivationCode = Guid.NewGuid().ToString(),
                    PurchasedHistoryId = historyId,
                    ProductId = productId
                };
                db.Add(newItem);
            }
            db.SaveChanges();
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
