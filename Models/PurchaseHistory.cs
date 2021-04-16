using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
