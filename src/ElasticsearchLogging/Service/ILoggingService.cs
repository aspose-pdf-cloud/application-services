using System.Threading.Tasks;

namespace Aspose.Cloud.Marketplace.Services
{
    public interface ILoggingService
    {
        Task ReportErrorLog(Model.Elasticsearch.ElasticsearchErrorDocument doc);
        Task ReportAccessLog(Model.Elasticsearch.ElasticsearchAccessLogDocument doc);
        Task ReportSetupLog(Model.Elasticsearch.ElasticsearchSetupDocument doc);
    }
}
