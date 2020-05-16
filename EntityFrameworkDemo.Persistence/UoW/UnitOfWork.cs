using EntityFrameworkDemo.Domain.DomainModels;
using EntityFrameworkDemo.Persistence.Repository;
using System.Threading.Tasks;

namespace EntityFrameworkDemo.Persistence.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EFDemoContext _efDemoContext;

        private PersonRepository _personRepository;
        public IPersonRepository PersonRepository => _personRepository 
            ?? (_personRepository = new PersonRepository(_efDemoContext));

        private Repository<Gender> _genderRepository;
        public IRepository<Gender> GenderRepository => _genderRepository 
            ?? (_genderRepository = new Repository<Gender>(_efDemoContext));

        public UnitOfWork(EFDemoContext efDemoContext)
        {
            _efDemoContext = efDemoContext;
        }

        public async Task<int> CompleteAsync()
        {
            return await _efDemoContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _efDemoContext.Dispose();
        }
    }
}
