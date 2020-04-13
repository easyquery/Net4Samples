using System.Web.Http;
using System.Data.SqlClient;

using Korzh.EasyQuery.Services;
using Korzh.EasyQuery.AspNet;
using System.Web;

namespace EqWebFormsDemo.Controllers
{

    public class SessionPreExecuteTuner : IEasyQueryManagerTuner
    {
        public bool Tune(EasyQueryManager manager)
        {
            var districtCode = (string)HttpContext.Current.Session["DistrictCode"];

            manager.Query.ExtraConditions.AddSimpleCondition("PregnantWoman.DistrictCode", "Equal", districtCode);

            return true;
        }
    }

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

            options.AddPreExecuteTuner(new SessionPreExecuteTuner());

            options.UsePaging(30);
        }
    }
}
