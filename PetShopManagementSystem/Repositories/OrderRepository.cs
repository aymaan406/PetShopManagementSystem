using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PetShopManagementSystem.Database;
using PetShopManagementSystem.Exceptions;
using PetShopManagementSystem.Models;

namespace PetShopManagementSystem.Repositories
{
    public class OrderRepository
    {
        private readonly DBConnection db;

        public OrderRepository()
        {
            db = new DBConnection();
        }

        public int PlaceOrder(
            int customerId,
            int managerId,
            List<CartItem> cartItems,
            out decimal totalAmount,
            out string orderStatus)
        {
            totalAmount = 0;
            orderStatus = "";

            if (cartItems == null || cartItems.Count == 0)
                return 0;

            using (SqlConnection connection = db.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        decimal calculatedTotal = 0;

                        foreach (CartItem item in cartItems)
                        {
                            string productQuery = @"
                                SELECT ManagerID, Price, StockQuantity
                                FROM Product
                                WHERE ProductID = @ProductID";

                            using (SqlCommand productCommand =
                                new SqlCommand(productQuery, connection, transaction))
                            {
                                productCommand.Parameters.AddWithValue(
                                    "@ProductID", item.Product.ProductID);

                                using (SqlDataReader reader = productCommand.ExecuteReader())
                                {
                                    if (!reader.Read())
                                        throw new Exception(
                                            "One of the selected products no longer exists.");

                                    int databaseManagerId = Convert.ToInt32(reader["ManagerID"]);
                                    decimal databasePrice = Convert.ToDecimal(reader["Price"]);
                                    int stockQuantity = Convert.ToInt32(reader["StockQuantity"]);

                                    if (databaseManagerId != managerId)
                                        throw new Exception(
                                            "The selected products belong to different managers.");

                                    if (item.Quantity <= 0 || item.Quantity > stockQuantity)
                                        throw new InsufficientStockException(
                                            "Not enough stock for " + item.Product.ProductName +
                                            ". Available: " + stockQuantity +
                                            ", requested: " + item.Quantity + ".");

                                    calculatedTotal += databasePrice * item.Quantity;
                                }
                            }
                        }

                        string insertOrderQuery = @"
                            INSERT INTO Orders
                            (CustomerID, ManagerID, OrderDate, TotalAmount, OrderStatus)
                            VALUES
                            (@CustomerID, @ManagerID, GETDATE(), @TotalAmount, 'PENDING');
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int orderId;

                        using (SqlCommand orderCommand =
                            new SqlCommand(insertOrderQuery, connection, transaction))
                        {
                            orderCommand.Parameters.AddWithValue("@CustomerID", customerId);
                            orderCommand.Parameters.AddWithValue("@ManagerID", managerId);
                            orderCommand.Parameters.AddWithValue("@TotalAmount", calculatedTotal);

                            orderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                        }

                        foreach (CartItem item in cartItems)
                        {
                            decimal unitPrice;

                            string priceQuery = @"
                                SELECT Price
                                FROM Product
                                WHERE ProductID = @ProductID";

                            using (SqlCommand priceCommand =
                                new SqlCommand(priceQuery, connection, transaction))
                            {
                                priceCommand.Parameters.AddWithValue(
                                    "@ProductID", item.Product.ProductID);

                                unitPrice = Convert.ToDecimal(priceCommand.ExecuteScalar());
                            }

                            decimal subtotal = unitPrice * item.Quantity;

                            string insertItemQuery = @"
                                INSERT INTO OrderItem
                                (OrderID, ProductID, Quantity, UnitPrice)
                                VALUES
                                (@OrderID, @ProductID, @Quantity, @UnitPrice);";

                            using (SqlCommand itemCommand =
                                new SqlCommand(insertItemQuery, connection, transaction))
                            {
                                itemCommand.Parameters.AddWithValue("@OrderID", orderId);
                                itemCommand.Parameters.AddWithValue("@ProductID", item.Product.ProductID);
                                itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                itemCommand.Parameters.AddWithValue("@UnitPrice", unitPrice);
                                itemCommand.ExecuteNonQuery();
                            }
                        }

                        string paymentQuery = @"
                            INSERT INTO Payment
                            (OrderID, Amount, PaymentMethod, PaymentStatus)
                            VALUES
                            (@OrderID, @Amount, 'COD', 'UNPAID');";

                        using (SqlCommand paymentCommand =
                            new SqlCommand(paymentQuery, connection, transaction))
                        {
                            paymentCommand.Parameters.AddWithValue("@OrderID", orderId);
                            paymentCommand.Parameters.AddWithValue("@Amount", calculatedTotal);
                            paymentCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        totalAmount = calculatedTotal;
                        orderStatus = "PENDING";
                        return orderId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool ConfirmOrder(int orderId, int managerId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                using (SqlCommand command =
                    new SqlCommand("sp_ConfirmOrder", connection))
                {
                    command.CommandType =
                        CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OrderID", orderId);
                    command.Parameters.AddWithValue("@ManagerID", managerId);

                    connection.Open();

                    command.ExecuteNonQuery();

                    return true;
                }
            }
        }

        public bool RejectOrder(int orderId, int managerId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                using (SqlCommand command =
                    new SqlCommand("sp_RejectOrder", connection))
                {
                    command.CommandType =
                        CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OrderID", orderId);
                    command.Parameters.AddWithValue("@ManagerID", managerId);

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT
                        OrderID,
                        CustomerID,
                        ManagerID,
                        OrderDate,
                        OrderStatus,
                        TotalAmount,
                        CustomerName,
                        CustomerPhone,
                        ManagerName,
                        PaymentStatus
                    FROM vw_OrderSummary
                    ORDER BY OrderDate DESC";

                using (SqlCommand command =
                    new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(MapOrder(reader));
                        }
                    }
                }
            }

            return orders;
        }

