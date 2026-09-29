using System;

namespace PetShopManagementSystem.Exceptions
{
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException() : base("Invalid username or password.")
        {
        }

        public InvalidLoginException(string message) : base(message)
        {
        }
    }
}
