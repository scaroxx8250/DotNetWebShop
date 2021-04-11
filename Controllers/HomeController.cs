using ASPDotNetShoppingCart.Data;
using ASPDotNetShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                TempData["username"] = "Guest";
                ViewData["errMsg"] = "No such user or incorrect password.";
                return View();
            }
            else
            {
                TempData["username"] = username;
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

        public IActionResult Products()
        {
            //List<Product> products = new List<Product>()
            //{
            //    new Product()
            //    {
            //        productName = ".NET Charts",
            //        price = 299,
            //        description = "Brings powerful charting capabilities to your .NET applications.",
            //        imagePath = "/img/NET_Charts.png"
            //    },
            //    new Product()
            //    {
            //        productName = ".NET Paypal",
            //        price = 69,
            //        description = "Integrate your .NET apps with Paypal the easy way!.",
            //        imagePath = "/img/NET_PayPal.png"
            //    },
            //    new Product()
            //    {
            //        productName = ".NET ML",
            //        price = 299,
            //        description = "Supercharged .NET machine learning libraries.",
            //        imagePath = "/img/NET_Machine_Learning.png"
            //    },
            //     new Product()
            //    {
            //        productName = ".NET Analytics",
            //        price = 299,
            //        description = "Performs data mining and analytics easily in .NET.",
            //        imagePath = "/img/NET_Analytics.png"
            //    },
            //    new Product()
            //    {
            //        productName = ".NET Logger",
            //        price = 169,
            //        description = "Logs and aggregates events easily in your .NET apps.",
            //        imagePath = "/img/NET_Logger.png"
            //    },
            //    new Product()
            //    {
            //        productName = ".NET Numerics",
            //        price = 299,
            //        description = "Powerful numerical methods for your .NET simulations.",
            //        imagePath = "/img/NET_Numerics.png"
            //    },
            //};
            ViewData["products"] = appData.Products;

            string sessionId = Request.Cookies["sessionId"];

            if (sessionId != null)
            {
                User user = appData.Users.Find(x => x.SessionId == sessionId);

                // If user == null, this means that there is no such user with this valid sessionId
                // This sessionId was bogus, send to Logout page (which will clear the sessionId so that it cannot be reused)
                if (user == null)
                    return RedirectToAction("Index", "Logout");

                // Store sessionId in the ViewData dictionary with a key called "sessionId"
                ViewData["sessionId"] = sessionId;
                ViewData["username"] = user.Username;

                //ViewData["cart"] = user.Cart;
            }

            return View();
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

        public IActionResult Cart()
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

            string[] price = { "399", "99", "299" };

            ViewData["images"] = imgs;
            ViewData["Names"] = product;
            ViewData["Description"] = description;
            ViewData["Price"] = price;

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

            string[] Quantity = { "1", "2", "3" };

            string[] ActivationCode = { "1", "2", "3" };



            ViewData["images"] = imgs;
            ViewData["Names"] = product;
            ViewData["Description"] = description;
            ViewData["Quantity"] = Quantity;
            ViewData["AcCode"] = ActivationCode;


            return View();
        }
    }
}
