# EasyQuery WebForms demo

This project demostrates how to use EasyQuery library (together with EasyQuery.JS scripts) in a WebForms application.

## Installation

Before the first launch it's necessary to update all NuGet packages in this project. 

There are two possible ways to do it:

### Using NuGet Package Manager

1. In Solution Explorer make a right click on project and choose "Manage NuGet packages..." menu item.

2. On window that appear, open "Updates" tab, select all packages in the list and click on "Update all" button

### Using Package Manager Console

1. In the Visual Studio select Tools | NuGet Package Manager | Package Manager Console menu item

2. In the console first check that the this project is selected in the "Default project" selector.
After that just enter the following command:

```ps
Update-Package -reinstall
```

Either of described above method will install all necessary packages and scripts.

After that just run the project and click on "Open EasyQuery demo" button to enter the advanced search page powered by EasyQuery components.

## How it works

### 1. The demo page

`EasyQuery.aspx` page is not actually a classic WebForms page.
It's just an HTML page which define several slot (empty `<div>` elements) for EasyQuery widgets and attach EasyQuery CSS file and scripts.
On the page start the script initialize EasyQuery's `view` object which do all the job.

### 2. Web API Controller

We use WebAPI packages to create an API controller which handles all AJAX requests from the client-side widgets.
You can it in `Controllers/AdvancedSearchController.cs` file.

