using System;
using System.Configuration;
using System.Web.Http;
using System.Data.SqlClient;

using Korzh.EasyQuery.Services;
using Korzh.EasyQuery.AspNet;

namespace EqAspNet4Demo.Controllers
{
    [RoutePrefix("api/easyquery")]
    public class AdvancedSearchController : EasyQueryApiController
    {
        protected override void ConfigureEasyQueryOptions(EasyQueryOptions options)
        {
            options.UseManager<EasyQueryManagerSql>();
            options.DefaultModelId = "NWindSQL";
            options.BuildQueryOnSync = true;

            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            if (connectionSettings == null || string.IsNullOrEmpty(connectionSettings.ConnectionString)) {
                throw new Exception("Fatal error: missing connecting string in web.config file");
            }

            options.UseDbConnection<SqlConnection>(connectionSettings.ConnectionString);

            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
            options.UseModelLoader((_) => new FileModelLoader(path));
            options.UseQueryStore((_) => new FileQueryStore(path));

            // Uncomment this line if you want to load model directly from connection 
            // Do not forget to uncomment SqlClientGate registration in WebApiConfig.cs file
            //options.UseDbConnectionModelLoader(config => {
            //    // Ignores Asp.Net Identity tables
            //    config.AddTableFilter((table) => !table.Name.StartsWith("Asp") 
            //                                  && table.Name != "IdentityUsers"
            //                                  && table.Name != "__MigrationHistory");

            //    // Ignores Reports table
            //    config.AddTableFilter((table) => table.Name != "Reports");
            //});

            options.UsePaging(30);
        }
    }
}
