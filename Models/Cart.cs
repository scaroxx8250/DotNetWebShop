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
            Products = new List<SelectedProducts>();
        }
        public List<SelectedProducts> Products { get; set; }
    }
}
