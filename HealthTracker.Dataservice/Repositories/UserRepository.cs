using HealthTracker.Dataservice.Data;
using HealthTracker.Dataservice.RepositoryContracts;
using HealthTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Dataservice.Repositories
{
    public class UserRepository : GenericrRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context,
             ILogger logger) : base(context, logger)
        {

        }
        public override async Task<IEnumerable<User>> GetAll()
        {
            try
            {
                return await dbset.Where(x => x.Status == 1)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll method has generated an error", typeof(UserRepository));
                return new List<User>();
            }
        }

    }
}
