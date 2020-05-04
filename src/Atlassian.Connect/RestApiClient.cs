using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect
{
    /// <summary>
    /// Rest client for Jira cloud instances
    /// </summary>
    public class RestApiClient
    {
        private readonly HttpClient _client;
        private readonly string _sharedSecret, _issuer, _subject;
        public RestApiClient(string issuer, string sharedSecret, string subject, HttpClient client)
        {
            _client = client;
            _sharedSecret = sharedSecret;
            _issuer = issuer;
            _subject = subject;
        }

        public async Task<string> Send(HttpMethod method, string path, string queryString = "")
        {
            var token = Utils.EncodeToken(_sharedSecret, _issuer, _subject, 5, method.Method, path, queryString);
            string requestUri = path;
            if (!string.IsNullOrEmpty(queryString))
                requestUri += $"?{queryString}";
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("JWT", token);
            //requestMessage.RequestUri.Query = queryString;
            using var response = await _client.SendAsync(requestMessage);
            string responseData = await response.Content.ReadAsStringAsync();
            return JToken.Parse(responseData).ToString(Formatting.Indented);
        }

        public async Task<HttpResponseMessage> Send(HttpRequestMessage message)
        {
            // Awful hack to allow message.RequestUri be relative
            Uri tmpUri = new Uri(new Uri("http://dummy.com"), message.RequestUri.ToString());
            var token = Utils.EncodeToken(_sharedSecret, _issuer, _subject, 5, message.Method.ToString(), tmpUri.LocalPath, tmpUri.Query);
            message.Headers.Authorization = new AuthenticationHeaderValue("JWT", token);
            //requestMessage.RequestUri.Query = queryString;
            return await _client.SendAsync(message);
        }


        public async Task<string> Get(string path, string queryString = "")
        {
            return await Send(HttpMethod.Get, path, queryString);
        }
    }
}
