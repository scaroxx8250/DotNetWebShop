using System.ComponentModel.DataAnnotations;

namespace ASPDotNetShoppingCart.Models
{
    public class PurchasedItems
    {
        [Key]
        [Required]
        public string ActivationCode { get; set; }

        public int PurchasedHistoryId { get; set; }
        public virtual PurchasedHistory PurchasedHistory { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

    }
}
