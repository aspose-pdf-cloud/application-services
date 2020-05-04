using Microsoft.AspNetCore.Http;
using System;
using Xunit;
using Aspose.Cloud.Marketplace.App.Middleware;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Aspose.Cloud.Marketplace.Services;
using Aspose.Cloud.Marketplace.Services.Model.Elasticsearch;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Aspose.Cloud.Marketplace.App.AppMiddleware.Tests
{
    [Trait("AppMiddleware", "Middleware_Tests")]
    public class Middleware_Tests
    {
        internal HttpContext Context;
        internal Mock<ILoggingService> RemoteLoggerMock;

        internal Mock<IAppClient> AppClienMock;
        internal Mock<IAppCustomErrorReportingClient> StoreAppClientMock;
        internal IOptions<LogMiddlewareOptions> middlewareOptions;
        public Middleware_Tests()
        {
            Context = new DefaultHttpContext();
            Context.Request.Headers["device-id"] = "20317";
            Context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"q1", new StringValues("q1_value") }
            });
            Context.Request.Form = new FormCollection(new Dictionary<string, StringValues>()
            {
                {"f1", new StringValues("f1_value") }
            });
            RoutingFeature routingFeature = new RoutingFeature();
            routingFeature.RouteData = new RouteData(new RouteValueDictionary(new Dictionary<string, string>()
            {
                {"controller", "ctest" },
                {"action", "atest" }
            }));
            Context.Features[typeof(IRoutingFeature)] = routingFeature;

            RemoteLoggerMock = new Mock<ILoggingService>();
            RemoteLoggerMock.Setup(f => f.ReportAccessLog(It.IsAny<ElasticsearchAccessLogDocument>()))
                .Returns((Func<ElasticsearchAccessLogDocument, Task>)(async (d) => await Task.CompletedTask));

            RemoteLoggerMock.Setup(f => f.ReportErrorLog(It.IsAny<ElasticsearchErrorDocument>()))
                .Returns((Func<ElasticsearchErrorDocument, Task>)(async (d) => await Task.CompletedTask));

            AppClienMock = SetupAppCliMock(new Mock<IAppClient>());
            
            StoreAppClientMock = new Mock<IAppCustomErrorReportingClient>();
            SetupAppCliMock(StoreAppClientMock.As<IAppClient>());
            StoreAppClientMock.Setup(e =>
                    e.ReportException(It.IsAny<ElasticsearchErrorDocument>(), It.IsAny<IServiceProvider>()))
                .Returns(Task.FromResult("errorfile.json"));

            middlewareOptions = Options.Create(new LogMiddlewareOptions
            {
                //AppName = "mock_app_name"
            });
        }

        internal static Mock<IAppClient> SetupAppCliMock(Mock<IAppClient> mock)
        {
            mock.Setup(e => e.ErrorResponseInfo(It.IsAny<Exception>()))
                .Returns((404, "notfound", "not found descr", new byte[] { 0x01, 0x02, 0x03 }));
            mock.SetupGet(p => p.RequestId).Returns("mockid");
            mock.SetupGet(p => p.AppName).Returns("mock_app_name");
            mock.SetupGet(p => p.ElapsedSeconds).Returns(55);
            mock.SetupProperty(p => p.Stat,
                new List<Common.StatisticalDocument> {
                    new Common.StatisticalDocument {
                        Call = "mockcall",
                        Comment = "mockcomment",
                        ElapsedSeconds = 44
                    }}
            );
            return mock;
        }

        [Fact]
        public async void AccessLogMiddleware_Tests()
        {
            var nullLogger = new NullLogger<ElasticsearchAccessLogMiddleware<IAppClient>>();
            var middleware = new ElasticsearchAccessLogMiddleware<IAppClient>(async (c) => { await Task.CompletedTask; }, nullLogger, middlewareOptions);

            await middleware.Invoke(Context, RemoteLoggerMock.Object, AppClienMock.Object);
            RemoteLoggerMock.Verify(e => e.ReportAccessLog(It.Is<ElasticsearchAccessLogDocument>(d => 
                d.LogName == "access_log" && d.AppName == "mock_app_name" &&
                d.ControllerName == "ctest" && d.ActionName == "atest" && d.RequestParameters.Count == 2 &&
                d.RequestParameters["q1"] == "q1_value" && d.RequestParameters["f1"] == "f1_value"
                )));
        }

        [Fact]
        public async void ErrorLogMiddleware_Tests()
        {
            var nullLogger = new NullLogger<ElasticsearchExceptionHandlingMiddleware<IAppClient>>();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ILoggingService>(provider => RemoteLoggerMock.Object)
                .BuildServiceProvider();

            var middleware = new ElasticsearchExceptionHandlingMiddleware<IAppClient>(async (c) => { throw new Exception("test"); }, nullLogger, serviceProvider, middlewareOptions);
            string resp = string.Empty;
            using (var bodyMemoryStream = new MemoryStream())
            {
                Context.Response.Body = bodyMemoryStream;
                await middleware.Invoke(Context, AppClienMock.Object);
                resp = Encoding.UTF8.GetString(bodyMemoryStream.ToArray());
            }
            Assert.NotEmpty(resp);
            Assert.Equal(404, Context.Response.StatusCode);
            RemoteLoggerMock.Verify(e => e.ReportErrorLog(It.Is<ElasticsearchErrorDocument>(d =>
                d.LogName == "error_log" && d.AppName == "mock_app_name" &&
                d.ControllerName == "ctest" && d.ActionName == "atest" && d.RequestParameters.Count == 2 &&
                d.RequestParameters["q1"] == "q1_value" && d.RequestParameters["f1"] == "f1_value"
            )));
            JToken expected = JToken.FromObject(new
            {
                request_id = "mockid",
                error = "notfound",
                error_description = "not found descr",
                error_result = (string)null
            });
            JToken actual = JToken.Parse(resp);

            Assert.True(JToken.DeepEquals(expected, actual), $"Response mistmatch. Expected {expected.ToString(Newtonsoft.Json.Formatting.None)}, actual {actual.ToString(Newtonsoft.Json.Formatting.None)}");
        }

        [Fact]
        public async void ErrorLogStoreMiddleware_Tests()
        {
            var nullLogger = new NullLogger<StoreExceptionHandlingMiddleware<IAppCustomErrorReportingClient>>();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ILoggingService>(provider => RemoteLoggerMock.Object)
                .BuildServiceProvider();

            var middleware = new StoreExceptionHandlingMiddleware<IAppCustomErrorReportingClient>(async (c) => { throw new Exception("test"); }, nullLogger, serviceProvider, middlewareOptions);
            string resp = string.Empty;
            using (var bodyMemoryStream = new MemoryStream())
            {
                Context.Response.Body = bodyMemoryStream;
                await middleware.Invoke(Context, StoreAppClientMock.Object);
                resp = Encoding.UTF8.GetString(bodyMemoryStream.ToArray());
            }
            Assert.NotEmpty(resp);
            Assert.Equal(404, Context.Response.StatusCode);
            // in this case we don't want to populate d.InputData in elasticsearch
            RemoteLoggerMock.Verify(e => e.ReportErrorLog(It.Is<ElasticsearchErrorDocument>(d =>
                d.LogName == "error_log" && d.AppName == "mock_app_name" &&
                d.ControllerName == "ctest" && d.ActionName == "atest" && d.RequestParameters.Count == 2 &&
                d.RequestParameters["q1"] == "q1_value" && d.RequestParameters["f1"] == "f1_value" &&
                 string.IsNullOrEmpty(d.InputData)
            )));
            // ... but we want it in .ReportException call
            StoreAppClientMock.Verify(e => e.ReportException(It.Is<ElasticsearchErrorDocument>(d =>
                !string.IsNullOrEmpty(d.InputData))
                , It.IsAny<IServiceProvider>()));
            JToken expected = JToken.FromObject(new
            {
                request_id = "mockid",
                error = "notfound",
                error_description = "not found descr",
                error_result = "errorfile.json"
            });
            JToken actual = JToken.Parse(resp);

            Assert.True(JToken.DeepEquals(expected, actual), $"Response mistmatch. Expected {expected.ToString(Newtonsoft.Json.Formatting.None)}, actual {actual.ToString(Newtonsoft.Json.Formatting.None)}");
        }
    }
}
