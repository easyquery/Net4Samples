Imports System
Imports System.Configuration
Imports System.Web
Imports System.Web.Http
Imports System.Web.Optimization
Imports System.Web.Routing

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)

        ' Create test db
        Dim connectionString As String = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString
        Dim initializer As DbInitializer = New DbInitializer(connectionString)
        initializer.EnsureCreated()

        ' Fires when the application is started
        GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)

    End Sub

End Class