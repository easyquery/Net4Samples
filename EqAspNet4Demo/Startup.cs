using Microsoft.Owin;
using Owin;

using EqAspNet4Demo.Models;

[assembly: OwinStartupAttribute(typeof(EqAspNet4Demo.Startup))]
namespace EqAspNet4Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            using (var db = new ApplicationDbContext())
            {
                db.Database.Initialize(true);
            }
        }
    }
}
