# (C#) ElasticsearchLogging Service for Web Applications

This library provides functionality to push documents to Elasticsearch.

## Target Framework
netstandart2.0

## Purpose
Provide Web Applications with easy to use and extendable methods to add Elasticsearch logging features. Library is based on [Nest](https://github.com/elastic/elasticsearch-net) official Elasticsearch client library

## Classes list

* **ElasticsearchBaseDocument** (*Model/BaseDocument.cs*) - Base document.
* **ElasticsearchAccessLogDocument** (*Model/AccessLogDocument.cs*) - Access log document.
* **ElasticsearchErrorInfoDocument** (*Model/ErrorLogDocument.cs*) - Error log document.
* **ElasticsearchReporter** (*ElasticsearchReporter.cs*) - Implements Elasticsearch reporting functions
* **ILoggingService** (*Service/ILoggingService.cs*) - Interface for *ElasticsearchLoggingService*
* **ElasticsearchLoggingService** (*Service/ElasticsearchLoggingService.cs*) -Implements Elasticsearch reporting service reado for integration with Web API projects

Becasue [Nest](https://github.com/elastic/elasticsearch-net)  does not use interfaces at all we had to develop several additional classes to let us use [Nest](https://github.com/elastic/elasticsearch-net)  using interfaces
* All Nest cleint interfaces are implemented in *ElasticsearchInterface.cs*
* Stubs for Nest library are implemented in *ElasticsearchMock.cs*

## Integration
* Add dependency to `ElasticsearchLogging`, `AppCommon`,`ConfigurationExpression` libraries
  NOTE:
  If you don't need `ConfigurationExpression` you can omit library dependence.   `ConfigurationExpression` allows you to use dynamic expressions in application configuration
* Add scoped service in your `ConfigureServices()` method from `Startup.cs` 
```c#
services.AddScoped<ILoggingService>(provider => {
    var configuration = provider.GetRequiredService<IConfiguration>();
    var configExpression = provider.GetRequiredService<IConfigurationExpression>();
    var hostEnvironment = provider.GetRequiredService<IWebHostEnvironment>();
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

    return new ElasticsearchLoggingService(loggerFactory.CreateLogger<ElasticsearchLoggingService>()
        , Configuration.GetSection("Elasticsearch:Uris")?.Get<string[]>()
        , configExpression.Get("Elasticsearch:ErrorlogIndex", "errorlog-{DateTime.Now.ToString(\"yyyy.MM.dd\")")
        , configExpression.Get("Elasticsearch:AccesslogIndex", "accesslog-{DateTime.Now.ToString(\"yyyy.MM.dd\")")
        , setuplogIndexName: configExpression.Get("Elasticsearch:SetuplogIndex", "setuplog-{DateTime.Now.ToString(\"yyyy.MM.dd\")")
        , apiId: configExpression.Get("Elasticsearch:apiId"), apiKey: configExpression.Get("Elasticsearch:apiKey")
        , timeoutSeconds: 5
        , debug: hostEnvironment.IsDevelopment());
});
```
* You also need to specify proper configuration for ElasticsearchLogging Service
  AppSettings.json style:
  
  ```json
  ..............
  "Elasticsearch": {
    "Uris": ["http://IP:PORT"],
    "apiId": "<ELASTICSEARCH_API_ID>",
    "apiKey": "<ELASTICSEARCH_API_KEY>",
    "ErrorlogIndex": "errorlog-MYAPP-{DateTime.Now.ToString(\"yyyy.MM.dd\")}",
    "AccesslogIndex": "accesslog-MYAPP-{DateTime.Now.ToString(\"yyyy.MM.dd\")}"
    "SetuplogIndex": "setuplog-MYAPP-{DateTime.Now.ToString(\"yyyy.MM.dd\")}"
  }
  ..............
  ```

* You will be able to use `ILoggingService` interface through DI

To automate access and report logging please refer to [AppMiddleware](../AppMiddleware/README.md) project