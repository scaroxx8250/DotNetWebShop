using ASPDotNetShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ASPDotNetShoppingCart.Models;

namespace ASPDotNetShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        List<User> users = new List<User>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            users.Add(new User { Username = "john", Password = "john" });
            User user = users.Find(x => x.Username == username && x.Password == password);

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
            User user = users.Find(x => x.SessionId == sessionId);
            if (user != null)
            {
                user.SessionId = null;
            }

            Response.Cookies.Delete("sessionId");

            return View("Login");
        }

        public IActionResult Products()
        {
            List<Products> products = new List<Products>()
            {
                new Products()
                {
                    productName = ".NET Charts",
                    price = 299,
                    description = "Brings powerful charting capabilities to your .NET applications.",
                    imagePath = "/img/NET_Charts.png"
                },
                new Products()
                {
                    productName = ".NET Paypal",
                    price = 69,
                    description = "Integrate your .NET apps with Paypal the easy way!.",
                    imagePath = "/img/NET_PayPal.png"
                },
                new Products()
                {
                    productName = ".NET ML",
                    price = 299,
                    description = "Supercharged .NET machine learning libraries.",
                    imagePath = "/img/NET_Machine_Learning.png"
                },
                 new Products()
                {
                    productName = ".NET Analytics",
                    price = 299,
                    description = "Performs data mining and analytics easily in .NET.",
                    imagePath = "/img/NET_Analytics.png"
                },
                new Products()
                {
                    productName = ".NET Logger",
                    price = 169,
                    description = "Logs and aggregates events easily in your .NET apps.",
                    imagePath = "/img/NET_Logger.png"
                },
                new Products()
                {
                    productName = ".NET Numerics",
                    price = 299,
                    description = "Powerful numerical methods for your .NET simulations.",
                    imagePath = "/img/NET_Numerics.png"
                },
            };
            ViewData["products"] = products;
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Purchases()
        {
            //if (TempData["username"] is null)
            //{
            //    return View("Login");
            //}


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
    }
}
