using PetShopManagementSystem.Models;
using PetShopManagementSystem.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace PetShopManagementSystem.Repositories
{
    public class FoodTypeRepository
    {
        private readonly DBConnection db;

        public FoodTypeRepository()
        {
            db = new DBConnection();
        }

        public List<FoodType> GetAll()
        {
            List<FoodType> foodTypes = new List<FoodType>();

            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"
                    SELECT FoodTypeID, TypeName
                    FROM FoodType
                    ORDER BY TypeName";

                using (SqlCommand command =
                    new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foodTypes.Add(new FoodType
                            {
                                FoodTypeID =
                                    Convert.ToInt32(reader["FoodTypeID"]),

                                TypeName =
                                    reader["TypeName"].ToString()
                            });
                        }
                    }
                }
            }

            return foodTypes;
        }
    }
}
