using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Dataservice.RepositoryContracts
{
    public interface IGenericRepository<T> where T : class
    {
        //Get all
        Task<IEnumerable<T>> GetAll();
        //Get single entity
        Task<T> GetById(Guid id);
        //add'
        Task<bool> Add(T entity);
        //Delete
        Task<bool> Delete(Guid id, string userId);
        //Update Entity
        Task<bool> Upsert(T entity);
    }
}
