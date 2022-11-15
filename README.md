[![NuGet](https://img.shields.io/nuget/dt/Tubular.ServerSide.svg)](https://www.nuget.org/packages/Tubular.ServerSide/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=unosquare_tubular-dotnet&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=unosquare_tubular-dotnet)
![Tubular DotNet](http://unosquare.github.io/tubular-angular/assets/tubular.png)

Tubular provides .NET Framework and .NET Core Library to create **REST service** to use with Tubular Angular Components easily with any WebApi library (ASP.NET Web API for example).

Please visit the [Tubular GitHub Page](http://unosquare.github.io/tubular) to learn how quickly you can start coding. See [Related projects](#related-projects) below to discover more Tubular libraries and backend solutions.

## Installation [![NuGet version](https://badge.fury.io/nu/Tubular.ServerSide.svg)](https://badge.fury.io/nu/Tubular.ServerSide)

```
PM> Install-Package Tubular.ServerSide
```

## Sample

You can check out the <a href="http://unosquare.github.io/tubular" target="_blank">Tubular GitHub Page</a> to get a few examples. We still need to work on more samples and better documentation, but we feel what we have now will get you up to speed very quickly :).

The following snippet shows how to use Tubular Helper to create a basic response grid using Entity Framework and ASP.NET Web API.

```csharp
 [RoutePrefix("api/users")]
 public class UsersController : ApiController
 {
     [HttpPost, Route("paged")]
     public IHttpActionResult GridData([FromBody] GridDataRequest request)
     {
         using (var context = new SampleDbContext(false)) {
             return Ok(request.CreateGridDataResponse(context.SystemUsers));
         }
     }
 }
```

## Related Projects

Name | Type | Language/tech | Description
-----|------|---------------|--------------
| [Tubular for AngularJS (formerly Tubular)](https://github.com/unosquare/tubular) | Library | AngularJs | Tubular provides a set of directives and services using AngularJS as framework. |
| [Tubular for Angular6 (formerly Tubular2)](https://github.com/unosquare/tubular2) | Library | Angular6 | New Tubular2 with Angular6 (Angular2) and Angular Material 2.
| [Tubular React](https://github.com/unosquare/tubular-react) | Library | React | Tubular-React is a DataGrid component using Material-UI |
| [Tubular Common](https://github.com/unosquare/tubular-common) | Library | Javascript/Typescript | Tubular Common provides TypeScript and Javascript models and data transformer to use any Tubular DataGrid component with an array of Javascript objects. |
| [Tubular Dotnet](https://github.com/unosquare/tubular-dotnet) | Backend library | C#/.NET Core | Tubular provides .NET Framework and .NET Core Library to create REST service to use with Tubular Angular Components easily with any WebApi library (ASP.NET Web API for example). |
| [Tubular Nodejs](https://github.com/unosquare/tubular-nodejs) | Backend Library | Javascript | Tubular Node.js provides an easy way to integrate Tubular Angular Components easily with any Node.js WebApi library. |
| [Tubular Boilerplate C#](https://github.com/unosquare/tubular-boilerplate-csharp) | Boilerplate | C# | Tubular Directives Boilerplate (includes AngularJS and Bootstrap) |
| [Tubular Boilerplate](https://github.com/unosquare/tubular-boilerplate) | Boilerplate | Javascript/AngularJS | Tubular Directives Boilerplate (includes AngularJS and Bootstrap). |
