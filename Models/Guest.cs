using System.ComponentModel.DataAnnotations;

namespace ASPDotNetShoppingCart.Models
{
    public class Guest
    {
        [Key]
        [Required]
        public string GsessionId { get; set; }

        public virtual Cart Usercart { get; set; }
    }
}
