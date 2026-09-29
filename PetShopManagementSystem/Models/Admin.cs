namespace PetShopManagementSystem.Models
{
    public class Admin : Person
    {
        public int AdminID { get; set; }

        public override string GetRole()
        {
            return "Admin";
        }
    }
}
