using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using PetShopManagementSystem.Database;
using PetShopManagementSystem.Models;

namespace PetShopManagementSystem.Repositories
{
    public class LoginRepository
    {
        private readonly DBConnection db;

        public LoginRepository()
        {
            db = new DBConnection();
        }

        public LoginResult Login(string username, string password)
        {
            LoginResult result = ExecuteLogin(username, password);

            // The provided SQL script contains seed accounts using plain text.
            // If the first attempt fails, try SHA256 for newly registered users.
            if (result == null || !result.IsSuccessful)
            {
                string hashedPassword = HashPassword(password);
                result = ExecuteLogin(username, hashedPassword);
            }

            return result;
        }

        public string GetCustomerStatus(string username)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT Status
                    FROM Customer
                    WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    object value = command.ExecuteScalar();

                    if (value == null || value == System.DBNull.Value)
                        return null;

                    int status = System.Convert.ToInt32(value);

                    if (status == 1) return "ACTIVE";
                    if (status == 2) return "BLOCKED";
                    return "INACTIVE";
                }
            }
        }

        private LoginResult ExecuteLogin(string username, string password)
        {
            LoginResult result = null;

            using (SqlConnection connection = db.GetConnection())
            {
                using (SqlCommand command =
                    new SqlCommand("sp_Login", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Role")))
                            {
                                result = new LoginResult
                                {
                                    Role = reader["Role"].ToString(),
                                    UserID = ConvertToInt(reader["UserID"]),
                                    Name = reader["Name"].ToString()
                                };
                            }
                        }
                    }
                }
            }

            return result;
        }

        private int ConvertToInt(object value)
        {
            if (value == null || value == System.DBNull.Value)
                return 0;

            return System.Convert.ToInt32(value);
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();

                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
