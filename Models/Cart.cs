using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASPDotNetShoppingCart.Models
{
    public class Cart
    {
        [Required]
        public int CartId { get; set; }

        public string GuestId { get; set; }

        public virtual Guest Guest { get; set; }

        public int? UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<CartItem> CartItem { get; set; }
    }
}
