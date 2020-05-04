using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Aspose.Cloud.Marketplace.Services;
using Aspose.Cloud.Marketplace.Services.Model.Elasticsearch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aspose.Cloud.Marketplace.App.Middleware
{
    /// <summary>
    /// Logs access events to the specific ILoggingService.
    /// Before logging clear any sensitive data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoreExceptionHandlingMiddleware<T> : BaseExceptionHandlingMiddleware<T> where T : IAppCustomErrorReportingClient
    {
        public StoreExceptionHandlingMiddleware(RequestDelegate next, ILogger<StoreExceptionHandlingMiddleware<T>> logger, IServiceProvider serviceProvider
            , IOptions<LogMiddlewareOptions> options) 
            : base(next, logger, serviceProvider, options)
        {
        }

        public static async Task<string> HandleReportError(T appCli, ElasticsearchErrorDocument doc,
            IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var remoteLogger = scope.ServiceProvider
                    .GetRequiredService<ILoggingService>();

                if (null != remoteLogger)
                {
                    ElasticsearchErrorDocument elasticDoc = new ElasticsearchErrorDocument();
                    doc.CloneTo(elasticDoc);
                    elasticDoc.InputData = null;
                    await remoteLogger.ReportErrorLog(elasticDoc);
                }
            }

            return await appCli.ReportException(doc, serviceProvider);
        }
        protected override async Task<string> ReportError(T appCli, ElasticsearchErrorDocument doc, IServiceProvider serviceProvider) =>
            await HandleReportError(appCli, doc, serviceProvider);
    }
}
