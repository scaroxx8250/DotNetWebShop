﻿using ASPDotNetShoppingCart.Data;
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

        public IActionResult Products(string searchString)
        {
            ViewData["products"] = appData.Products;
            ViewData["CurrentFilter"] = searchString;

            var prod = from product in appData.Products select product;
            if (!String.IsNullOrEmpty(searchString))
            {
                prod = prod.Where(prod => prod.description.Contains(searchString.ToLower()) || prod.productName.Contains(searchString.ToLower()));
                ViewData["products"] = prod.ToList();

            }

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    IEnumerable<Product> product = appData.Products.Where(s => s.description.Contains(searchString.ToLower()) || s.description.StartsWith(searchString.ToLower()) || s.productName.Contains(searchString.ToLower()));
            //    ViewData["products"] = product.ToList();

            //}

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
