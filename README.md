[![NuGet](https://img.shields.io/nuget/dt/Tubular.ServerSide.svg)](https://www.nuget.org/packages/Tubular.ServerSide/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=unosquare_tubular-dotnet&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=unosquare_tubular-dotnet)
[![NuGet version](https://badge.fury.io/nu/Tubular.ServerSide.svg)](https://badge.fury.io/nu/Tubular.ServerSide)

![Tubular DotNet](http://unosquare.github.io/tubular-angular/assets/tubular.png)

Tubular provides .NET Framework and .NET Core Library to create **REST service** to use with Tubular Angular Components easily with any WebApi library (ASP.NET Web API for example).

Please visit the [Tubular GitHub Page](http://unosquare.github.io/tubular) to learn how quickly you can start coding. See [Related projects](#related-projects) below to discover more Tubular libraries and backend solutions.

## Installation

```
PM> Install-Package Tubular.ServerSide
```

## Sample

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
