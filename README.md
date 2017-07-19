 [![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/tubular/)](https://github.com/igrigorik/ga-beacon)
 [![Build Status](https://travis-ci.org/unosquare/tubular-dotnet.svg?branch=master)](https://travis-ci.org/unosquare/tubular-dotnet)
 [![Build status](https://ci.appveyor.com/api/projects/status/ia9hnxea6b64xbhh?svg=true)](https://ci.appveyor.com/project/geoperez/tubular-dotnet)
[![Coverage Status](https://coveralls.io/repos/github/unosquare/tubular-dotnet/badge.svg?branch=master)](https://coveralls.io/github/unosquare/tubular-dotnet?branch=master)

![Tubular DotNet](http://unosquare.github.io/tubular/assets/tubular.png)

:star: *Please star this project if you find it useful!*

Tubular provides .NET Framework and .NET Core Library to create **REST service** to use with Tubular Angular Components easily with any WebApi library (ASP.NET Web API for example).

Now you can use Javascript as backend using [Tubular Node.js module](https://github.com/unosquare/tubular-nodejs).

Please visit the <a href="http://unosquare.github.io/tubular" target="_blank">Tubular GitHub Page</a> to learn how quickly you can start coding. Don't forget to check out the Tubular Generator which quickly turns models into an awesome UIs!

## NuGet Installation [![NuGet version](https://badge.fury.io/nu/tubular.svg)](http://badge.fury.io/nu/tubular)

The same Nuget contains `.NET Framework 4.5.2` and `.NET Standard 1.6` targets.

<pre>
PM> Install-Package Tubular.ServerSide
</pre>

## Global Settings

You can access to global settings by using the static object `TubularDefaultSettings` and setup common behavior in Tubular. The settings included are:

<table>
    <tr><th>Setting</th><th>Default value</th><th>Notes</th></tr>
    <tr><th>AdjustTimezoneOffset</th><td><i>True</i></td><td>Determines if the DateTime from a Response should adjust the timezone offset send by within the Request.</td></tr>
</table>

## Building C# Library

Depending in your environment you must use the solution **Unosquare.Tubular.sln** if you want to build with `dotnet` or **Unosquare.Tubular.Lib.sln** if you use `msbuild` or `xbuild`.

In other words, use first solution file for VS2015 with .NET Core support or the second one for any other environment.

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

## Boilerplate

We have 3 boilerplates ready to seed your project:

* <a href="https://github.com/unosquare/tubular-boilerplate" target="_blank">Simple Boilerplate</a> without server-side. 
* [ASP.NET 4.6 Boilerplate](https://github.com/unosquare/tubular-boilerplate-csharp).
* [ASP.NET Core Boilerplate](https://github.com/unosquare/tubular-aspnet-core-boilerplate)
