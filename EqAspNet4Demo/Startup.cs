using Microsoft.Owin;
using Owin;

using System.Data.Entity.Migrations;

using EqAspNet4Demo.Migrations;

[assembly: OwinStartupAttribute(typeof(EqAspNet4Demo.Startup))]
namespace EqAspNet4Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var databaseMigrator = new DbMigrator(new Configuration());
            databaseMigrator.Update();

            IdentityHelper.SeedEqManagerRole();
            IdentityHelper.SeedDefaultUser();
        }
    }
}
