using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PetShopManagementSystem.Database;
using PetShopManagementSystem.Interfaces;
using PetShopManagementSystem.Models;

namespace PetShopManagementSystem.Repositories
{
    public class ProductRepository : ICrudRepository<Product>
    {
        private readonly DBConnection db;

        public ProductRepository()
        {
            db = new DBConnection();
        }

        public List<Product> GetAll()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT 
                        p.ProductID,
                        p.ManagerID,
                        p.FoodTypeID,
                        p.ProductName,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        f.TypeName AS FoodTypeName
                    FROM Product p
                    INNER JOIN FoodType f 
                        ON p.FoodTypeID = f.FoodTypeID
                    ORDER BY p.ProductID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(MapProduct(reader));
                        }
                    }
                }
            }

            return products;
        }

        public Product GetById(int id)
        {
            Product product = null;

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT 
                        p.ProductID,
                        p.ManagerID,
                        p.FoodTypeID,
                        p.ProductName,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        f.TypeName AS FoodTypeName
                    FROM Product p
                    INNER JOIN FoodType f 
                        ON p.FoodTypeID = f.FoodTypeID
                    WHERE p.ProductID = @ProductID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", id);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = MapProduct(reader);
                        }
                    }
                }
            }

            return product;
        }

        public List<Product> GetByManager(int managerId)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT 
                        p.ProductID,
                        p.ManagerID,
                        p.FoodTypeID,
                        p.ProductName,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        f.TypeName AS FoodTypeName
                    FROM Product p
                    INNER JOIN FoodType f 
                        ON p.FoodTypeID = f.FoodTypeID
                    WHERE p.ManagerID = @ManagerID
                    ORDER BY p.ProductID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ManagerID", managerId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(MapProduct(reader));
                        }
                    }
                }
            }

            return products;
        }

        public List<Product> Search(string keyword)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT 
                        p.ProductID,
                        p.ManagerID,
                        p.FoodTypeID,
                        p.ProductName,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        f.TypeName AS FoodTypeName
                    FROM Product p
                    INNER JOIN FoodType f 
                        ON p.FoodTypeID = f.FoodTypeID
                    WHERE p.ProductName LIKE @Keyword
                       OR p.Description LIKE @Keyword
                       OR f.TypeName LIKE @Keyword
                    ORDER BY p.ProductID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(MapProduct(reader));
                        }
                    }
                }
            }

            return products;
        }

        public bool Add(Product product)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    INSERT INTO Product
                    (
                        ManagerID,
                        FoodTypeID,
                        ProductName,
                        Description,
                        Price,
                        StockQuantity
                    )
                    VALUES
                    (
                        @ManagerID,
                        @FoodTypeID,
                        @ProductName,
                        @Description,
                        @Price,
                        @StockQuantity
                    )";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    AddProductParameters(command, product);

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(Product product)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    UPDATE Product
                    SET
                        FoodTypeID = @FoodTypeID,
                        ProductName = @ProductName,
                        Description = @Description,
                        Price = @Price,
                        StockQuantity = @StockQuantity
                    WHERE ProductID = @ProductID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", product.ProductID);

                    command.Parameters.AddWithValue(
                        "@FoodTypeID", product.FoodTypeID);

                    command.Parameters.AddWithValue(
                        "@ProductName", product.ProductName);

                    command.Parameters.AddWithValue(
                        "@Description",
                        string.IsNullOrEmpty(product.Description)
                            ? (object)DBNull.Value
                            : product.Description);

                    command.Parameters.AddWithValue(
                        "@Price", product.Price);

                    command.Parameters.AddWithValue(
                        "@StockQuantity", product.StockQuantity);

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    DELETE FROM Product
                    WHERE ProductID = @ProductID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", id);

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        private Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                ProductID = Convert.ToInt32(reader["ProductID"]),
                ManagerID = Convert.ToInt32(reader["ManagerID"]),
                FoodTypeID = Convert.ToInt32(reader["FoodTypeID"]),
                ProductName = reader["ProductName"].ToString(),
                Description = reader["Description"] == DBNull.Value
                    ? ""
                    : reader["Description"].ToString(),
                Price = Convert.ToDecimal(reader["Price"]),
                StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                FoodTypeName = reader["FoodTypeName"].ToString()
            };
        }

        private void AddProductParameters(
            SqlCommand command,
            Product product)
        {
            command.Parameters.AddWithValue(
                "@ManagerID", product.ManagerID);

            command.Parameters.AddWithValue(
                "@FoodTypeID", product.FoodTypeID);

            command.Parameters.AddWithValue(
                "@ProductName", product.ProductName);

            command.Parameters.AddWithValue(
                "@Description",
                string.IsNullOrEmpty(product.Description)
                    ? (object)DBNull.Value
                    : product.Description);

            command.Parameters.AddWithValue(
                "@Price", product.Price);

            command.Parameters.AddWithValue(
                "@StockQuantity", product.StockQuantity);
        }
    }
}
