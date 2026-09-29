namespace PetShopManagementSystem.Models
{
    public class ReportSummary
    {
        internal int TotalCustomers;

        public decimal TotalRevenue { get; set; }

        public int PaidOrderCount { get; set; }

        public int TotalOrders { get; set; }

        public int TotalManagers { get; set; }

        public int PendingOrders { get; set; }

        public int ConfirmedOrders { get; set; }

        public int DeniedOrders { get; set; }
    }
}
