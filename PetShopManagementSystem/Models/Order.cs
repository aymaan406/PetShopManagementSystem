using System;

namespace PetShopManagementSystem.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public int ManagerID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }

        public string ManagerName { get; set; }

        public string PaymentStatus { get; set; }
    }
}
