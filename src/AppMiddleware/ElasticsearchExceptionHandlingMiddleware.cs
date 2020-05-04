using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Aspose.Cloud.Marketplace.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Aspose.Cloud.Marketplace.App.Middleware
{
    /// <summary>
    /// Logs error events to the specific ILoggingService
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ElasticsearchExceptionHandlingMiddleware<T> : BaseExceptionHandlingMiddleware<T>  where T : IAppClient
    {
        public ElasticsearchExceptionHandlingMiddleware(RequestDelegate next, ILogger<ElasticsearchExceptionHandlingMiddleware<T>> logger
            , IServiceProvider serviceProvider, IOptions<LogMiddlewareOptions> options) : base(next, logger, serviceProvider, options)
        {
        }
        protected override async Task<string> ReportError(T appCli, Services.Model.Elasticsearch.ElasticsearchErrorDocument doc, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var remoteLogger = scope.ServiceProvider
                    .GetRequiredService<ILoggingService>();
                await remoteLogger.ReportErrorLog(doc);
                return null;
            }
        }
    }
}
