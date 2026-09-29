namespace PetShopManagementSystem.Models
{
    public abstract class Person
    {
        private string name;
        private string username;
        private string password;
        private string email;
        private string phone;
        private int status;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public abstract string GetRole();

        public virtual string GetDisplayName()
        {
            return Name;
        }
    }
}
