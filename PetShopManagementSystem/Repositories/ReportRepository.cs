using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using PetShopManagementSystem.Database;
using PetShopManagementSystem.Models;

namespace PetShopManagementSystem.Repositories
{
    public class ReportRepository
    {
        private readonly DBConnection dbConnection;

        public ReportRepository()
        {
            dbConnection = new DBConnection();
        }

        public ReportSummary GetSummary()
        {
            ReportSummary summary =
                new ReportSummary();

            using (SqlConnection connection =
                   dbConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT
                        (SELECT ISNULL(SUM(TotalRevenue), 0)
                         FROM vw_TotalRevenue) AS TotalRevenue,

                        (SELECT COUNT(*)
                         FROM Orders) AS TotalOrders,

                        (SELECT COUNT(*)
                         FROM Customer) AS TotalCustomers,

                        (SELECT COUNT(*)
                         FROM Manager) AS TotalManagers,

                        (SELECT COUNT(*)
                         FROM Orders
                         WHERE OrderStatus = 'Pending') AS PendingOrders,

                        (SELECT COUNT(*)
                         FROM Orders
                         WHERE OrderStatus = 'Confirmed') AS ConfirmedOrders,

                        (SELECT COUNT(*)
                         FROM Orders
                         WHERE OrderStatus = 'Denied') AS DeniedOrders;";

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.TotalRevenue =
                                Convert.ToDecimal(
                                    reader["TotalRevenue"]);

                            summary.TotalOrders =
                                Convert.ToInt32(
                                    reader["TotalOrders"]);

                            summary.TotalCustomers =
                                Convert.ToInt32(
                                    reader["TotalCustomers"]);

                            summary.TotalManagers =
                                Convert.ToInt32(
                                    reader["TotalManagers"]);

                            summary.PendingOrders =
                                Convert.ToInt32(
                                    reader["PendingOrders"]);

                            summary.ConfirmedOrders =
                                Convert.ToInt32(
                                    reader["ConfirmedOrders"]);

                            summary.DeniedOrders =
                                Convert.ToInt32(
                                    reader["DeniedOrders"]);
                        }
                    }
                }
            }

            return summary;
        }

        public List<Order> GetOrderSummary()
        {
            List<Order> orders =
                new List<Order>();

            using (SqlConnection connection =
                   dbConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT
                        OrderID,
                        CustomerName,
                        ManagerName,
                        OrderDate,
                        TotalAmount,
                        OrderStatus
                    FROM vw_OrderSummary
                    ORDER BY OrderDate DESC;";

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order order =
                                new Order();

                            order.OrderID =
                                Convert.ToInt32(
                                    reader["OrderID"]);

                            order.CustomerName =
                                Convert.ToString(
                                    reader["CustomerName"]);

                            order.ManagerName =
                                Convert.ToString(
                                    reader["ManagerName"]);

                            order.OrderDate =
                                Convert.ToDateTime(
                                    reader["OrderDate"]);

                            order.TotalAmount =
                                Convert.ToDecimal(
                                    reader["TotalAmount"]);

                            order.OrderStatus =
                                Convert.ToString(
                                    reader["OrderStatus"]);

                            orders.Add(order);
                        }
                    }
                }
            }

            return orders;
        }
    }
}
