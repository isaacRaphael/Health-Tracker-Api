using HealthTracker.Dataservice.Data;
using HealthTracker.Dataservice.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Dataservice.Repositories
{

    public class GenericrRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _appDbContext;
        internal DbSet<T> dbset;
        protected readonly ILogger _logger;

        public GenericrRepository(AppDbContext appDbContext, ILogger logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            dbset = _appDbContext.Set<T>();
        }

        public virtual async Task<bool> Add(T entity)
        {
            var done = await dbset.AddAsync(entity);
            return true;
        }

        public  Task<bool> Delete(Guid id, string userId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await dbset.ToListAsync();   
        }

        public virtual async  Task<T> GetById(Guid id)
        {
            return await dbset.FindAsync(id);
        }

        public Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
