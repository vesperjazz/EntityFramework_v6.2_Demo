using EntityFrameworkDemo.Domain.DomainModels;
using EntityFrameworkDemo.Persistence.Repository;
using System;
using System.Threading.Tasks;

namespace EntityFrameworkDemo.Persistence.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IPersonRepository PersonRepository { get; }
        IRepository<Gender> GenderRepository { get; }

        Task<int> CompleteAsync();
    }
}
