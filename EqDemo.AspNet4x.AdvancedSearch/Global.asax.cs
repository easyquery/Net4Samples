using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using EqDemo.Migrations;

namespace EqDemo.AspNet4x.AdvancedSearch
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // License key
            Korzh.EasyQuery.AspNet.License.Key = "ZNQbiled5JoJMwzArFmSSQDSU0J8UX";
            Korzh.EasyQuery.AspNet.JSLicense.Key = "voC5XVNcovrofRwXkHNKEABNJ4VH40";

            // init db
            var databaseMigrator = new DbMigrator(new Configuration());
            databaseMigrator.Update();
        }
    }
}
