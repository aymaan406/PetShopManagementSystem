using PetShopManagementSystem.Models;
using System.Collections.Generic;

namespace PetShopManagementSystem.Interfaces
{
    public interface ICrudRepository<T>
    {
        List<T> GetAll(); /* fetch all products from DB */
        T GetById(int id);/* fetch one product */
        bool Add(T entity);/* insert into DB */
        bool Update(T entity); /* update in DB */
        bool Delete(int id); /* delete from DB */
    }
}
