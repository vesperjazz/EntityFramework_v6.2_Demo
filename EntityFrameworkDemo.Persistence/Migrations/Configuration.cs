namespace EntityFrameworkDemo.Persistence.Migrations
{
    using EntityFrameworkDemo.Domain.DomainModels;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EFDemoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(EFDemoContext context)
        {
            SeedGender(context);
            context.SaveChanges();
        }

        private void SeedGender(EFDemoContext context)
        {
            if (context.Genders.Any()) { return; }

            context.Genders.Add(new Gender { Name = "Male" });
            context.Genders.Add(new Gender { Name = "Female" });
            context.Genders.Add(new Gender { Name = "Unknown" });
        }
    }
}
