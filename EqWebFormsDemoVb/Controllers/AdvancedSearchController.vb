Imports System
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Web.Http

Imports Korzh.EasyQuery.AspNet
Imports Korzh.EasyQuery.Services

<RoutePrefix("api/easyquery")>
Public Class AdvancedSearchController : Inherits EasyQueryApiController

    Protected Overrides Sub ConfigureEasyQueryOptions(options As EasyQueryOptions)

        options.UseManager(Of EasyQueryManagerSql)()

        options.DefaultModelId = "NWindSQL"
        options.BuildQueryOnSync = True

        Dim connectionString As String = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString
        options.UseDbConnection(Of SqlConnection)(connectionString)

        Dim path As String = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data")
        Dim modelLoader As Func(Of IServiceProvider, IModelLoader) =
                Function()
                    Return New FileModelLoader(path)
                End Function
        Dim queryStore As Func(Of IServiceProvider, IQueryStore) =
                Function()
                    Return New FileQueryStore(path)
                End Function
        options.UseModelLoader(modelLoader)
        options.UseQueryStore(queryStore)

        options.UsePaging(30)

    End Sub
End Class
