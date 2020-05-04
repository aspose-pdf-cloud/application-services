using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect
{
    /// <summary>
    /// Logs requests/responses in HttpClient
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private readonly bool _dump;
        public LoggingHandler(bool dump)
        {
            _dump = dump;
        }

        public LoggingHandler(HttpMessageHandler innerHandler, bool dump)
        : base(innerHandler)
        {
            _dump = dump;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Trace.WriteLine("Request:");
            Trace.WriteLine(request.ToString());
            string requestContent = null;
            if (request.Content != null)
            {
                requestContent = await request.Content.ReadAsStringAsync();
                Trace.WriteLine(requestContent);
            }
            Trace.WriteLine("-------------------");

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Trace.WriteLine("Response:");
            Trace.WriteLine(response.ToString());
            string responseContent = null;
            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
                Trace.WriteLine(responseContent);
            }

            if (_dump)
            {
                //string replaceHost = null;
                string replaceHost = "mockjiracloud.com";

                string folder = "responses";
                Directory.CreateDirectory(folder);
                string id = $"{replaceHost ?? request.RequestUri.Host}{request.RequestUri.LocalPath.Replace('/', '-')}#{request.RequestUri.GetHashCode():x}";
                string requestDataFile = string.IsNullOrEmpty(requestContent) ? null : $"{id}-requestdata.txt";
                string responseDataFile = string.IsNullOrEmpty(responseContent) ? null : $"{id}-responsedata.txt";
                if (!string.IsNullOrEmpty(requestDataFile))
                    File.WriteAllText(Path.Combine(folder, requestDataFile), string.IsNullOrEmpty(replaceHost) ? responseContent : requestContent.Replace(request.RequestUri.Host, replaceHost));
                if (!string.IsNullOrEmpty(responseDataFile))
                    File.WriteAllText(Path.Combine(folder, responseDataFile), string.IsNullOrEmpty(replaceHost) ? responseContent : responseContent.Replace(request.RequestUri.Host, replaceHost));
                File.WriteAllText(Path.Combine(folder, $"{id}-info.json"), JsonConvert.SerializeObject( new {
                    RequestUri= string.IsNullOrEmpty(replaceHost) ? request.RequestUri.ToString() : request.RequestUri.ToString().Replace(request.RequestUri.Host, replaceHost),
                    RequestDataFile = requestDataFile,
                    ResponseDataFile = responseDataFile,
                    RequestHeaders = request.Headers,
                    ResponseHeaders = response.Headers,
                    Date = DateTime.Now,
                    Method = request.Method.Method
                }, Formatting.Indented));
            }
            Trace.WriteLine("-------------------");

            return response;
        }
    }
}
