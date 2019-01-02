using System.Data.Entity.Migrations;
using Cafocha.Entities;

namespace Cafocha.BusinessContext
{
    internal sealed class MigrationConfiguration : DbMigrationsConfiguration<LocalContext>
    {
        public MigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        //protected override void Seed(AsowellContext context)
        //{

        // This method will be called after migrating to the latest version.

        // You can use the DbSet<T>.AddOrUpdate() helper extension method
        // to avoid creating duplicate seed data. E.g.
        //
        //   context.People.AddOrUpdate(
        //     p => p.FullName,
        //     new Person { FullName = "Andrew Peters" },
        //     new Person { FullName = "Brice Lambson" },
        //     new Person { FullName = "Rowan Miller" }
        //   );
        //
        //}
    }
}