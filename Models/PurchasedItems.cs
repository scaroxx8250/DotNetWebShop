using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Models
{
    public class PurchasedItems
    {
        [Key]
        [Required]
        public string ActivationCode { get; set; }

        public virtual PurchaseHistory PurchaseHistory { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

    }
}