        public List<Order> GetOrdersByCustomer(int customerId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT
                        o.OrderID,
                        o.CustomerID,
                        o.ManagerID,
                        o.OrderDate,
                        o.OrderStatus,
                        o.TotalAmount,
                        c.Name AS CustomerName,
                        c.Phone AS CustomerPhone,
                        s.Name AS ManagerName,
                        p.PaymentStatus
                    FROM Orders o
                    INNER JOIN Customer c
                        ON o.CustomerID = c.CustomerID
                    INNER JOIN Manager s
                        ON o.ManagerID = s.ManagerID
                    LEFT JOIN Payment p
                        ON o.OrderID = p.OrderID
                    WHERE o.CustomerID = @CustomerID
                    ORDER BY o.OrderDate DESC";

                using (SqlCommand command =
                    new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@CustomerID", customerId);

                    connection.Open();

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(MapOrder(reader));
                        }
                    }
                }
            }

            return orders;
        }

        public List<Order> GetOrdersByManager(int managerId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT
                        o.OrderID,
                        o.CustomerID,
                        o.ManagerID,
                        o.OrderDate,
                        o.OrderStatus,
                        o.TotalAmount,
                        c.Name AS CustomerName,
                        c.Phone AS CustomerPhone,
                        s.Name AS ManagerName,
                        p.PaymentStatus
                    FROM Orders o
                    INNER JOIN Customer c
                        ON o.CustomerID = c.CustomerID
                    INNER JOIN Manager s
                        ON o.ManagerID = s.ManagerID
                    LEFT JOIN Payment p
                        ON o.OrderID = p.OrderID
                    WHERE o.ManagerID = @ManagerID
                    ORDER BY o.OrderDate DESC";

                using (SqlCommand command =
                    new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@ManagerID", managerId);

                    connection.Open();

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(MapOrder(reader));
                        }
                    }
                }
            }

            return orders;
        }

        public string GetOrderStatus(int orderId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = "SELECT OrderStatus FROM Orders WHERE OrderID = @OrderID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);
                    connection.Open();
                    object value = command.ExecuteScalar();
                    return value == null || value == DBNull.Value ? null : value.ToString();
                }
            }
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            List<OrderItem> items =
                new List<OrderItem>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT
                        oi.OrderItemID,
                        oi.OrderID,
                        oi.ProductID,
                        oi.Quantity,
                        oi.UnitPrice,
                        oi.SubTotal,
                        p.ProductName
                    FROM OrderItem oi
                    INNER JOIN Product p
                        ON oi.ProductID = p.ProductID
                    WHERE oi.OrderID = @OrderID";

                using (SqlCommand command =
                    new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@OrderID", orderId);

                    connection.Open();

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new OrderItem
                            {
                                OrderItemID =
                                    Convert.ToInt32(
                                        reader["OrderItemID"]),

                                OrderID =
                                    Convert.ToInt32(
                                        reader["OrderID"]),

                                ProductID =
                                    Convert.ToInt32(
                                        reader["ProductID"]),

                                Quantity =
                                    Convert.ToInt32(
                                        reader["Quantity"]),

                                UnitPrice =
                                    Convert.ToDecimal(
                                        reader["UnitPrice"]),

                                SubTotal =
                                    Convert.ToDecimal(
                                        reader["SubTotal"]),

                                ProductName =
                                    reader["ProductName"].ToString()
                            });
                        }
                    }
                }
            }

            return items;
        }

        private Order MapOrder(SqlDataReader reader)
        {
            return new Order
            {
                OrderID =
                    Convert.ToInt32(reader["OrderID"]),

                CustomerID =
                    Convert.ToInt32(reader["CustomerID"]),

                ManagerID =
                    Convert.ToInt32(reader["ManagerID"]),

                OrderDate =
                    Convert.ToDateTime(reader["OrderDate"]),

                OrderStatus =
                    reader["OrderStatus"].ToString(),

                TotalAmount =
                    Convert.ToDecimal(reader["TotalAmount"]),

                CustomerName =
                    reader["CustomerName"].ToString(),

                CustomerPhone =
                    reader["CustomerPhone"] == DBNull.Value
                        ? ""
                        : reader["CustomerPhone"].ToString(),

                ManagerName =
                    reader["ManagerName"].ToString(),

                PaymentStatus =
                    reader["PaymentStatus"] == DBNull.Value
                        ? "UNPAID"
                        : reader["PaymentStatus"].ToString()
            };
        }
    }
}
