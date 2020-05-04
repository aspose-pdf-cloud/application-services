using Aspose.Cloud.Marketplace.Common;
using Aspose.Cloud.Marketplace.Services.Model.Elasticsearch;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Aspose.Cloud.Marketplace.Services;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using IElasticClient = Aspose.Cloud.Marketplace.Services.ElasticsearchInterface.IElasticClient;

namespace Aspose.Cloud.Marketplace.App.ElasticsearchLogging.Tests
{
    [Trait("ElasticsearchLogging", "ElasticsearchLoggingService_Tests")]
    public class ElasticsearchLoggingService_Tests
    {
        internal ElasticsearchLoggingService service;
        internal Mock<IElasticClient> accessLogCliMock, errorLogCliMock, setupLogCliMock;
        public ElasticsearchLoggingService_Tests()
        {
            service = new ElasticsearchLoggingService();
            accessLogCliMock = ElasticsearchReporter_Tests.CreateCli<ElasticsearchAccessLogDocument>();
            errorLogCliMock = ElasticsearchReporter_Tests.CreateCli<ElasticsearchErrorDocument>();
            setupLogCliMock = ElasticsearchReporter_Tests.CreateCli<ElasticsearchSetupDocument>();
            service.accessLogReporter = new ElasticsearchReporter(accessLogCliMock.Object);
            service.errorLogReporter = new ElasticsearchReporter(errorLogCliMock.Object);
            service.setupLogReporter = new ElasticsearchReporter(setupLogCliMock.Object);
        }
        
        [Fact]
        public async void ReportAccessLog_Test()
        {
            DateTime dt = DateTime.Now;
            await service.ReportAccessLog(new ElasticsearchAccessLogDocument("mockid", appName: "mock_app_name", message: "msg"));
            await Task.Delay(500); // we hope that background task finishes before

            accessLogCliMock.Verify(e => e.IndexAsync(It.Is<ElasticsearchAccessLogDocument>(d =>
                    d.LogName == "access_log" && d.Timestamp < dt.AddSeconds(1) && d.Timestamp > dt.AddSeconds(-1)
                    && d.Id == "mockid" && d.AppName == "mock_app_name" && d.Message == "msg"
                )
                , It.IsAny<Func<IndexDescriptor<ElasticsearchAccessLogDocument>, IIndexRequest<ElasticsearchAccessLogDocument>>>()
                , It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void ReportErrorLog_Test()
        {
            DateTime dt = DateTime.Now;
            await service.ReportErrorLog(new ElasticsearchErrorDocument("mockid", ex: new Exception("mockerror", new Exception("mockinner")), appName: "mock_app_name"));
            await Task.Delay(500); // we hope that background task finishes before
            
            errorLogCliMock.Verify(e => e.IndexAsync(It.Is<ElasticsearchErrorDocument>(d =>
                    d.LogName == "error_log" && d.Timestamp <= dt.AddSeconds(1) && d.Timestamp > dt.AddSeconds(-1)
                    && d.Id == "mockid" && d.AppName == "mock_app_name" && d.Error.ErrorText == "mockerror" && d.Error.InnerError.ErrorText == "mockinner"
                )
                , It.IsAny<Func<IndexDescriptor<ElasticsearchErrorDocument>, IIndexRequest<ElasticsearchErrorDocument>>>()
                , It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void ReportSetupLog_Test()
        {
            DateTime dt = DateTime.Now;
            await service.ReportSetupLog(new ElasticsearchSetupDocument("mockid", action:"installed", appName: "mock_app_name"));
            await Task.Delay(500); // we hope that background task finishes before

            setupLogCliMock.Verify(e => e.IndexAsync(It.Is<ElasticsearchSetupDocument>(d =>
                    d.LogName == "setup_log" && d.Timestamp < dt.AddSeconds(1) && d.Timestamp > dt.AddSeconds(-1)
                    && d.Id == "mockid" && d.AppName == "mock_app_name" && d.Action == "installed"
                )
                , It.IsAny<Func<IndexDescriptor<ElasticsearchSetupDocument>, IIndexRequest<ElasticsearchSetupDocument>>>()
                , It.IsAny<CancellationToken>()));
        }
    }
}
