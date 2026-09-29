using System;
using System.Data;
using System.Data.SqlClient;
using PetShopManagementSystem.Database;

namespace PetShopManagementSystem.Repositories
{
    public class PaymentRepository
    {
        private readonly DBConnection db;

        public PaymentRepository()
        {
            db = new DBConnection();
        }

        public bool MarkPaymentPaid(int orderId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    UPDATE p
                    SET p.PaymentStatus = 'PAID',
                        p.PaymentDate = GETDATE()
                    FROM Payment p
                    INNER JOIN Orders o ON o.OrderID = p.OrderID
                    WHERE p.OrderID = @OrderID
                      AND p.PaymentStatus = 'UNPAID'
                      AND o.OrderStatus = 'CONFIRMED'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public decimal GetPaymentAmount(int orderId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT Amount
                    FROM Payment
                    WHERE OrderID = @OrderID";

                using (SqlCommand command =
                    new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@OrderID", orderId);

                    connection.Open();

                    object result =
                        command.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                    {
                        return 0;
                    }

                    return Convert.ToDecimal(result);
                }
            }
        }

        public string GetPaymentStatus(int orderId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT PaymentStatus
                    FROM Payment
                    WHERE OrderID = @OrderID";

                using (SqlCommand command =
                    new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@OrderID", orderId);

                    connection.Open();

                    object result =
                        command.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                    {
                        return "UNPAID";
                    }

                    return result.ToString();
                }
            }
        }
    }
}
