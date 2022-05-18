using HealthTracker.Dataservice.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Dataservice.Configurations
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepo { get;  }

        Task CompleteAsync();
    }
}
