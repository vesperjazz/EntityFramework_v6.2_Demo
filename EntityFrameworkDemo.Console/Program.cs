using EntityFrameworkDemo.Domain.DomainModels;
using EntityFrameworkDemo.Persistence;
using EntityFrameworkDemo.Persistence.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkDemo.Console
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            // Note that each of these methods are different Database transaction.

            #region Create

            await InsertDummyDataAsync();

            #endregion

            #region Update

            var persons = await UpdateRecordsWithSelectAsync();
            await UpdateRecordsWithoutSelectAsync(persons);
            await UpdateRecordsWithoutTrackingAsync();

            #endregion

            #region Read (Lazy, Eager and Explicit loading for EntityFramework)

            await GetPersonsAsync_LazyLoading();
            await GetPersonsAsync_EagerOrExplicitLoading();

            #endregion

            #region Delete

            await RemovePersonsAsync();

            #endregion
        }

        private static async Task InsertDummyDataAsync()
        {
            using (var efDemoContext = new EFDemoContext())
            {
                // The unit of work exposes the repositories and controls the transaction.
                // The repository provides persistence ignorance, as in the caller doesn't have
                // the knowledge where and how the data is being read or saved.
                // This obeys the principle of abstraction.
                // The responsibility to return data is on the repository and not the service!
                using (var unitOfWork = new UnitOfWork(efDemoContext))
                {
                    // This is the very first call of the efDemoContext.
                    // Since we have enabled Automatic migrations, 3 things will now happen.
                    // 1. EntityFramework will check if the database exists based on the definition in the connection string.
                    //    If not exist, EntityFramework will generate SQL scripts based on the POCO models defined by the EFDemoContext. 
                    //    The SQL scripts will then be executed by the SQL intance defined by the connection string.
                    // 2. The Seed method in Configuration.cs will run once to seed the database.
                    // 3. The query, in this case, get all genders, will then be run.
                    var isDummyDataInserted = await unitOfWork.PersonRepository.AnyAsync();

                    if (isDummyDataInserted) { return; }

                    var genders = await unitOfWork.GenderRepository.GetAllAsync();
                    var male = genders.Single(g => g.Name == "Male");
                    var female = genders.Single(g => g.Name == "Female");

                    var aragorn = new Person
                    {
                        FirstName = "Aragorn",
                        LastName = "Elessar",
                        DateOfBirth = new DateTime(1991, 3, 14),
                        // Using the navigation property of gender.
                        Gender = male,
                        PersonContactNumbers = new List<PersonContactNumber>
                        {
                            // The relationship is implicit here, no need to define PersonID.
                            new PersonContactNumber { PhoneNumber = "86930853" }
                        }
                    };
                    var arwen = new Person
                    {
                        FirstName = "Arwen",
                        LastName = "Undomiel",
                        DateOfBirth = new DateTime(1996, 5, 10),
                        Gender = female
                    };
                    var gandalf = new Person
                    {
                        FirstName = "Gandalf",
                        LastName = "Greyhame",
                        DateOfBirth = new DateTime(1900, 12, 12),
                        // Using the foreign key of gender, works the same as navigation property!
                        GenderID = male.ID,
                        PersonContactNumbers = new List<PersonContactNumber>
                        {
                            new PersonContactNumber { PhoneNumber = "88889999" },
                            new PersonContactNumber { PhoneNumber = "77776666" }
                        }
                    };

                    // The following is normally called in the Service layer.
                    unitOfWork.PersonRepository.Add(aragorn);
                    unitOfWork.PersonRepository.AddRange(new List<Person> { arwen, gandalf });

                    // This is normally called in the Controller layer.
                    // The transaction is controlled such that any exception that happens in 
                    // the service layer will bubble up and hence the following line will not be called!
                    await unitOfWork.CompleteAsync();
                }
            }
        }

        private static async Task<IEnumerable<Person>> UpdateRecordsWithSelectAsync()
        {
            using (var efDemoContext = new EFDemoContext())
            {
                using (var unitOfWork = new UnitOfWork(efDemoContext))
                {
                    // Observe the gender property of each person here.
                    // The Gender navigation property is not explicitly joined in the query,
                    // so it will be NULL for all person objects.
                    var persons = await unitOfWork.PersonRepository.GetAllAsync();

                    // Observe the gender property of each person above AGAIN.
                    // The gender navigation property is automatically populated as EntityFramework
                    // has decided that the Gender navigation property is the same by comparing the
                    // GenderID of person to the ID of Gender under the same EFDemoContext scope.
                    var genders = await unitOfWork.GenderRepository.GetAllAsync();

                    // Since the objects are tracked and within the same scope,
                    // updating the values implies an Update query to the database.
                    // The downside of this kind of operation is that the data has to be first selected
                    // from the database before the values can be updated, what if we simply want to update
                    // a record without selecting it first?
                    foreach(var person in persons)
                    {
                        person.DateOfBirth = DateTime.Now.AddYears(10);
                    }

                    await unitOfWork.CompleteAsync();

                    return persons;
                }
            }
        }

        private static async Task UpdateRecordsWithoutSelectAsync(IEnumerable<Person> persons)
        {
            using (var efDemoContext = new EFDemoContext())
            {
                using (var unitOfWork = new UnitOfWork(efDemoContext))
                {
                    var currentDateTime = DateTime.Now;
                    foreach(var person in persons)
                    {
                        person.DateOfBirth = currentDateTime;

                        // A single column update for Birthday.
                        unitOfWork.PersonRepository.UpdateBirthdayOnly(person);
                    }
                    await unitOfWork.CompleteAsync();
                    // Take a look at the Database at this point.
                    
                    foreach(var person in persons)
                    {
                        person.FirstName = $"Updated {person.FirstName}";
                        person.LastName = $"Updated {person.LastName}";

                        unitOfWork.PersonRepository.Update(person);
                    }
                    await unitOfWork.CompleteAsync();
                    // Take a look at the Database at this point.
                }
            }
        }

        private static async Task UpdateRecordsWithoutTrackingAsync()
        {
            using (var efDemoContext = new EFDemoContext())
            {
                using (var unitOfWork = new UnitOfWork(efDemoContext))
                {
                    var untrackedPersons = await unitOfWork.PersonRepository.GetAllAsNoTrackingAsync();

                    foreach(var untrackedPerson in untrackedPersons)
                    {
                        // Everything done here will not make a difference.
                        untrackedPerson.FirstName = string.Empty;
                        untrackedPerson.LastName = string.Empty;
                    }

                    // Untracked entities are detached from EntityFramework and 
                    // the following line will have no effect.
                    await unitOfWork.CompleteAsync();
                }
            }
        }

        private static async Task GetPersonsAsync_LazyLoading()
        {
            using (var efDemoContext = new EFDemoContext())
            {
                using (var unitOfWork = new UnitOfWork(efDemoContext))
                {
                    // Run this with an SQL Server Profiler to see what's going on.
                    // Notice that this query has no explicit or eager loading of PersonContactNumber.
                    var persons = await unitOfWork.PersonRepository.GetAllAsync();

                    // The PersonContactNumbers navigation property in the Person object must also be
                    // marked as virtual for lazy loading to work.
                    foreach (var person in persons)
                    {
                        // When the PersonContactNumbers is being used, a separate query is then sent
                        // to the Database to obtain the particular records.
                        // This is the famous N+1 problem, 1 initial query to the database, but due to looping,
                        // N times other query is sent to the database!!
                        var personContactNumbers = person.PersonContactNumbers;

                        // Lazy loading should be avoided at all costs, unless the developer
                        // knows clearly what the fuck is going on.
                    }
                }
            }
        }

        private static async Task GetPersonsAsync_EagerOrExplicitLoading()
        {
            using (var efDemoContext = new EFDemoContext())
            {
                using (var unitOfWork = new UnitOfWork(efDemoContext))
                {
                    // This performs an inner join.
                    var personsWithContact = await unitOfWork.PersonRepository
                        .GetPersonsWithContactNumbersAsync_EagerLoading();

                    // This is ORM exclusive, 2 separate queries are sent to the Database.
                    //var personsWithContact = await unitOfWork.PersonRepository
                    //    .GetPersonsWithContactNumbersAsync_ExplicitLoading();

                    unitOfWork.PersonRepository.RemoveRange(personsWithContact);

                    // Notice that PersonContactNumber is dependent on Person,
                    // a deletion on Person will cascade to PersonContactNumber as well.
                    await unitOfWork.CompleteAsync();
                }
            }
        }

        private static async Task RemovePersonsAsync()
        {
            using (var efDemoContext = new EFDemoContext())
            {
                using (var unitOfWork = new UnitOfWork(efDemoContext))
                {
                    // No difference for eager or explicit loading.
                    var personsWithContact = await unitOfWork.PersonRepository
                        .GetPersonsWithContactNumbersAsync_ExplicitLoading();

                    unitOfWork.PersonRepository.RemoveRange(personsWithContact);

                    // Notice that PersonContactNumber is dependent on Person,
                    // a deletion on Person will cascade to PersonContactNumber as well.
                    await unitOfWork.CompleteAsync();
                }
            }
        }
    }
}
