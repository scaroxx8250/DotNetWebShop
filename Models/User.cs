using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(32)]
        public string Username { get; set; }
        [Required]
        [MaxLength(32)]
        public string Password { get; set; }
        
        [MaxLength(36)]
        public string SessionId { get; set; }

        public virtual ICollection<PurchasedHistory> PurchasedHistory { get; set; }

        public virtual Cart Usercart { get; set; }

    }
}
