using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aspose.Cloud.Marketplace.App.Middleware
{
    /// <summary>
    /// Model class represents error result
    /// </summary>
    public class ErrorInfo
    {
        public string request_id { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public string error_result { get; set; }
    }
    /// <summary>
    /// Base class for Exception handling
    /// </summary>
    /// <typeparam name="T">Application client</typeparam>
    public abstract class BaseExceptionHandlingMiddleware<T> where T : IAppClient
    {

        protected readonly RequestDelegate _next;
        protected readonly ILogger<BaseExceptionHandlingMiddleware<T>> _logger;
        protected readonly IServiceProvider _serviceProvider;
        private readonly LogMiddlewareOptions _options;
        public BaseExceptionHandlingMiddleware(RequestDelegate next, ILogger<BaseExceptionHandlingMiddleware<T>> logger, IServiceProvider serviceProvider, IOptions<LogMiddlewareOptions> options)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options?.Value;
        }

        protected abstract Task<string> ReportError(T appCli, Services.Model.Elasticsearch.ElasticsearchErrorDocument doc, IServiceProvider serviceProvider);

        /// <summary>
        /// Report an Error
        /// </summary>
        /// <param name="appCli"></param>
        /// <param name="exc"></param>
        /// <param name="context"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="reporter"></param>
        /// <param name="logger"></param>
        /// <returns>Error data</returns>
        public static async Task<Tuple<int, ErrorInfo>> HandleError(T appCli, Exception exc, HttpContext context, IServiceProvider serviceProvider
            , Func<T, Services.Model.Elasticsearch.ElasticsearchErrorDocument, IServiceProvider, Task<string>> reporter
            , ILogger<BaseExceptionHandlingMiddleware<T>> logger
            )
        {
            string errorResultUrl = "";
            var (responseStatusCode, error, errorDescription, customData) = appCli.ErrorResponseInfo(exc);
            //Post error to the logging service
            try
            {
                var (controller, action, parameters) = Utils.GetRequestParameters(context);
                logger.LogError(exc, $"Error occured in '{controller}', action '{action}'");
                errorResultUrl = await reporter(appCli, new Services.Model.Elasticsearch.ElasticsearchErrorDocument(
                    id: appCli.RequestId, ex: exc, appName: appCli.AppName, message: exc.Message, path: context.Request.Path
                    , controllerName: controller, actionName: action
                    , parameters: parameters, elapsedSeconds: appCli.ElapsedSeconds, resultCode: responseStatusCode, inputData: customData
                ), serviceProvider);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, "Error communicating with remote logging");
                else throw;
            }

            return new Tuple<int, ErrorInfo>(responseStatusCode, new ErrorInfo()
            {
                request_id = appCli.RequestId,
                error = error,
                error_description = errorDescription,
                error_result = errorResultUrl
            });
        }

        public async Task Invoke(HttpContext context, T appCli)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exc)
            {
                var (responseStatusCode, errorInfo) = await HandleError(appCli, exc, context, _serviceProvider, ReportError, _logger);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = responseStatusCode;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorInfo));
            }
        }
    }
}
