using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SessionId { get; set; }

        public Cart Usercart { get; set; }

        public User()
        {
            Usercart = new Cart();

        }
    }
}
