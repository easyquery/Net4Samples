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
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes(new WebApiCustomDirectRouteProvider());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Register you exportes here
            // to make export works
            EasyQueryManagerBase.RegisterExporter("csv", new CsvDataExporter());
            EasyQueryManagerBase.RegisterExporter("excel-html", new ExcelHtmlDataExporter());
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
