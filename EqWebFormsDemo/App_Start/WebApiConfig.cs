using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Korzh.EasyQuery.Services;

namespace EqWebFormsDemo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //EasyQuery license keys
            Korzh.EasyQuery.AspNet.License.Key = "w5T2jSxvpyT39qaz5N6wegGBP921ZI";
            Korzh.EasyQuery.AspNet.JSLicense.Key = "AlzWbvUgrkISH9AEAEoV7wBKJXGX14";

            // Web API configuration and services
            var httpControllerRouteHandler = typeof(System.Web.Http.WebHost.HttpControllerRouteHandler).GetField("_instance",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            // To support Session in WebAPI
            if (httpControllerRouteHandler != null) {
                httpControllerRouteHandler.SetValue(null,
                    new Lazy<System.Web.Http.WebHost.HttpControllerRouteHandler>(() => new SessionHttpControllerRouteHandler(), true));
            }

            // Web API routes
            config.MapHttpAttributeRoutes(new WebApiCustomDirectRouteProvider());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Register you exportes here
            // to make export works
            EasyQueryManager.RegisterExporter("csv", new CsvDataExporter());
            EasyQueryManager.RegisterExporter("excel-html", new ExcelHtmlDataExporter());
        }
    }

    public class WebApiCustomDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory>
            GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            // inherit route attributes decorated on base class controller's actions
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
        }
    }
}
