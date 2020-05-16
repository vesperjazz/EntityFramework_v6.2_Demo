using EntityFrameworkDemo.Domain.DomainModels;
using System.Data.Entity;

namespace EntityFrameworkDemo.Persistence
{
    public class EFDemoContext : DbContext
    {
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonContactNumber> PersonContactNumbers { get; set; }

        // Tells the context to look for a connection string matching with name equivalent to "EFDemoContext"
        // Unless multiple connection is required for a single context, always do it like this.
        public EFDemoContext() : base("name=EFDemoContext") 
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EFDemoContext, Migrations.Configuration>(nameof(EFDemoContext)));
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Tells EF to name the Person table as Person, else EF will generate a plural Persons table.
            modelBuilder.Entity<Person>()
                .ToTable(nameof(Person));

            // This statement is redundant, EntityFramework works on convention over configuration.
            // All properties with Id/ID/PersonId/PersonID will be treated as Primary Key by convention.
            //modelBuilder.Entity<Person>()
            //    .HasKey(p => p.ID);
            modelBuilder.Entity<Person>()
                .Property(p => p.FirstName)
                .HasMaxLength(200)
                .IsRequired();
            modelBuilder.Entity<Person>()
                .Property(p => p.LastName)
                .HasMaxLength(200)
                .IsRequired();

            // Person has a one to many relationship with Gender.
            modelBuilder.Entity<Person>()
                .HasOptional(p => p.Gender)
                .WithMany(g => g.Persons)
                .HasForeignKey(p => p.GenderID);
            // The reverse is true, Gender has a many to one relationship with Person.
            // However, EF recognises the symmetric relationship and only one side of the relationship needs 
            // to be defined.
            //modelBuilder.Entity<Gender>()
            //    .HasMany(g => g.Persons)
            //    .WithOptional(p => p.Gender)
            //    .HasForeignKey(p => p.GenderID);

            // Person has a many to one relationship with PersonContactNumber.
            modelBuilder.Entity<Person>()
                .HasMany(p => p.PersonContactNumbers)
                .WithRequired(pcn => pcn.Person)
                .HasForeignKey(pcn => pcn.PersonID);
            // The reverse is true, but not required.
            //modelBuilder.Entity<PersonContactNumber>()
            //    .HasRequired(pcn => pcn.Person)
            //    .WithMany(p => p.PersonContactNumbers)
            //    .HasForeignKey(pcn => pcn.PersonID);

            modelBuilder.Entity<Gender>()
                .ToTable(nameof(Gender));
            modelBuilder.Entity<Gender>()
                .Property(g => g.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<PersonContactNumber>()
                .ToTable(nameof(PersonContactNumber));
            modelBuilder.Entity<PersonContactNumber>()
                .Property(pcn => pcn.PhoneNumber)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<PersonContactNumber>()
                .HasIndex(pcn => pcn.PhoneNumber)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
