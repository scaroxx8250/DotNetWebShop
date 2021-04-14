using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
