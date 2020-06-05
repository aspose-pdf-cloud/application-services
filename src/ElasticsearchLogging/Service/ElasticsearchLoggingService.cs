using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cloud.Marketplace.Services.Model.Elasticsearch;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("acm.ElasticsearchLogging.Tests")]
namespace Aspose.Cloud.Marketplace.Services
{
    /// <summary>
    /// Implements Elasticsearch logging
    /// supports 3 document types:
    /// - ElasticsearchErrorDocument
    /// - ElasticsearchAccessLogDocument
    /// - ElasticsearchSetupDocument
    /// </summary>
    public class ElasticsearchLoggingService : ILoggingService
    {
        internal ElasticsearchReporter errorLogReporter;
        internal ElasticsearchReporter accessLogReporter;
        internal ElasticsearchReporter setupLogReporter;
        internal ILogger<ElasticsearchLoggingService> _logger;
        public ElasticsearchLoggingService(ILogger<ElasticsearchLoggingService> logger
            , string[] elasticsearchUris, string errorIndexName, string accesslogIndexName
            , string setuplogIndexName
            , string apiId = null, string apiKey = null
            , string username = null, string password = null
            , int?timeoutSeconds = null, bool debug = false)
        {
            _logger = logger;
            errorLogReporter = accessLogReporter = setupLogReporter = null;
            if (null != elasticsearchUris && elasticsearchUris.Length > 0)
            {
                if (!string.IsNullOrEmpty(errorIndexName))
                    errorLogReporter = new ElasticsearchReporter(elasticsearchUris, errorIndexName, apiId: apiId, apiKey: apiKey, username:username, password:password, timeoutSeconds: timeoutSeconds, debug: debug);
                if (!string.IsNullOrEmpty(accesslogIndexName))
                    accessLogReporter = new ElasticsearchReporter(elasticsearchUris, accesslogIndexName, apiId: apiId, apiKey: apiKey, username: username, password: password, timeoutSeconds: timeoutSeconds, debug: debug);
                if (!string.IsNullOrEmpty(setuplogIndexName))
                    setupLogReporter = new ElasticsearchReporter(elasticsearchUris, setuplogIndexName, apiId: apiId, apiKey: apiKey, username: username, password: password, timeoutSeconds: timeoutSeconds, debug: debug);
            }
        }

        private static SemaphoreSlim maxSimultaneousReports;
        static ElasticsearchLoggingService()
        {
            maxSimultaneousReports = new SemaphoreSlim(500);
        }

        internal ElasticsearchLoggingService()
        {
        }
        /// <summary>
        /// Reports event using specific reporter
        /// For now we allow only 500 simultaneous reports. This should be changed (use queue or something)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reporter"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal async Task DoReport<T>(ElasticsearchReporter reporter, T doc) where T : class
        {
            //Stopwatch s = new Stopwatch(); s.Start();
            try
            {
                await maxSimultaneousReports.WaitAsync();
                if (null != reporter)
                    await reporter.Post(doc);
            }
            finally
            {
                //Trace.WriteLine($"\n\n=== EL elapsed {s.Elapsed}\n\n");
                maxSimultaneousReports.Release();
            }
        }
        /// <summary>
        /// Post Elasticsearch logs in the background
        /// </summary>
        /// <typeparam name="T">Model class</typeparam>
        /// <param name="reporter">reporter</param>
        /// <param name="doc">document</param>
        /// <returns></returns>
        internal Task Report<T>(ElasticsearchReporter reporter, T doc) where T : class
        {
            Task.Run(async () =>
            {
                await DoReport(reporter, doc);
            }).ContinueWith(t =>
                {
                    _logger.LogError(t.Exception, $"Exception occured during posting to index '{reporter.Index}'");
                },
                TaskContinuationOptions.OnlyOnFaulted);
            // We don't need to wait for the reporting task to complete
            // so just return completed task
            return Task.CompletedTask;
        }

        public async Task ReportErrorLog(ElasticsearchErrorDocument doc) => await Report(errorLogReporter, doc);
        public async Task ReportAccessLog(ElasticsearchAccessLogDocument doc) => await Report(accessLogReporter, doc);
        
        public async Task ReportSetupLog(ElasticsearchSetupDocument doc) => await Report(setupLogReporter, doc);
    }
}
