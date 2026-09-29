namespace PetShopManagementSystem.Models
{
    public class LoginResult
    {
        public string Role { get; set; }

        public int UserID { get; set; }

        public string Name { get; set; }

        public bool IsSuccessful
        {
            get
            {
                return !string.IsNullOrEmpty(Role) && UserID > 0;
            }
        }
    }
}
