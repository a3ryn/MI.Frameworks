# Shared.Core.Common Package

## Description

The `Shared.Core.Common` class library contains a few useful utilities (extension methods) and simple data types, that may be used in various non-specific contexts, from reflection to simple dependency injection, from modeling and reading configuration settings - for both .NET Framework as well as .NET Core style configurations, to serialization and deserialization - for both XML as well as JSON.

Additionally, this library also exposes a few very basic contracts (interfaces) for general cross-cutting concerns, such as logging, (SQL Server) data access, and caching. They are implemented by other projects in this solution, but other custom implementations may be injected, if these exposed contracts are sufficient to address the cross-cutting concerns of the solution at hand.

As of 1.2.0, these utilities are supporting .NET Framework (4.7.2), .NET Standard (2.0), and .NET Core (3.1).

## Dependencies

The only external/3rd party dependency that this library takes is on `Newtonsoft.Json`, regarding the serialization/deserialization extension methods.

Given the data access interface exposed, this library also employs `Microsoft.Data.SqlClient`. 

## Usage Samples & Unit Tests

How to use some of the utilities and features exposed by this library and the other frameworks in this solution, can be found under the `Candea.Frameworks.Tests` project.

## _Feature_: Dependency Injection/Service Location

Striving to provide a very simple mechanism for DI, a tiny framework was built around MEF, allowing any component to _discover_ implementations to any custom interface, with the ability to distinguish between different implementation by supplying a contract name for unique identification of the implementation class. This relies on `System.ComponentModel.Composition`.

### Configuration

By default, the DI framework will probe the directory where the current assembly is executing, and all present DLLs will be searched for implementation of the requested interface. In case of web applications and to support probing other directories for concretions, both JSON-based as well as .NET framework style XML AppSettings configuration can direct the framework to look into a specific directory. Also, search patterns (regarding the DLL names) can be specified in the form of a comma-separated string. Here are a few examples:

* Using the custom `mefSettings.json` configuration file, that comes with the NuGet package (and which is copied to the output directory during build, if newer): 

    ````JSON
    {
      "mef": {
        "assembliesPath": ".",
        "csvSearchPatterns": "*"
      }
    }
    ````

* Including the JSON configuration details into the appsettings.json file (for .NET Core applications); the content shown above would be included as one of the top KVPs into the JSON configuration file:

     ````json
    {
      "mef": {
        "assembliesPath": "/bin",
        "csvSearchPatterns": "myLibs.*,myOtherLibs.Some.*"
      }
    }
    ````


* Using XML-based configuration (under `<appSettings>`):

    ````xml
    <appSettings>
        <!-- other KVPs -->
        <add key="MEF.AssembliesPath" value="." />
        <add key="MEF.CsvSearchPatterns" value="Shared.*"/>
    </appSettings>
    ````


### Usage Sample

* Single implementation of some interface:

    ````csharp
    using static Shared.Core.Common.DI.Mef;

    //...
    var provider = Resolve<IMyCustomProvider>();
    ````

* Named implementation:

    ````csharp
    using static Shared.Core.Common.DI.Mef;

    //...
    var provider = Resolve<IMyCustomProvider>("prov1");
    ````

    where the implementation class was decorated with the `ExportAttribute` like this:

    ````csharp
    using System.ComponentModel.Composition;

    namespace Abc
    {
        [Export("prov1", typeof(IMyCustomProvider))]
        public class MyCustomProvider
        {
            //...
        }
    }
    ````

Additional samples can be found under the unit test class [`MefConfigTests`](../Candea.Frameworks.Tests/MefConfigTests.cs).

## _Feature & Models_: Configuration Settings

Configuration KVPs are modeled using the data types introduced under the `Config` folder. 

The data type [`AppSettings`](Config/AppSettings.cs) provides the ability to load such KVPs in three different ways (depending how they are stored) - via static methods exposed on the type itself. An instance of it can also be initialized by poassing the collection of custom [`IAppSetting`](Config/IAppSetting.cs) instances. 

Settings can be read from a custom JSON file, or the default appsettings.json (with .NET Core), but it also supports reading KVPs from the XML `app.config` configuration file (.NET Framework) from the `<appSettings>` section (if no JSON configuration is found/provided).
 
Moreover, a custom configuration object can be populate with settings read from a JSON file, by calling the generic `FromFile<T>` static method.

Usage samples can be found in this unit test: [`ConfigTests`](../Candea.Frameworks.Tests/ConfigTests.cs).

## _Contracts_: Extensibility Points

This class library also exposes a few very basic contracts for cross-cutting concerns, such as caching, logging, and data access.

### Caching

See [`ICache` interface](Caching/ICache.cs).

The project/package `Shared.Frameworks.Caching` provides an implementation for this contract using `System.Runtime.Caching`. Other implementations can be supplied, if so desired, if this interface is to be used.

See the unit test class [`CachingTests`](../Candea.Frameworks.Tests/CachingTests.cs) for samples of how to configure the caching options and how to use the framework.

### Logging

See [`ILogger`](Logging/ILogger.cs) and [`ILogManager`](Logging/ILogManager.cs) interfaces, as well as the logging resolution utility [`LogResolver`](Logging/LogResolver.cs). The utility helps identify implementations of the logging contracts (using MEF) and instantiates/disposes the concretions found.

The project/package `Shared.Frameworks.Logging.Log4Net` provides an implementation of these contracts using Apache's `log4net`. Other implementations may be provided as desired, if used.

See the unit test class [`LoggingTests`](../Candea.Frameworks.Tests/LoggingTests.cs) for samples of how to configure the logging options and how to use the framework.

### SQL Server Data Access (ADO.NET-based)

See [`IDataAccess`](DataAccess/IDataAccess.cs). It supports querying a SQL Server database directly (query-as-string), calling custom TVFs and populating custom C# models, calling stored procedures with input parameters, simple or UDTTs, as well as simple output parameters.

The project/package `Shared.Frameworks.DataAccess` provides an implementation using straightforward ADO.NET and some mapping facilities to custom data types (automatic mapping or custom translations can be provided as input to the exposed data access methods).

Configuration detail, such as connection string and SQL command timeout, can be specified in a custom JSON file, part of the appsettings.json or as appSettings in an XML configuration file. Examples are provided under the unit test project - which also deploys and uses a very simple custom SQL Server DB (see the local publishing profile included with the DB project).

See [`DataAccessTests`](../Candea.Frameworks.Tests/DataAccessTests.cs) for examples on how to configure and use the framework, for all supported DB calls (direct query, TVF call, stored procedure calls with various parameters).

Additionally, the framework package also implements the 'IDataProxiesGenerator` contract, automating the creation of POCOs that map to tables or views. 


