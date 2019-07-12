using System;
using System.Configuration;
using System.Web.Http;
using System.Data.SqlClient;

using Korzh.EasyQuery.Services;
using Korzh.EasyQuery.AspNet;

namespace EqAspNet4Demo.Controllers
{
    [RoutePrefix("api/easyquery")]
    public class EqApiController : EasyQueryApiController
    {
        public EqApiController() : base() { }

        static EqApiController()
        {
            // Register you exportes here
            // to make export works

            EasyQueryManagerBase.RegisterExporter("csv", new CsvDataExporter());
            EasyQueryManagerBase.RegisterExporter("excel-html", new ExcelHtmlDataExporter());
        }

        protected override void ConfigureEasyQueryOptions(EasyQueryOptions options)
        {
            options.DefaultModelId = "NWindSQL";

            options.BuildQueryOnSync = true;
            options.SaveQueryOnSync = false;

            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");

            options.UseDbConnection<SqlConnection>(ConfigurationManagerWrapper.GetConnectionString("DefaultConnection"));
            options.UseModelLoader((_) => new FileModelLoader(path));
            options.UseQueryStore((_) => new FileQueryStore(path));
            options.UsePaging(30);
        }

        protected override EasyQueryManagerBase CreateEasyQueryManager(IServiceProvider services, EasyQueryOptions options)
        {
            return new EasyQueryManagerSql(services, options);
        }
    }
}
