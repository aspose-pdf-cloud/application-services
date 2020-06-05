using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace Aspose.Cloud.Marketplace.Services
{
    public class ElasticsearchException : Exception
    {
        public ElasticsearchException(string message) : base(message) { }
    }
    /// <summary>
    /// Report specific document to elasticsearch
    /// supports elastic single node/ node pools as well as Key Authentication
    /// 
    /// </summary>
    public class ElasticsearchReporter
    {
        //private readonly ConnectionSettings _settings;
        protected readonly ElasticsearchInterface.IElasticClient _client;
        protected readonly string[] _uris;
        protected readonly string _indexName;
        protected readonly bool _debug;

        public string Index => _indexName;
        public ElasticsearchReporter(string[] elasticsearchUris, string indexName
            , string apiId = null, string apiKey = null
            , string username = null, string password = null
            , int? timeoutSeconds = null, bool debug = false)
        {
            _uris = elasticsearchUris;
            _indexName = indexName;
            _debug = debug;
            //https://www.elastic.co/guide/en/elasticsearch/client/net-api/2.x/connection-pooling.html
            if (null == elasticsearchUris || 0 == elasticsearchUris.Length)
                throw new ArgumentException("Uri list should not be empty", nameof(elasticsearchUris));
            IConnectionPool pool = elasticsearchUris.Length == 1 ? new SingleNodeConnectionPool(new Uri(elasticsearchUris.First())) as IConnectionPool :
                new StaticConnectionPool(_uris.Select(u => new Uri(u)));
            var _settings = new ConnectionSettings(pool)
                        .DefaultIndex(indexName)
                        //.DefaultMappingFor<ElasticsearchErrorDocument>(m => m)
                        //.IndexName())
                        ;
            if (!string.IsNullOrEmpty(apiId))
                _settings.ApiKeyAuthentication(apiId, apiKey);
            if (!string.IsNullOrEmpty(username))
            {
                _settings.BasicAuthentication(username, password);
                _settings.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
            }
            if (timeoutSeconds.HasValue)
            {
                _settings = _settings.RequestTimeout(TimeSpan.FromSeconds(timeoutSeconds.Value))
                    .MaxDeadTimeout(TimeSpan.FromSeconds(timeoutSeconds.Value));
            }
            if (_debug)
                _settings = _settings.DisableDirectStreaming();
            _client = new ElasticsearchImpl.ElasticClient(new ElasticClient(_settings));
        }

        public ElasticsearchReporter(ElasticsearchInterface.IElasticClient client)
        {
            _client = client;
        }


        public async Task Post<T>(IEnumerable<T> objs, string indexName = null, bool createIndexIfNotExists = true) where T: class
        {
            var index = indexName ?? _indexName;
            if (string.IsNullOrWhiteSpace(index))
                throw new ArgumentException("empty index", nameof(index));

            if (createIndexIfNotExists)
                await CheckIndex<T>(index);
            
            var objects = objs.ToList();

            var asyncIndexResponse = await _client.IndexManyAsync(objects, index);
            CheckResponse(asyncIndexResponse, $"Unable to post data. documents {objects.Count()}; index: {index}");
        }

        public async Task Post<T>(T obj, string indexName = null, bool createIndexIfNotExists = true) where T : class
        {
            var index = indexName ?? _indexName;
            if (string.IsNullOrWhiteSpace(index))
                throw new ArgumentException("empty index", nameof(index));

            if (createIndexIfNotExists)
                await CheckIndex<T>(index);

            var asyncIndexResponse = await _client.IndexAsync(obj, d => d.Index(index));
            CheckResponse(asyncIndexResponse, $"Unable to post data. index: {index}");
        }

        private async Task CheckIndex<T>(string indexName) where T: class
        {
            var exists = await _client.IndexExistsAsync(indexName);
            CheckResponse(exists, $"Unable to check if index exists. index: {indexName}", response => (response.ApiCall?.Success ?? false) && response.ServerError == null);
            if (!exists.Exists)
            {
                var createResponse = await _client.IndexCreateAsync(indexName, c => c.Map<T>(m => m.
                        AutoMap<T>(null, 3)
                    ));
                CheckResponse(createResponse, $"Unable to create index. index: {indexName}");
            }
            
        }
        /// <summary>
        /// Check response
        /// remove customCheck after resolving https://github.com/elastic/elasticsearch-net/issues/4684 
        /// </summary>
        /// <param name="response">response</param>
        /// <param name="failedMessage">exception message to throw</param>
        /// <param name="customCheck">custom check (temporarily solution)</param>
        private void CheckResponse(IResponse response, string failedMessage, Func<IResponse, bool> customCheck = null)
        {
            if ( null != customCheck && customCheck.Invoke(response) || response.IsValid)
                return;

            string nodeUri = response.ApiCall?.Uri?.GetLeftPart(UriPartial.Authority);
            throw new ElasticsearchException($"{failedMessage} node: {nodeUri}; error: {response.OriginalException?.Message}; debugInfo: {response.DebugInformation}");
            
        }
    }
}
