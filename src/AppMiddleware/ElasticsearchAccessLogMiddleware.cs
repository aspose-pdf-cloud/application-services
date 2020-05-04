using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Cloud.Marketplace.Services;
using Microsoft.Extensions.Options;

namespace Aspose.Cloud.Marketplace.App.Middleware
{
    /// <summary>
    /// Logs access events to the specific ILoggingService
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ElasticsearchAccessLogMiddleware<T> where T : IAppClient
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ElasticsearchAccessLogMiddleware<T>> _logger;
        private readonly LogMiddlewareOptions _options;
        public ElasticsearchAccessLogMiddleware(RequestDelegate next, ILogger<ElasticsearchAccessLogMiddleware<T>> logger, IOptions<LogMiddlewareOptions> options)
        {
            _next = next;
            _logger = logger;
            _options = options?.Value;
        }

        public async Task Invoke(HttpContext context, ILoggingService remoteLogger, T appCli)
        {
            await _next(context);
            try
            {
                var (controller, action, parameters) = Utils.GetRequestParameters(context);

                await remoteLogger.ReportAccessLog(new Services.Model.Elasticsearch.ElasticsearchAccessLogDocument(
                    id: appCli.RequestId, appName: appCli.AppName, message: null, path: context.Request.Path
                    , controllerName: controller, actionName: action
                    , elapsedSeconds: appCli?.ElapsedSeconds, parameters: parameters
                    , resultCode: context.Response.StatusCode, stat: appCli.Stat, 
                    headers: context?.Request?.Headers?
                        .Where(kv => !kv.Key.Equals("Authorization", StringComparison.InvariantCultureIgnoreCase)) // strip Authorization header
                        .ToDictionary(h => h.Key, h => string.Join(";", h.Value))
                    ));
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError(ex, "Error communicating with remote logging");
                else throw;
            }
        }
    }
}
