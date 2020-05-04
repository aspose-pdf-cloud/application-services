using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Tests
{
    public class RestApiClientTests
    {
        public static (Mock<HttpMessageHandler>, HttpClient) GetJiraHttpClientMock(Dictionary<string, Tuple<string, string>> setup, Uri baseAddress)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            foreach (var s in setup)
                messageHandlerMock
                    .Protected()
                    // Setup the PROTECTED method to mock
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(m => 
                            m.Method == new HttpMethod(s.Value.Item1) && m.RequestUri.Host == baseAddress.Host && m.RequestUri.PathAndQuery == s.Key)
                        , ItExpr.IsAny<CancellationToken>()
                    )
                    // prepare the expected response of the mocked http call
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(s.Value.Item2),
                    })
                    .Verifiable();
            return (messageHandlerMock, new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = baseAddress,
            });
        }

        public static (Mock<HttpMessageHandler>, HttpClient) GetJiraHttpClientMock(string requestsFolder, Uri baseAddress)
        {
            var requests = new Dictionary<string, Tuple<string, string>>();
            foreach (var infoFile in Directory.GetFiles(requestsFolder, "*.json"))
            {
                var props = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(infoFile));
                string responseDataFile = null == props.ResponseDataFile ? null : Path.Combine(requestsFolder, props.ResponseDataFile.ToString());
                Uri uri = new Uri(baseAddress, new Uri(props.RequestUri.ToString()).PathAndQuery);
                requests.Add(uri.PathAndQuery, new Tuple<string, string>(props.Method.ToString()
                    , null != responseDataFile && File.Exists(responseDataFile) ? File.ReadAllText(responseDataFile) : ""));
            }
            return GetJiraHttpClientMock(requests, baseAddress);
        }

        [Fact]
        public async void GET_Test()
        {
            /*
            var (handler1, client1) = GetJiraHttpClientMock(
                @"N:\maxx\Aspose\GitLab\Conholdate.Cloud\aspose.pdf-exporter-for-jira-cloud\src\AsposePdfExporterJiraCloud\responses"
                , new Uri("https://mock123jiracloud.com"));
            */
            var (handler, client) = GetJiraHttpClientMock(new Dictionary<string, Tuple<string, string>>
            {
                { "/rest/api/3/field", new Tuple<string, string>("GET", "{'prop':'value'}")}
            }, new Uri("https://mockjiracloud.com"));
            var api = new RestApiClient("issuer_mock", "123", "clientkey_mock", client);
            var result = await api.Get("/rest/api/3/field");
            Assert.Equal("value", JToken.Parse(result)["prop"]);
        }
    }
}
