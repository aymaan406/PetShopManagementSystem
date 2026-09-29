using System;

namespace PetShopManagementSystem.Exceptions
{
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException() : base("There is not enough stock for this order.")
        {
        }

        public InsufficientStockException(string message) : base(message)
        {
        }
    }
}
