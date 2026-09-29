namespace PetShopManagementSystem.Models
{
    public class Customer : Person
    {
        public int CustomerID { get; set; }

        public string Address { get; set; }

        public override string GetRole()
        {
            return "Customer";
        }
    }
}
