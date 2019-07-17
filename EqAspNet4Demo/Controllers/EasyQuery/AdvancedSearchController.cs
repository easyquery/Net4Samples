using System.Web.Http;
using System.Data.SqlClient;

using Korzh.EasyQuery.Services;
using Korzh.EasyQuery.AspNet;

using EqAspNet4Demo.Services;

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

            var connectionString = ConfigurationManagerWrapper.GetConnectionString("DefaultConncetion");
            options.UseDbConnection<SqlConnection>(connectionString);

            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
            options.UseModelLoader((_) => new FileModelLoader(path));
            options.UseQueryStore((_) => new FileQueryStore(path));

            options.UsePaging(30);
        }
    }
}
