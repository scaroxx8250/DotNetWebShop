using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string productName { get; set; }
        public double price { get; set; }
        public string description { get; set; }
        public string imagePath { get; set; }

        
    }
}
