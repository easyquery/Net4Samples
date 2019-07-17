using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace EqWebFormsDemo
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            var initializer = new DbInitializer(ConfigurationManagerWrapper.GetConnectionString("DefaultConnection"));
            initializer.EnsureCreated();

            // Code that runs on application startup
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}