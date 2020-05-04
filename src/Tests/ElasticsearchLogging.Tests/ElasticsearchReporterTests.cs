using Aspose.Cloud.Marketplace.Common;
using Aspose.Cloud.Marketplace.Services.Model.Elasticsearch;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Aspose.Cloud.Marketplace.Services;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Cloud.Marketplace.Services.ElasticsearchInterface;
using Nest;
using IElasticClient = Aspose.Cloud.Marketplace.Services.ElasticsearchInterface.IElasticClient;

namespace Aspose.Cloud.Marketplace.App.ElasticsearchLogging.Tests
{
    [Trait("ElasticsearchLogging", "ElasticsearchReporter_Tests")]
    public class ElasticsearchReporter_Tests
    {
        public ElasticsearchReporter_Tests()
        {
        }
        internal static Mock<IElasticClient> CreateCli<TDocument>(bool IndexResponse_IsValid = true, bool IndexExists_Exists = true) where TDocument : class
        {
            var bulkResponseMock = new Mock<IBulkResponse>();
            bulkResponseMock.Setup(m => m.IsValid).Returns(IndexResponse_IsValid);

            var indexResponseMock = new Mock<IIndexResponse>();
            indexResponseMock.Setup(m => m.IsValid).Returns(IndexResponse_IsValid);
            
            var indexExistsMock = new Mock<IExistsResponse>();
            indexExistsMock.Setup(m => m.IsValid).Returns(true);
            indexExistsMock.Setup(m => m.Exists).Returns(IndexExists_Exists);

            var indexCreateMock = new Mock<ICreateIndexResponse>();
            indexCreateMock.Setup(m => m.IsValid).Returns(true);
            indexCreateMock.Setup(m => m.Acknowledged).Returns(true);

            var cli = new Mock<IElasticClient>();
            cli.Setup(m => m.IndexManyAsync(It.IsAny<IEnumerable<TDocument>>(), It.IsAny<Nest.IndexName>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(
                    bulkResponseMock.Object
                    ));
            cli.Setup(m => m.IndexAsync(It.IsAny<TDocument>()
                , It.IsAny<Func<Nest.IndexDescriptor<TDocument>, Nest.IIndexRequest<TDocument>>>()
                , It.IsAny<CancellationToken>()
                ))
                .Returns(Task.FromResult(
                    indexResponseMock.Object
                    ));
            cli.Setup(m => m.IndexExistsAsync(It.IsAny<Nest.Indices>()
                , It.IsAny<Func<Nest.IndexExistsDescriptor, Nest.IIndexExistsRequest>>()
                , It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(
                    indexExistsMock.Object
                    ));
            cli.Setup(m => m.IndexCreateAsync(It.IsAny<Nest.IndexName>()
                , It.IsAny<Func<Nest.CreateIndexDescriptor, Nest.ICreateIndexRequest>>()
                , It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(
                    indexCreateMock.Object
                    ));
            return cli;
        }
        
        [Fact]
        public async void IndexDocumentExpectedExceptionTest()
        {
            ElasticsearchReporter r = new ElasticsearchReporter(CreateCli<ElasticsearchAccessLogDocument>(IndexResponse_IsValid : false).Object);
            var d = new ElasticsearchAccessLogDocument();
            await Assert.ThrowsAsync<ElasticsearchException>(() => r.Post(d, "testindex", false));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void AcessLogDocumentTest(bool IndexExists_Exists)
        {
            var cli = CreateCli<ElasticsearchAccessLogDocument>(IndexExists_Exists : IndexExists_Exists);
            ElasticsearchReporter r = new ElasticsearchReporter(cli.Object);
            var d = new ElasticsearchAccessLogDocument("mockid", appName: "mockapp", message:"mock msg", path:"mock path", controllerName:"mock_controller", actionName:"mockAction"
            , elapsedSeconds:2, parameters: new Dictionary<string, string>()
            {
                { "mockparam1", "mockvalue1"}
            }, resultCode:200, stat: new List<StatisticalDocument>()
            {
                new StatisticalDocument()
                {
                    Call = "mockcall"
                    , Comment = "mockcomment"
                    , ElapsedSeconds = 5
                }
            }, headers: new Dictionary<string, string>()
            {
                { "mockheader1", "mockheadervalue1"}
            });
            await r.Post(d, "testindex");

            cli.Verify(e => e.IndexExistsAsync(It.Is<Nest.Indices>(i => 
                    i.Match(all => false, many => many.Indices.Contains("testindex")))
            , It.IsAny<Func<IndexExistsDescriptor, IIndexExistsRequest>>()
            , It.IsAny<CancellationToken>()), Times.Once);
            if (false == IndexExists_Exists)
                cli.Verify(e => e.IndexCreateAsync(It.Is<Nest.IndexName>(i =>
                    i.Name == "testindex")
                    , It.IsAny<Func<CreateIndexDescriptor, ICreateIndexRequest>>()
                    , It.IsAny<CancellationToken>()), Times.Once);
            
            cli.Verify(e => e.IndexAsync(It.Is<ElasticsearchAccessLogDocument>(d =>
                    d.LogName == "access_log" && d.Timestamp < DateTime.Now && d.Timestamp > DateTime.Now.AddSeconds(-1)
                    && d.Id == "mockid" && d.AppName == "mockapp"
                    && d.RequestParameters.ContainsKey("mockparam1") && d.RequestParameters["mockparam1"] == "mockvalue1"
                    && d.Headers.ContainsKey("mockheader1") && d.Headers["mockheader1"] == "mockheadervalue1"
                    && d.Stat.Count == 1 && d.Stat[0].Call == "mockcall" && d.Stat[0].Comment == "mockcomment" && d.Stat[0].ElapsedSeconds == 5
                )
                , It.IsAny<Func<IndexDescriptor<ElasticsearchAccessLogDocument>, IIndexRequest<ElasticsearchAccessLogDocument>>>()
                , It.IsAny<CancellationToken>()), Times.Once);
                
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void AcessLogDocumentMultiTest(bool IndexExists_Exists)
        {
            var cli = CreateCli<ElasticsearchAccessLogDocument>(IndexExists_Exists: IndexExists_Exists);
            ElasticsearchReporter r = new ElasticsearchReporter(cli.Object);
            var d = new[]
            {
                new ElasticsearchAccessLogDocument("mockid1", appName: "mockapp", message: "mock msg", path: "mock path",
                    controllerName: "mock_controller", actionName: "mockAction"
                    , elapsedSeconds: 2, parameters: new Dictionary<string, string>()
                    {
                        {"mockparam1", "mockvalue1"}
                    }, resultCode: 200, stat: new List<StatisticalDocument>()
                    {
                        new StatisticalDocument()
                        {
                            Call = "mockcall", Comment = "mockcomment", ElapsedSeconds = 5
                        }
                    }, headers: new Dictionary<string, string>()
                    {
                        {"mockheader1", "mockheadervalue1"}
                    }),
                new ElasticsearchAccessLogDocument("mockid2", appName: "mockapp", message: "mock msg",
                    path: "mock path", controllerName: "mock_controller", actionName: "mockAction"
                    , elapsedSeconds: 2, parameters: new Dictionary<string, string>()
                    {
                        {"mockparam1", "mockvalue1"}
                    }, resultCode: 200, stat: new List<StatisticalDocument>()
                    {
                        new StatisticalDocument()
                        {
                            Call = "mockcall", Comment = "mockcomment", ElapsedSeconds = 5
                        }
                    }, headers: new Dictionary<string, string>()
                    {
                        {"mockheader1", "mockheadervalue1"}
                    })
            };
            await r.Post((IEnumerable<ElasticsearchAccessLogDocument>)d, "testindex");

            cli.Verify(e => e.IndexExistsAsync(It.Is<Nest.Indices>(i =>
                    i.Match(all => false, many => many.Indices.Contains("testindex")))
            , It.IsAny<Func<IndexExistsDescriptor, IIndexExistsRequest>>()
            , It.IsAny<CancellationToken>()), Times.Once);
            if (false == IndexExists_Exists)
                cli.Verify(e => e.IndexCreateAsync(It.Is<Nest.IndexName>(i =>
                    i.Name == "testindex")
                    , It.IsAny<Func<CreateIndexDescriptor, ICreateIndexRequest>>()
                    , It.IsAny<CancellationToken>()), Times.Once);

            cli.Verify(e => e.IndexManyAsync(It.Is< IEnumerable<ElasticsearchAccessLogDocument>>( e => e.Count(d =>
                    d.LogName == "access_log" && d.Timestamp < DateTime.Now && d.Timestamp > DateTime.Now.AddSeconds(-1)
                    && d.Id.StartsWith("mockid1") && d.AppName == "mockapp"
                    && d.RequestParameters.ContainsKey("mockparam1") && d.RequestParameters["mockparam1"] == "mockvalue1"
                    && d.Headers.ContainsKey("mockheader1") && d.Headers["mockheader1"] == "mockheadervalue1"
                    && d.Stat.Count == 1 && d.Stat[0].Call == "mockcall" && d.Stat[0].Comment == "mockcomment" && d.Stat[0].ElapsedSeconds == 5
                    ) == 1
                    &&
                    e.Count(d =>
                        d.LogName == "access_log" && d.Timestamp < DateTime.Now && d.Timestamp > DateTime.Now.AddSeconds(-1)
                        && d.Id.StartsWith("mockid2") && d.AppName == "mockapp"
                        && d.RequestParameters.ContainsKey("mockparam1") && d.RequestParameters["mockparam1"] == "mockvalue1"
                        && d.Headers.ContainsKey("mockheader1") && d.Headers["mockheader1"] == "mockheadervalue1"
                        && d.Stat.Count == 1 && d.Stat[0].Call == "mockcall" && d.Stat[0].Comment == "mockcomment" && d.Stat[0].ElapsedSeconds == 5
                    ) == 1
                )
                , It.Is<IndexName>(e => e.Name == "testindex")
                , It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async void ErrorLogDocumentTest()
        {
            ElasticsearchReporter r = new ElasticsearchReporter(CreateCli<ElasticsearchErrorDocument>().Object);
            var d = new ElasticsearchErrorDocument();
            await r.Post(d, "testindex");
            // TODO: Verify result
        }

        [Fact]
        public async void ErrorLogDocumentMultiTest()
        {
            ElasticsearchReporter r = new ElasticsearchReporter(CreateCli<ElasticsearchErrorDocument>().Object);
            var d = new ElasticsearchErrorDocument();
            await r.Post(new ElasticsearchErrorDocument[] { d, d }.AsEnumerable(), "testindex");
            // TODO: Verify result
        }

        [Fact]
        public async void SetupLogDocumentTest()
        {
            ElasticsearchReporter r = new ElasticsearchReporter(CreateCli<ElasticsearchSetupDocument>().Object);
            var d = new ElasticsearchSetupDocument();
            await r.Post(d, "testindex");
            // TODO: Verify result
        }

        [Fact]
        public async void SetupLogDocumentMultiTest()
        {
            ElasticsearchReporter r = new ElasticsearchReporter(CreateCli<ElasticsearchSetupDocument>().Object);
            var d = new ElasticsearchSetupDocument();
            await r.Post(new ElasticsearchSetupDocument[] { d, d }.AsEnumerable(), "testindex");
            // TODO: Verify result
        }
    }
}
