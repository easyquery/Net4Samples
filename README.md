# EasyQuery .NET 4 Samples


This repository contains several .NET 4.x projects which show how to use [EasyQuery library](https://korzh.com/easyquery) in different scenarios.

## EasyQuery packages

|NuGet Stable|NuGet Preview|NPM Latest|
|---|---|---|
|[![NuGet](https://img.shields.io/nuget/v/Korzh.EasyQuery.AspNet4)](https://www.nuget.org/packages/Korzh.EasyQuery.AspNet4)|[![NuGet](https://img.shields.io/nuget/vpre/Korzh.EasyQuery.AspNet4)](https://www.nuget.org/packages/Korzh.EasyQuery.AspNet4)|[![Npm](https://img.shields.io/npm/v/@easyquery/ui/latest)](https://www.npmjs.com/package/@easyquery/ui)|

## EasyQuery.JS browsers support

| [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/edge/edge_48x48.png" alt="IE / Edge" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>IE / Edge | [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/firefox/firefox_48x48.png" alt="Firefox" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Firefox | [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/chrome/chrome_48x48.png" alt="Chrome" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Chrome | [<img src="https://raw.githubusercontent.com/alrra/browser-logos/master/src/opera/opera_48x48.png" alt="Opera" width="24px" height="24px" />](http://godban.github.io/browsers-support-badges/)</br>Opera | ![Without jQuery](https://i.ibb.co/ZKSGMjt/no-jquery-logo.jpg)
| --------- | --------- | --------- | --------- | --------- |
| IE11, Edge| last version| last version| last version | without jQuery |

## Prerequisites

To run these samples you will need:

* [.NET 4.6.1](https://dotnet.microsoft.com/download/dotnet-framework/net48) or higher
* [SQL Server Express LocalDB](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express) (it's installed automatically with Visual Studio)
* Visual Studio 2017 or higher

## Getting started with Visual Studio

* Clone the repository
* Open EqNet4Samples.sln solution file in Visual Studio
* Build and run.

## EqAspNet4Demo project

This project implements several of the most usual scenarios of using EasyQuery in an ASP.NET MVC web-application. We tried to combine all these cases in one application for two reasons:

* to simplify the demonstration process since it's easier to set up and run one project instead of several different projects.
* to show how to configure different scenarios of using EasyQuery in one application.

So, when you start this sample project you will see an index page which leads you to the following demo pages:

### Advanced search

The page itself is implemented as a MVC view (`Views/Home/AdvancedSearch.cshtml`) and it corresponds to the `AdvancedSearch` action in the `HomeController`. The scripts and CSS files are taken directly from our CDN and the initialization of the client-side code was done right in the `Scripts` section of .cshtml file.
All AJAX requests from this page are handled by `AdvancedSearchController` WebAPI controller (`Controllers/EasyQuery/AdvancedSearchController.cs`). It is listening for requests on `/api/easyquery` endpoint. The model is loaded from the XML file `App_Data/NWindSQL.xml` but you can easily switch to loading it directly from DB connection (just replace `options.UseModelLoader((_) => new FileModelLoader(path));` line with `options.UseDbConnectionModelLoader()`).

### Ad-hoc reporting

This is the page which demonstrates full capabilities of EasyQuery library: columns editing (with `ColumnsBar` widget), saving/loading of the queries (reports) to some server-side storage and loading the data model directly from a DbContext.
The page is available at `Views/Home/AdhocReporting.cshtml`. The server-side part is processed by `EasyQueryEasyReportController` ( you can find it in `Controllers/EasyQuery/EasyReportController.cs`).

### Data filtering

To implement this scenario we used a totally different approach. The page which is responsible for the implementation of this scenario is available at `Views/Order/Orders.cshtml`. The only widget added on that page is `FilterBar`.

Instead of WebAPI controller, the server-side part in this scenario is handled by a usual MVC controller (`Controllers/OrderController.cs` file). Basically, in addition to `Index` action it contains only 3 extra methods which handles the requests from EasyQuery client-side code: `GetModel` (returns the model), `GetList` (returns the lists of values for lookup columns) and `ApplyQueryFilter` which executes the query (filter) over `Orders` DbSet using `DynamicQuery` extension method and passes the result lost of orders to `_OrderListPartial` partial view for rendering.

This is a great demonstration of using EasyQuery components without a WebAPI.

### Full-text search

The last scenario is even simpler than the previous one. It demonstrates how quickly you can implement a full-text search over your database with only one useful extension function provided by EasyQuery: `FullTextSearchQuery`.
The page is implemented as usual MVC action/view pair (`Controllers/CustomerController` and `Views/Customer/Customers.cshtml`). It does not contain any EasyQuery JavaScript at all. All the magic happens in the `Index` method of the controller's class: we just call our `FullTextSearchQuery` function over the `Customers` DB set there.
On the page, we also use `EqHighLightFor` HTML helper to highlight the found parts of the text inside the data table.

## Sample database

All of these demo projects work with some sample database. That database is created and initialized automatically at the first start. It may take some time (up to 1 minute) - so, please don't worry. Next time the app will be up and ready in a few seconds after launch.

The sample database is created in your SQL Express LocalDB instance by default. To change that you can modify the connection string in `web.config` file in the project's folder.

## Links

* [EasyQuery home page](https://korzh.com/easyquery)
* [EasyQuery documentation](https://korzh.com/easyquery/docs)
* [EasyQuery live demos](https://korzh.com/demo/easyquery-asp-net-core-razor)
