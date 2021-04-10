using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Models
{
    public class Cart
    {
      
        public Cart()
        {
            Products = new List<selectedProducts>();
        }
        public List<selectedProducts> Products { get; set; }
    }
    public class selectedProducts
    {
        public Product Products { get; set; }
        public int Qty { get; set; }
    }
}
