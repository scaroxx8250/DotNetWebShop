using ASPDotNetShoppingCart.Db;
using ASPDotNetShoppingCart.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ASPDotNetShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbWebShop db;

        public HomeController(DbWebShop db)
        {
            this.db = db;
        }

        public IActionResult Login()
        {
            string sessionId = Request.Cookies["sessionId"];
            if(sessionId != null)
            {
               return RedirectToAction("Products");
            }
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
                // check for gsessionid
                // if gsessionid exists, it means that user has gone through products page as guest and tried to check out cart thus redirecting to here
                string GSessionId = Request.Cookies["GsessionId"];
                if (GSessionId != null)
                {
                    // instantiate guest.cart as cart object to assign to signed in user
                    Cart Gcart = db.Carts.FirstOrDefault(x => x.GuestId == GSessionId);
                    Cart Ucart = db.Carts.FirstOrDefault(x => x.UserId == user.Id);

                    // check for the case where user previously checked out a cart (thereby clearing it from DB) 
                    // and is now checking out as guest
                    if (Ucart != null)
                    {
                        db.Carts.Remove(Ucart);
                        db.SaveChanges();
                    }

                    // transfer cart from guest to user and change cookies to reflect change in user status
                    Gcart.UserId = user.Id;
                    Gcart.GuestId = null;
                    user.SessionId = Guid.NewGuid().ToString();
                    Guest guest = db.Guests.FirstOrDefault(x => x.GsessionId == GSessionId);
                    db.Guests.Remove(guest);
                    db.SaveChanges();
                    Response.Cookies.Append("sessionId", user.SessionId);
                    Response.Cookies.Delete("GsessionId");
                    return RedirectToAction("Cart");
                }
                else
                {
                    // user just logged in directly with no guest cart
                    user.SessionId = Guid.NewGuid().ToString();
                    db.SaveChanges();
                    Response.Cookies.Append("sessionId", user.SessionId);
                    db.SaveChanges();
                    return RedirectToAction("Products");
                }
            }
        }

        public IActionResult Logout()
        {
            // clear sessionId
            string sessionId = Request.Cookies["sessionId"];
            User user = db.Users.FirstOrDefault(x => x.SessionId == sessionId);
            if (user != null)
            {
                user.SessionId = null;

                // apply changes to DB
                db.SaveChanges();
            }

            Response.Cookies.Delete("sessionId");

            return View("Login");
        }

        // suppress browser caching so that items in cart are reflected correctly
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Products(string searchString)
        {
            // get all products from database
            List<Product> products = db.Products.ToList();

            ViewData["products"] = products;
            ViewData["CurrentFilter"] = searchString;

            // search functionality
            if (!String.IsNullOrEmpty(searchString))
            {
                // create new list for filtered products 
                List<Product> filterPrd = new List<Product>();

                foreach (var p in products)
                {
                    // if Description or product name contains searched string
                    if (p.Description.ToLower().Contains(searchString.ToLower()) || p.ProductName.ToLower().Contains(searchString.ToLower()))
                    {
                        // add product to list of filtered products 
                        filterPrd.Add(p);
                    }
                }

                ViewData["products"] = filterPrd;
            }

            // check for logged in user
            string sessionId = Request.Cookies["sessionId"];

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
                    // store sessionId in the ViewData dictionary with a key called "sessionId"
                    ViewData["sessionId"] = sessionId;
                    ViewData["username"] = user.Username;

                    Cart cart = db.Carts.FirstOrDefault(x => x.UserId == user.Id);

                    // if the cart is null, create cart for user
                    if (cart == null)
                    {
                        cart = new Cart()
                        {
                            UserId = user.Id
                        };
                        db.Carts.Add(cart);
                        db.SaveChanges();
                    }
                    ViewData["cart"] = cart;
                }
            }
            // for guest (no sessionId)
            else
            {
                string GsessionId = Request.Cookies["GsessionId"];

                // existing guest that comes to the product page again
                if (GsessionId != null)
                {
                    Guest guest = db.Guests.FirstOrDefault(x => x.GsessionId == GsessionId);
                }
                else
                {
                    // new guest
                    GsessionId = Guid.NewGuid().ToString();

                    Guest guest = new Guest()
                    {
                        GsessionId = GsessionId
                    };
                    db.Guests.Add(guest);
                    db.SaveChanges();
                    Response.Cookies.Append("GsessionId", guest.GsessionId);
                }

                // get the cart that is tagged to the guest
                Cart guestCart = db.Carts.FirstOrDefault(x => x.GuestId == GsessionId);

                // if the cart is null, create cart for guest
                if (guestCart == null)
                {
                    guestCart = new Cart()
                    {
                        GuestId = GsessionId
                    };

                    db.Carts.Add(guestCart);
                    db.SaveChanges();
                }
                ViewData["cart"] = guestCart;
                ViewData["GsessionId"] = GsessionId;
            }
            return View();
        }

        public IActionResult Cart()
        {
            // checking for logged in user
            Cart cart = new Cart();
            User users = db.Users.FirstOrDefault(x => x.SessionId == Request.Cookies["sessionId"]);

            // remember username
            if (users != null)
            {
                cart = db.Carts.FirstOrDefault(x => x.UserId == users.Id);
                ViewData["Username"] = users.Username;
            }

            // configuring cookies to reflect logged in user
            ViewData["sessionId"] = Request.Cookies["sessionId"];
            ViewData["GsessionId"] = null;

            // checking for guest user
            string GsessionId = Request.Cookies["GsessionId"];
            if (GsessionId != null)
            {
                Guest guests = db.Guests.FirstOrDefault(x => x.GsessionId == GsessionId);
                cart = db.Carts.FirstOrDefault(x => x.GuestId == guests.GsessionId);
                ViewData["GsessionId"] = GsessionId;
                ViewData["sessionId"] = null;
            }
            ViewData["Cart"] = cart;

            // generate token for purchases page to identify user coming from checkout
            string token = Guid.NewGuid().ToString();
            ViewData["token"] = token;
            TempData["token2"] = token;

            return View();
        }

        public IActionResult AddToCart([FromBody] Product product)
        {
            int countItems = 0;

            // get the sessionid
            string sessionId = Request.Cookies["sessionId"];

            if (sessionId != null)
            {
                // get the user object
                User user = db.Users.FirstOrDefault(x => x.SessionId == sessionId);
                if (user == null)
                {
                    return Json(new { success = false });   // error; no session
                }
                else
                {
                    // get the user's cart object and add cart item to database
                    Cart cart = db.Carts.FirstOrDefault(x => x.UserId == user.Id);

                    if (cart.CartItem.Count() == 0)
                    {
                        CartItem cartitem = new CartItem()
                        {
                            CartId = cart.CartId,
                            ProductId = product.Id,
                            Qty = 1
                        };
                        db.CartItems.Add(cartitem);
                        db.SaveChanges();
                        countItems++;
                    }
                    else
                    {
                        // get total quantity
                        foreach (var item in cart.CartItem)
                        {
                            countItems += item.Qty;
                        }

                        bool match = false;

                        // loop thru the products
                        foreach (var item in cart.CartItem)
                        {
                            // add quantity if the product matches
                            if (item.ProductId == product.Id)
                            {
                                countItems++;
                                item.Qty++;
                                db.SaveChanges();
                                match = true;
                                break;
                            }
                        }
                        // add new products if not match
                        if (match == false)
                        {
                            CartItem cartitem = new CartItem()
                            {
                                CartId = cart.CartId,
                                ProductId = product.Id,
                                Qty = 1
                            };
                            db.CartItems.Add(cartitem);
                            db.SaveChanges();
                            countItems++;
                        }
                    }
                    return Json(new { success = true, quantity = countItems });
                }
            }
            else
            {
                // checking if user is a guest
                string Gsessionid = Request.Cookies["GsessionId"];

                if (Gsessionid != null)
                {
                    //get guest object
                    Guest guest = db.Guests.FirstOrDefault(x => x.GsessionId == Gsessionid);

                    // get the guest's cart object
                    Cart Guestcart = db.Carts.FirstOrDefault(x => x.GuestId == guest.GsessionId);

                    if (Guestcart.CartItem.Count == 0)
                    {
                        CartItem cartitem = new CartItem()
                        {
                            CartId = Guestcart.CartId,
                            ProductId = product.Id,
                            Qty = 1
                        };
                        db.CartItems.Add(cartitem);
                        db.SaveChanges();
                        countItems++;
                    }
                    else
                    {
                        // get total quantity
                        foreach (var item in Guestcart.CartItem)
                        {
                            countItems += item.Qty;
                        }

                        bool match = false;
                        // loop thru the products
                        foreach (var item in Guestcart.CartItem)
                        {
                            // add quantity if the product matches
                            if (item.ProductId == product.Id)
                            {
                                countItems++;
                                item.Qty++;
                                db.SaveChanges();
                                match = true;
                                break;
                            }
                        }
                        // add new products if not match
                        if (match == false)
                        {
                            CartItem cartitem = new CartItem()
                            {
                                CartId = Guestcart.CartId,
                                ProductId = product.Id,
                                Qty = 1
                            };
                            db.CartItems.Add(cartitem);
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

        public IActionResult UpdateCart([FromBody] CartItem product)
        {
            // verifying the input cart item belong to user cart
            CartItem cartItem = db.CartItems.FirstOrDefault(x => x.CartId == product.CartId && x.ProductId == product.ProductId);

            if (cartItem == null)
            {
                return Json(new { success = false });
            }

            // update the product quantity in the cart item
            else
            {
                cartItem.Qty = product.Qty;
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        public IActionResult RemoveFromCart([FromBody] CartItem product)
        {
            // verifying the input cart item belong to user cart
            CartItem cartItem = db.CartItems.FirstOrDefault(x => x.CartId == product.CartId && x.ProductId == product.ProductId);

            if (cartItem == null)
                return Json(new { success = false });

            // remove product from the cart item
            else
            {
                db.CartItems.Remove(cartItem);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        public IActionResult Purchases(string token)
        {
            // for guest users who directly use url to access purchases, redirect to login page.
            if (Request.Cookies["sessionId"] == null)
            {
                return RedirectToAction("Login");
            }

            Cart cart = new Cart();

            // sessionid is not null, so we check to see if it is a valid sessionid tagged to existing user (i.e. bogus or not?)
            User users = db.Users.FirstOrDefault(x => x.SessionId == Request.Cookies["sessionId"]);

            if (users == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                cart = db.Carts.FirstOrDefault(x => x.UserId == users.Id);
                ViewData["Username"] = users.Username;
                ViewData["UserId"] = users.Id;
            }

            // Check if user is accessing action method from checkout. The query string token will only be there if this is the case
            // Otherwise the user is accessing purchases page only to see his recent purchases
            if (token == Convert.ToString(TempData["token2"]))
            {
                // if cart is not empty create new rows in Purchased tables
                if (cart.CartItem.Count() > 0)
                {
                    PurchasedHistory newHistory = new PurchasedHistory
                    {
                        DateTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        UserId = Convert.ToInt32(cart.UserId)
                    };
                    db.Add(newHistory);
                    db.SaveChanges();

                    // based on productid and qty, run generateActCode as required and save to PurchasedItems in DB
                    int HistoryId = db.PurchasedHistories.OrderBy(x => x.Id).LastOrDefault().Id;
                    foreach (CartItem x in cart.CartItem)
                    {
                        GenerateActCode(x.ProductId, x.Qty, HistoryId);
                    }

                    // clear user cart on checkout.
                    db.Carts.Remove(cart);
                    db.SaveChanges();
                }
            }

            // extract and save purchased history into viewdata for view retrieval
            List<PurchasedHistory> Histories = db.PurchasedHistories.Where(history => history.UserId == users.Id).ToList();
            ViewData["Histories"] = Histories;
            List<Product> products = db.Products.OrderBy(x => x.Id).ToList();
            ViewData["Products"] = products;
            return View();
        }

        public void GenerateActCode(int productId, int qty, int historyId)
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

       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("{*url}", Order = int.MaxValue)]
        public IActionResult Error()
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
