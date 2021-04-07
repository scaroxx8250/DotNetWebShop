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
        public IActionResult Products()
        {
            List<Products> products = new List<Products>()
            {
                new Products()
                {
                    productName = ".NET Charts",
                    price = 99,
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
