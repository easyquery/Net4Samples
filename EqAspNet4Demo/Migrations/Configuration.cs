namespace EqAspNet4Demo.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EqAspNet4Demo.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "EqAspNet4Demo.Models.ApplicationDbContext";
        }

        protected override void Seed(EqAspNet4Demo.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var initializer = new DbIntializer(context.Database.Connection.ConnectionString);
            initializer.AddTestData();
        }
    }
}
