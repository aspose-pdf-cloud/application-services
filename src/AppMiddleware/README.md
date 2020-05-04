# (C#) AppMiddleware library

This library provides different middlewares for your web application .

## Target Framework
netcoreapp3.1

## Purpose
Provide Web Applications with easy to use middleware classes

## Classes list

* **ElasticsearchAccessLogMiddleware** (*ElasticsearchAccessLogMiddleware.cs*) - handle access logs publishing to Elasticsearch
* **BaseExceptionHandlingMiddleware** (*BaseExceptionHandlingMiddleware.cs*) - base class for error logging
* **ElasticsearchExceptionHandlingMiddleware** (*ElasticsearchExceptionHandlingMiddleware.cs*) - handle error logs publishing to Elasticsearch
* **StoreExceptionHandlingMiddleware** (*StoreExceptionHandlingMiddleware.cs*) - handle error logs publishing to Elasticsearch witout user sensitive data. User sensitive data should be stored by application
* **IAppClient** (*IAppClient.cs*) - base application interface for Elasticsearch reporting. Web application shgould implement this interface to enable Access log and Error log publishing
* **IAppCustomErrorReportingClient** (*IAppClient.cs*) - Application has to implement this interface in order to use **StoreExceptionHandlingMiddleware**

## IAppClient interface
Web application should implement this basic interface to be able to use logging classes
* **RequestId** - request identifier
* **Stat** - array of statistical information data. Handy for profiling
* **ElapsedSeconds** - elapsed seconds since request execution startup
* **ValueTuple<int, string, string, byte[]> ErrorResponseInfo(Exception ex)** - returns tuple that describe exception occured  
*ErrorResponseInfo* should return Tuple(*Http error code*, *error text*, *error description*, *binary sensitive data to be logged*)

## IAppCustomErrorReportingClient interface
Allows Exceptions with sensitive data to be logged through application provided *ReportException()* function. 

## ElasticsearchAccessLogMiddleware
Automatically logs to Elasticsearch every request for your Web Application.  
Currently it logs:

* Request ID
* Query parameters
* Form parameters
* Request Headers
* Path
* Controller and Action name
* Result Code
* Statistics
* Elapsed Seconds
* ...

## ElasticsearchAccessLogMiddleware integration
in your `Configure()` method in `Startup.cs`
```c#
app.UseMiddleware<ConholdateCloud.App.Middleware.ElasticsearchAccessLogMiddleware<AppCli>>();
```
Where  
`AppCli` - your interface/class derived from `IAppClient`

## ElasticsearchExceptionHandlingMiddleware
Automatically logs to Elasticsearch every exception
Currently it logs:

* Request ID
* Query parameters
* Form parameters
* Exception Info
* Path
* Controller and Action name
* Result Code
* Elapsed Seconds
* ......

ErrorLog middleware automatically returns error code, error description to the user

## ElasticsearchExceptionHandlingMiddleware  initialization
in your `Configure()` method in `Startup.cs`
```C#
app.UseMiddleware< ConholdateCloud.App.Middleware.ElasticsearchExceptionHandlingMiddleware<AppCli>>();
```
Where  
`AppCli` - your interface/class derived from `IAppClient`

## StoreExceptionHandlingMiddleware
Basically it logs the same as *ElasticsearchExceptionHandlingMiddleware* but without user sensitive information

## ElasticsearchExceptionHandlingMiddleware  initialization
in your `Configure()` method in `Startup.cs`
```C#
app.UseMiddleware< ConholdateCloud.App.Middleware.StoreExceptionHandlingMiddleware<AppCli>>();
```
Where  
`AppCli` - your interface/class derived from `IAppCustomErrorReportingClient`