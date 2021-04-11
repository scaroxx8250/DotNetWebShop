using ASPDotNetShoppingCart.Data;
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
        private readonly ILogger<HomeController> _logger;

        private readonly AppData appData;

        //List<User> users = new List<User>();

        public HomeController(ILogger<HomeController> logger, AppData appData)
        {
            _logger = logger;
            this.appData = appData;
            appData = new AppData();
        }

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
            

            //users.Add(new User { Username = "john", Password = "john" });
            User user = appData.Users.Find(x => x.Username == username && x.Password == password);

            if (user == null)
            {
                ViewData["errMsg"] = "No such user or incorrect password.";
                return View();
            }
            else
            {
                user.SessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("sessionId", user.SessionId);
                return RedirectToAction("Products");
            }
        }

        public IActionResult Logout()
        {
            string sessionId = Request.Cookies["sessionId"];
            User user = appData.Users.Find(x => x.SessionId == sessionId);
            if (user != null)
            {
                user.SessionId = null;
            }

            Response.Cookies.Delete("sessionId");

            return View("Login");
        }

        public IActionResult Products(string searchString)
        {
            ViewData["products"] = appData.Products;
            ViewData["CurrentFilter"] = searchString;

            var prod = from product in appData.Products select product;
            if (!String.IsNullOrEmpty(searchString))
            {
                //create new list for filtered products 
                List<Product> filterPrd = new List<Product>();

                //for each product in appData.Products
                foreach (var p in prod)
                {
                    //if description or product name contains searched string
                    if(p.description.ToLower().Contains (searchString.ToLower()) || p.productName.ToLower().Contains(searchString.ToLower()))
                    {
                        //add product to list of filtered products 
                        filterPrd.Add(p);
                    }
                }

                ViewData["products"] = filterPrd;


            }


            string sessionId = Request.Cookies["sessionId"];

            if (sessionId != null)
            {
                User user = appData.Users.Find(x => x.SessionId == sessionId);

                // If user == null, this means that there is no such user with this valid sessionId
                // This sessionId was bogus, send to Logout page (which will clear the sessionId so that it cannot be reused)
                if (user == null)

                    return RedirectToAction("Logout", "Home");

                // Store sessionId in the ViewData dictionary with a key called "sessionId"
                ViewData["sessionId"] = sessionId;
                ViewData["username"] = user.Username;
                ViewData["cart"] = user.Usercart;
            }
            else
            {
                Guest guest = new Guest()
                {
                    GsessionId = Guid.NewGuid().ToString()
                };

                 appData.Guests.Add(guest);

                Response.Cookies.Append("GsessionId", guest.GsessionId);

                ViewData["GSessionId"] = guest.GsessionId;
            }

            return View();
        }
        public IActionResult Cart()
        {
            //mock
            string[] imgs = { "/img/NET_Analytics.png",
            "/img/NET_Charts.png",
            "/img/NET_Machine_Learning.png"};

            string[] product = { "NET_Analytics",
            "NET_Charts",
            "NET_Machine_Learning"};

            string[] description = { "Performs data mining and analytics easily in .NET.",
            "Brings powerful charting capabilities to your .NET applications.",
            "Supercharged .NET machine learning libraries."};

            string[] price = { "399", "99", "299" };

            ViewData["images"] = imgs;
            ViewData["Names"] = product;
            ViewData["Description"] = description;
            ViewData["Price"] = price;


            string sessionId = Request.Cookies["sessionId"];

            if (sessionId != null)
            {
                User user = appData.Users.Find(x => x.SessionId == sessionId);

                // If user == null, this means that there is no such user with this valid sessionId
                // This sessionId was bogus, send to Logout page (which will clear the sessionId so that it cannot be reused)
                if (user == null)

                    return RedirectToAction("Logout", "Home");

                // Store sessionId in the ViewData dictionary with a key called "sessionId"
                ViewData["sessionId"] = sessionId;
                ViewData["username"] = user.Username;
                ViewData["cart"] = user.Usercart;
            }
            else
            {
                sessionId = Request.Cookies["GSessionId"];

                Guest guest = appData.Guests.Find(x => x.GsessionId == sessionId);

                ViewData["GSessionId"] = guest.GsessionId;

                ViewData["Guestcart"] = guest.Usercart;
            }

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

            string[] description = { "Performs data mining and analytics easily in .NET.",
            "Brings powerful charting capabilities to your .NET applications.",
            "Supercharged .NET machine learning libraries."};

            string[] Quantity = { "3", "3", "3" };

            string[] ActivationCode = { "1", "2", "3" };



            ViewData["images"] = imgs;
            ViewData["Names"] = product;
            ViewData["Description"] = description;
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
                User user = appData.Users.Find(x => x.SessionId == sessionId);

                // If user == null, this means that there is no such user with this valid sessionId
                // This sessionId was bogus, send to Logout page (which will clear the sessionId so that it cannot be reused)
                if (user == null)
                {
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    // Store sessionId in the ViewData dictionary with a key called "sessionId"
                    ViewData["sessionId"] = sessionId;
                    ViewData["username"] = user.Username;

                    //ViewData["cart"] = user.Cart;
                }

            }

            return View();
        }


        public IActionResult AddToCart([FromBody] Product product)
        {
            //initialize selectedProducts object
            SelectedProducts sp = new SelectedProducts();

            //get the sessionid
            string sessionId = Request.Cookies["sessionId"];

            if(sessionId != null)
            {
                //get the user object
                User user = appData.Users.Find(x => x.SessionId == sessionId);
                if (user == null)
                    return Json(new { success = false });   // error; no session

                else
                {
                    //pass the request product to sp object 
                    sp.Products = product;
                    sp.Qty = 1;

                    //set countItems to be Qty that user has clicked on the button.
                    int countItems = sp.Qty;

                    //if the cart is empty, add the product and countItem.
                    if (user.Usercart.Products.Count == 0)
                    {
                        user.Usercart.Products.Add(sp);
                    }
                    else
                    {
                        //get the total items of the cart
                        foreach (var item in user.Usercart.Products)
                        {
                            countItems += item.Qty;
                        }

                        bool match = false;
                        //loop thru the products
                        foreach (var item in user.Usercart.Products)
                        {
                            //add quantity if the product matches
                            if (item.Products.ProductId == sp.Products.ProductId)
                            {
                                item.Qty++;
                                match = true;
                                break;
                            }
                        }
                        //add new products if not match
                        if (match == false)
                        {
                            user.Usercart.Products.Add(sp);
                        }

                    }
                    return Json(new { success = true, quantity = countItems });


                }
            }
            else
            {
                //store GsessionId into sessionId variable;
                sessionId = Request.Cookies["GsessionId"];

                //get the guest object
                Guest guest = appData.Guests.Find(x => x.GsessionId == sessionId);

                //pass the request product to sp object 
                sp.Products = product;
                sp.Qty = 1;

                //set countItems to be Qty that user has clicked on the button.
                int countItems = sp.Qty;

                //if the cart is empty, add the product and countItem.
                if (guest.Usercart.Products.Count == 0)
                {
                    guest.Usercart.Products.Add(sp);
                }
                else
                {
                    //get the total items of the cart
                    foreach (var item in guest.Usercart.Products)
                    {
                        countItems += item.Qty;
                    }

                    bool match = false;
                    //loop thru the products
                    foreach (var item in guest.Usercart.Products)
                    {
                        //add quantity if the product matches
                        if (item.Products.ProductId == sp.Products.ProductId)
                        {
                            item.Qty++;
                            match = true;
                            break;
                        }
                    }
                    //add new products if not match
                    if (match == false)
                    {
                        guest.Usercart.Products.Add(sp);
                    }

                }
                return Json(new { success = true, quantity = countItems });
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
