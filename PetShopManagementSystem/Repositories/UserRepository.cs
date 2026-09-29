using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using PetShopManagementSystem.Database;
using PetShopManagementSystem.Models;

namespace PetShopManagementSystem.Repositories
{
    public class UserRepository
    {
        private readonly DBConnection db;

        public UserRepository()
        {
            db = new DBConnection();
        }

        public bool UsernameExists(string username)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM
                    (
                        SELECT Username FROM Admin
                        UNION
                        SELECT Username FROM Manager
                        UNION
                        SELECT Username FROM Customer
                    ) AS Users
                    WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool UsernameExistsForAnotherManager(string username, int managerId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM
                    (
                        SELECT Username FROM Admin WHERE Username = @Username
                        UNION ALL
                        SELECT Username FROM Customer WHERE Username = @Username
                        UNION ALL
                        SELECT Username FROM Manager
                        WHERE Username = @Username AND ManagerID <> @ManagerID
                    ) AS Users";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@ManagerID", managerId);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool AddManager(Manager manager)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    INSERT INTO Manager
                    (Name, Username, Password, Email, Phone, Status)
                    VALUES
                    (@Name, @Username, @Password, @Email, @Phone, @Status)";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    AddNullableParameter(command, "@Name", manager.Name);
                    AddNullableParameter(command, "@Username", manager.Username);
                    AddNullableParameter(command, "@Password", manager.Password);
                    AddNullableParameter(command, "@Email", manager.Email);
                    AddNullableParameter(command, "@Phone", manager.Phone);
                    command.Parameters.AddWithValue("@Status", manager.Status);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool AddCustomer(Customer customer)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
            INSERT INTO Customer
            (Name, Username, Password, Email, Phone, Address, Status)
            VALUES
            (@Name, @Username, @Password, @Email, @Phone, @Address, @Status)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    AddNullableParameter(command, "@Name", customer.Name);
                    AddNullableParameter(command, "@Username", customer.Username);
                    AddNullableParameter(command, "@Password", customer.Password);
                    AddNullableParameter(command, "@Email", customer.Email);
                    AddNullableParameter(command, "@Phone", customer.Phone);
                    AddNullableParameter(command, "@Address", customer.Address);

                    // New customers must wait for Admin approval.
                    command.Parameters.AddWithValue("@Status", 0);

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }


        public bool UpdateManager(Manager manager, bool updatePassword)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query;

                if (updatePassword)
                {
                    query = @"
                        UPDATE Manager
                        SET Name = @Name,
                            Username = @Username,
                            Password = @Password,
                            Email = @Email,
                            Phone = @Phone,
                            Status = @Status
                        WHERE ManagerID = @ManagerID";
                }
                else
                {
                    query = @"
                        UPDATE Manager
                        SET Name = @Name,
                            Username = @Username,
                            Email = @Email,
                            Phone = @Phone,
                            Status = @Status
                        WHERE ManagerID = @ManagerID";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ManagerID", manager.ManagerID);
                    AddNullableParameter(command, "@Name", manager.Name);
                    AddNullableParameter(command, "@Username", manager.Username);
                    AddNullableParameter(command, "@Email", manager.Email);
                    AddNullableParameter(command, "@Phone", manager.Phone);
                    command.Parameters.AddWithValue("@Status", manager.Status);

                    if (updatePassword)
                        AddNullableParameter(command, "@Password", manager.Password);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteManager(int managerId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                connection.Open();

                // Keep existing products/orders safe. A manager with related
                // records cannot be physically deleted.
                string dependencyQuery = @"
                    SELECT
                        (SELECT COUNT(*) FROM Product WHERE ManagerID = @ManagerID) +
                        (SELECT COUNT(*) FROM Orders WHERE ManagerID = @ManagerID)";

                using (SqlCommand dependencyCommand =
                    new SqlCommand(dependencyQuery, connection))
                {
                    dependencyCommand.Parameters.AddWithValue("@ManagerID", managerId);

                    int dependencyCount =
                        Convert.ToInt32(dependencyCommand.ExecuteScalar());

                    if (dependencyCount > 0)
                        return false;
                }

                using (SqlCommand command =
                    new SqlCommand("DELETE FROM Manager WHERE ManagerID = @ManagerID", connection))
                {
                    command.Parameters.AddWithValue("@ManagerID", managerId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT CustomerID, Name, Username, Email, Phone, Address, Status
                    FROM Customer
                    ORDER BY CustomerID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                Name = reader["Name"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                                Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString(),
                                Address = reader["Address"] == DBNull.Value ? "" : reader["Address"].ToString(),
                                Status = Convert.ToInt32(reader["Status"])
                            });
                        }
                    }
                }
            }

            return customers;
        }

        public List<Manager> GetManagers()
        {
            List<Manager> managers = new List<Manager>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT ManagerID, Name, Username, Email, Phone, Status
                    FROM Manager
                    ORDER BY ManagerID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            managers.Add(new Manager
                            {
                                ManagerID = Convert.ToInt32(reader["ManagerID"]),
                                Name = reader["Name"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                                Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString(),
                                Status = Convert.ToInt32(reader["Status"])
                            });
                        }
                    }
                }
            }

            return managers;
        }

        public bool UpdateManagerStatus(int managerId, int status)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"UPDATE Manager SET Status = @Status WHERE ManagerID = @ManagerID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@ManagerID", managerId);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateCustomerStatus(int customerId, int status)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"UPDATE Customer SET Status = @Status WHERE CustomerID = @CustomerID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@CustomerID", customerId);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }


        public bool UsernameExistsForAnotherCustomer(string username, int customerId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM
                    (
                        SELECT Username FROM Admin WHERE Username = @Username
                        UNION ALL
                        SELECT Username FROM Manager WHERE Username = @Username
                        UNION ALL
                        SELECT Username FROM Customer
                        WHERE Username = @Username AND CustomerID <> @CustomerID
                    ) AS Users";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@CustomerID", customerId);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public Customer GetCustomerById(int customerId)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT CustomerID, Name, Username, Email, Phone, Address, Status
                    FROM Customer
                    WHERE CustomerID = @CustomerID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Customer
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                Name = reader["Name"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                                Phone = reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString(),
                                Address = reader["Address"] == DBNull.Value ? "" : reader["Address"].ToString(),
                                Status = Convert.ToInt32(reader["Status"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public bool UpdateCustomerProfile(Customer customer)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    UPDATE Customer
                    SET Name = @Name,
                        Username = @Username,
                        Email = @Email,
                        Phone = @Phone,
                        Address = @Address
                    WHERE CustomerID = @CustomerID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    AddNullableParameter(command, "@Name", customer.Name);
                    AddNullableParameter(command, "@Username", customer.Username);
                    AddNullableParameter(command, "@Email", customer.Email);
                    AddNullableParameter(command, "@Phone", customer.Phone);
                    AddNullableParameter(command, "@Address", customer.Address);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateCustomerPassword(int customerId, string hashedPassword)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    UPDATE Customer
                    SET Password = @Password
                    WHERE CustomerID = @CustomerID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerId);
                    AddNullableParameter(command, "@Password", hashedPassword);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        private void AddNullableParameter(SqlCommand command, string name, string value)
        {
            command.Parameters.AddWithValue(
                name,
                string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value);
        }
    }
}
