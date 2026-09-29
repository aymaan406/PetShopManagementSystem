namespace PetShopManagementSystem.Models
{
    public class Manager : Person
    {
        public int ManagerID { get; set; }

        public override string GetRole()
        {
            return "Manager";
        }
    }
}
