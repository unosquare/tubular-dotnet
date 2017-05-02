 [![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/tubular/)](https://github.com/igrigorik/ga-beacon)
 [![Build Status](https://travis-ci.org/unosquare/tubular-dotnet.svg?branch=master)](https://travis-ci.org/unosquare/tubular-dotnet)
 [![Build status](https://ci.appveyor.com/api/projects/status/ia9hnxea6b64xbhh?svg=true)](https://ci.appveyor.com/project/geoperez/tubular-dotnet)

![Tubular DotNet](http://unosquare.github.io/tubular/assets/tubular.png)

:star: *Please star this project if you find it useful!*

Tubular provides .NET Framework and .NET Core Library to create **REST service** to use with Tubular Angular Components easily with any WebApi library. 

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

## Samples

You can check out the <a href="http://unosquare.github.io/tubular" target="_blank">Tubular GitHub Page</a> to get a few examples. We still need to work on more samples and better documentation, but we feel what we have now will get you up to speed very quickly :).

The following HTML represents a basic grid. You don't need to add anything else to your controller! Everything you need is to create your markup.

```html
 <div class="container">
        <tb-grid server-url="/data/customers.json" page-size="20" 
                 class="row" grid-name="mygrid">
            <div class="col-md-12">
                <div class="panel panel-default panel-rounded">
                    <tb-grid-table class="table-bordered">
                        <tb-column-definitions>
                            <tb-column name="CustomerName">
                                <tb-column-header>
                                    <span>{{label}}</span>
                                </tb-column-header>
                            </tb-column>
                            <tb-column name="Invoices">
                                <tb-column-header>
                                    <span>{{label}}</span>
                                </tb-column-header>
                            </tb-column>
                        </tb-column-definitions>
                        <tb-row-set>
                            <tb-row-template ng-repeat="row in $component.rows" 
                                             row-model="row">
                                <tb-cell-template>
                                    {{row.CustomerName}}
                                </tb-cell-template>
                                <tb-cell-template>
                                    {{row.Invoices}}
                                </tb-cell-template>
                            </tb-row-template>
                        </tb-row-set>
                    </tb-grid-table>
                </div>
            </div>
        </tb-grid>
    </div>
```

Tubular works directly with either your own OData service or a custom RESTful call. You can simplify your RESTful API significantly by using our .NET Tubular.ServerSide library which handles `IQueryable` easily.

## Boilerplate

We have 3 boilerplates ready to seed your project:

* <a href="https://github.com/unosquare/tubular-boilerplate" target="_blank">Simple Boilerplate</a> without server-side. 
* [ASP.NET 4.6 Boilerplate](https://github.com/unosquare/tubular-boilerplate-csharp).
* [ASP.NET Core Boilerplate](https://github.com/unosquare/tubular-aspnet-core-boilerplate)
