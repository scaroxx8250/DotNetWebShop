using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Models
{
    public class PurchasedHistory
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public long DateTime { get; set; }

        public virtual ICollection<PurchasedItems> PurchasedItems { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
