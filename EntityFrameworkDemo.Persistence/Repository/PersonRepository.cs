using EntityFrameworkDemo.Domain.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkDemo.Persistence.Repository
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        private readonly EFDemoContext _efDemoContext;
        public PersonRepository(EFDemoContext efDemoContext) : base(efDemoContext)
        {
            _efDemoContext = efDemoContext;
        }

        public async Task<IEnumerable<Person>> GetPersonsByLastNameAsync(string lastName)
        {
            return await _efDemoContext.Persons
                .Where(p => p.LastName == lastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonsByGenderAsync(string gender)
        {
            return await _efDemoContext.Persons
                .Include(p => p.Gender)
                .Where(p => p.Gender.Name == gender)
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonsWithContactNumbersAsync_EagerLoading()
        {
            // A single round trip to the Database
            return await _efDemoContext.Persons
                .Include(p => p.PersonContactNumbers)
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonsWithContactNumbersAsync_ExplicitLoading()
        {
            // First round trip to the Database
            var persons = await _efDemoContext.Persons.ToListAsync();
            var personIDs = persons.Select(p => p.ID);

            // Second round trip to the Database
            _efDemoContext.PersonContactNumbers
                .Where(x => personIDs.Contains(x.PersonID))
                .Load();

            // Sometimes, explicit loading will have a higher performance than EagerLoading.
            // It all depends on the kind of data, and a suitable choice must be made by the developer.
            return persons;
        }

        public void UpdateBirthdayOnly(Person person)
        {
            _efDemoContext.Persons.Attach(person);
            _efDemoContext.Entry(person).Property(p => p.DateOfBirth).IsModified = true;
        }
    }
}
