using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Models
{
    public class Guest
    {
        public string GsessionId { get; set; }
        public Cart Usercart { get; set; }

        public Guest()
        {
            Usercart = new Cart();

        }
    }
}
