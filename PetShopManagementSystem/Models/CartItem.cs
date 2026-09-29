namespace PetShopManagementSystem.Models
{
    public class CartItem
    {
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal
        {
            get
            {
                return Product.Price * Quantity;
            }
        }
    }
}
