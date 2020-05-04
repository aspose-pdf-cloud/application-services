using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace Aspose.Cloud.Marketplace.Services.ElasticsearchImpl
{
    /// <summary>
    /// Nest.ElasticClient mock
    /// </summary>
    public class ElasticClient : ElasticsearchInterface.IElasticClient
    {
        Nest.ElasticClient _cli;
        public ElasticClient(Nest.ElasticClient cli) => _cli = cli;
        public async Task<ElasticsearchInterface.IBulkResponse> IndexManyAsync<T>(IEnumerable<T> objects, IndexName index = null, CancellationToken cancellationToken = default) where T : class =>
            new BulkResponse(await _cli.IndexManyAsync<T>(objects, index, cancellationToken));
        public async Task<ElasticsearchInterface.IIndexResponse> IndexAsync<TDocument>(TDocument document, Func<IndexDescriptor<TDocument>, IIndexRequest<TDocument>> selector, CancellationToken ct = default) where TDocument : class =>
            new IndexResponse(await _cli.IndexAsync(document, selector, ct));
        public async Task<ElasticsearchInterface.IExistsResponse> IndexExistsAsync(Indices index, Func<IndexExistsDescriptor, IIndexExistsRequest> selector = null, CancellationToken ct = default) =>
            new ExistsResponse(await _cli.Indices. ExistsAsync(index, selector, ct));
        public ElasticsearchInterface.IExistsResponse IndexExists(Indices index, Func<IndexExistsDescriptor, IIndexExistsRequest> selector = null) =>
            new ExistsResponse(_cli.Indices.Exists(index, selector));
        
        public async Task<ElasticsearchInterface.ICreateIndexResponse> IndexCreateAsync(IndexName index, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null, CancellationToken ct = default) =>
            new CreateIndexResponse(await _cli.Indices.CreateAsync(index, selector, ct));

        public void a()
        {
            //var a = _cli.Indices.Stats()
        }
    }

    internal class ResponseBase : ElasticsearchInterface.IResponseBase
    {
        protected readonly Nest.ResponseBase _base_resp;
        protected ResponseBase(Nest.ResponseBase resp) => _base_resp = resp;

        public virtual Elasticsearch.Net.IApiCallDetails ApiCall { get => _base_resp.ApiCall; set { } }
        public string DebugInformation => _base_resp.DebugInformation;
        public virtual bool IsValid => _base_resp.IsValid;
        public Exception OriginalException => _base_resp.OriginalException;
        public Elasticsearch.Net.ServerError ServerError => _base_resp.ServerError;

        public override string ToString() => _base_resp.ToString();
        public bool TryGetServerErrorReason(out string reason)
        {
            reason = ServerError?.Error?.ToString();
            return !string.IsNullOrEmpty(reason);
        }
    }

    internal class ExistsResponse : ResponseBase, ElasticsearchInterface.IExistsResponse
    {
        protected readonly Nest.ExistsResponse _resp;
        public ExistsResponse(Nest.ExistsResponse resp) : base(resp) => _resp = resp;
        public bool Exists => _resp.Exists;
    }

    internal class BulkResponse : ResponseBase, ElasticsearchInterface.IBulkResponse
    {
        protected readonly Nest.BulkResponse _resp;
        public BulkResponse(Nest.BulkResponse resp) : base(resp) => _resp = resp;
        public bool Errors => _resp.Errors;
        public IReadOnlyList<BulkResponseItemBase> Items => _resp.Items;
        public IEnumerable<BulkResponseItemBase> ItemsWithErrors => _resp.ItemsWithErrors;
        public long Took => _resp.Took;
    }

    internal class IndexResponse : ResponseBase, ElasticsearchInterface.IIndexResponse
    {
        protected readonly Nest.IndexResponse _resp;
        public IndexResponse(Nest.IndexResponse resp) : base(resp) => _resp = resp;
        public string Id => _resp.Id;
        public string Index => _resp.Index;
        public long PrimaryTerm => _resp.PrimaryTerm;
        public Result Result => _resp.Result;
        public long SequenceNumber => _resp.SequenceNumber;
        public ShardStatistics Shards => _resp.Shards;
        public string Type => _resp.Type;
        public long Version => _resp.Version;
    }

    internal class CreateIndexResponse : ResponseBase, ElasticsearchInterface.ICreateIndexResponse
    {
        protected readonly Nest.CreateIndexResponse _resp;
        public CreateIndexResponse(Nest.CreateIndexResponse resp) : base(resp) => _resp = resp;
        public bool Acknowledged => _resp.Acknowledged;
        public bool ShardsAcknowledged { get { return _resp.ShardsAcknowledged; } set { _resp.ShardsAcknowledged = value; } }
        public string Index { get { return _resp.Index; } set { _resp.Index = value; } }
    }
}