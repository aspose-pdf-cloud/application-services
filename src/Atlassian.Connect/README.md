# (C#) Atlassian.Connect library

Provides necessary functions to perform Rest API queries to Jira Cloud instances

## Target Framework

netstandart2.1

## Purpose

Provide easy way to perform Rest API requests to Jira Cloud instances.

## Classes list
* **LoggingHandler** (*LoggingHandler.cs*) - Allows requests/responses logging in HttpClient
* **QueryStringHasher** (*QueryStringHasher.cs*) - Performs URL hashing for Jira Cloud RestApi calls
* **RequestValidation** (*RequestValidation.cs*) - Validates request from jira cloud instance protected by jwt token
* **RestApiClient** (*RestApiClient.cs*) - Rest client for Jira cloud instances


## Examples
### Calculate URL hash
```C#
string hash = QueryStringHasher.CalculateHash("GET", "a/b/c", "a=b")
```
### Using RestApiClient
```C#
HttpClient httpCli = new HttpClient();
var api = new RestApiClient("issuer", "123secret321", "subject", client);
string response = await cli.Get("/rest/api/3/field");
```
### Using LoggingHandler to dump request/responses
```C#
HttpClient cli = new HttpClient(new LoggingHandler(dump: true));
```
### Using RequestValidation to validate token
```C#
var requestValidation = new RequestValidation(Request);
if (requestValidation.TokenExists)
{
    var registrationData = <Get client registration data from external source>;
    requestValidation.Validate(registrationData);
}
else throw new Exception("No token");
```
