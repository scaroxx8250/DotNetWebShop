namespace ASPDotNetShoppingCart.Models
{
    public class CartItem
    {
        public int Qty { get; set; }

        public int CartId { get; set; }

        public virtual Cart Cart { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

    }
}
