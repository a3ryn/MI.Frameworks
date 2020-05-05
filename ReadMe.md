# SharedLibs Solution

## Description

This solution consists of several class libraries that provide a collection of utilities and implement a few basic cross-cutting concerns, that are easy to configure and use.
A unit test project, which uses a database project for testing the data access framework, are also included. The unit tests provide code samples around the configuration and usage of the various features and frameworks exposed here.

As of 1.2.0, these utilities are supporting .NET Framework (4.7.2), .NET Standard (2.0), and .NET Core (3.1).

## Dependencies

The external/3rd party dependencies that this solution takes are: 
* `Newtonsoft.Json`, for the serialization/deserialization extension methods (referenced from the top `Shared.Core.Common` package)
* `log4net`, referenced exclusively from the logging implementation package

## Usage Samples & Unit Tests

How to use some of the utilities and features exposed by this library and the other frameworks in this solution, can be found under the `Candea.Frameworks.Tests` project.

## Implemented Features

### Dependency Injection/Service Location

This feature is implemented under the  `Shared.Core.Common` package, in ['Shared.Core.Common.DI.Mef'](Shared.Core.Common/DI/Mef.cs), as a very simple DI (or rather Service Location) framework around MEF, to discover at runtime implementations of custom interfaces. More details can be found in the [`ReadMe`](Shared.Core.Common/ReadMe.md) file of the `Shared.Core.Common` project.

### Data Access (targeting SQL Server and using ADO.NET)

The project/package `Shared.Frameworks.DataAccess` implements the [`Shared.Core.Common.DataAccess.IDataAccess`](Shared.Core.Common/DataAccess/IDataAccess.cs) contract exposed from the main `Shared.Core.Common` project. It is a very simple collection of methods that support data querying (explicit or via TVF calls) as well as stored procedure calls (with simple parameters, including output ones, as well as using UDTT inputs).

Additionally, this framework also supports some very limited code generation to produce simple POCOs/proxies for tables and views found in the underlying SQL Server database. It is the implementation of the [`Shared.Core.Common.DataAccess.IDataProxiesGenerator`](Shared.Core.Common/DataAccess/IDataProxiesGenerator.cs) contract from `Shared.Core.Common`. These proxies can be used instead of manually coding the POCOs for data querying supported by this framework.

### In-Memory Caching

The project/package `Shared.Frameworks.Caching` implements the [`Shared.Core.Common.Caching.ICache`](Shared.Core.Common/Caching/ICache.cs) contract exposed from the main `Shared.Core.Common` project. This is implemented using `System.Runtime.Caching`.

### Logging using Log4Net

This feature is implemented under the `Shared.Frameworks.Logging.Log4Net` project. It implementes the [`Shared.Core.Common.Logging.Ilogger`](Shared.Core.Common/DataAccess/IDataAccess.cs) and [`Shared.Core.Common.Logging.ILogManager`](Shared.Core.Common/Logging/ILogManager.cs) contracts exposed from the main `Shared.Core.Common` project.

### Simple Reusable Data Types

See data types exposed in `Shared.Core.Common` for modeling application settings/configuration (under `Config` folder).
See other data types exposed in `Shared.Core.Common` under `CustomTypes` folder.

### Other Utilities

* XML and JSON serialization and deserialization;
* Functional utilities;
* Extension methods to deal with enumerations, to use with reflection, and that implements the method template pattern.