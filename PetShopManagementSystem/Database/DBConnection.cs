using System.Configuration;
using System.Data.SqlClient;

namespace PetShopManagementSystem.Database
{
    public class DBConnection
    {
        private readonly string connectionString;

        public DBConnection()
        {
            connectionString =
                ConfigurationManager.ConnectionStrings["PetShopDB"].ConnectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
