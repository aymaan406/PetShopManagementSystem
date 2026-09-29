namespace PetShopManagementSystem.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        public int ManagerID { get; set; }

        public int FoodTypeID { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string FoodTypeName { get; set; }
    }
}
