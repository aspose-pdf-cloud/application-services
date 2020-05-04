# (C#) ConfigurationExpression library

This library provides simple way to write dynamic expressions in configuration.

## Target Framework

netcoreapp3.1

## Purpose

Provide Web Applications with simple way to write dynamic expressions in application configuration that available through *IConfiguration* interface.

## Classes list

* **IConfigurationExpression** (*IConfigurationExpression.cs*) - Interface.
* **ConfigurationExpression** (*ConfigurationExpression.cs*) - Provides dynamic expression evaluation

## Integration
Just add
```
services.AddSingleton<IConfigurationExpression, ConfigurationExpression>();
```
in your *ConfigureServices()* function in *Startup.cs*
Instance of *ConfigureServices()* will be available to all your controllers through DI

## Usage

suppose you obtained *configExpression* through DI or `provider.GetRequiredService<IConfigurationExpression>()`
and have `"Sample": "sample-{DateTime.Now.ToString(\"yyyy.MM.dd\")}"` in your `appSettings.json`
```
configExpression.Get("Sample");
```
will result in `"sample-CURRENT_DATE"`

