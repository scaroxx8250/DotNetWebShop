using ASPDotNetShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Data
{
    public class AppData
    {
        public List<Guest> Guests { get; set; }
        public List<User> Users { get; set; }
        public List<Product> Products { get; set; }

        // On instantiation, create the following empty lists
        public AppData()
        {
            Guests = new List<Guest>();
            Users = new List<User>();
            Products = new List<Product>();
        }
    }
}
