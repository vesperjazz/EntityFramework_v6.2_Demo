using EntityFrameworkDemo.Domain.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFrameworkDemo.Persistence.Repository
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<IEnumerable<Person>> GetPersonsByLastNameAsync(string lastName);
        Task<IEnumerable<Person>> GetPersonsByGenderAsync(string gender);
        Task<IEnumerable<Person>> GetPersonsWithContactNumbersAsync_EagerLoading();
        Task<IEnumerable<Person>> GetPersonsWithContactNumbersAsync_ExplicitLoading();
        void UpdateBirthdayOnly(Person person);
    }
}
