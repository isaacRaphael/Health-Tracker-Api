using HealthTracker.Dataservice.Configurations;
using HealthTracker.Dataservice.Repositories;
using HealthTracker.Dataservice.RepositoryContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Dataservice.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public IUserRepository UserRepo { get; private set; }

        public UnitOfWork(AppDbContext context, ILoggerFactory loggerFac)
        {
            _context = context;
            _logger = loggerFac.CreateLogger("db_logs");


            UserRepo = new UserRepository(_context, _logger);
        }

        public async  Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
