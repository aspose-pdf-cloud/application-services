using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;

namespace Aspose.Cloud.Marketplace.Services.ElasticsearchInterface
{
    /// <summary>
    /// Since Nest does not support interfaces we have to invent them
    /// </summary>
    public interface IElasticClient
    {
        Task<IBulkResponse> IndexManyAsync<T>(IEnumerable<T> objects, IndexName index = null, CancellationToken cancellationToken = default) where T : class;
        Task<IIndexResponse> IndexAsync<TDocument>(TDocument document, Func<IndexDescriptor<TDocument>, IIndexRequest<TDocument>> selector, CancellationToken ct = default) where TDocument : class;
        Task<IExistsResponse> IndexExistsAsync(Indices index, Func<IndexExistsDescriptor, IIndexExistsRequest> selector = null, CancellationToken ct = default);
        IExistsResponse IndexExists(Indices index, Func<IndexExistsDescriptor, IIndexExistsRequest> selector = null);
        Task<ICreateIndexResponse> IndexCreateAsync(IndexName index, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null, CancellationToken ct = default);
    }
    
    public interface IResponseBase : Nest.IResponse, IElasticsearchResponse 
    {
    }

    public interface IWriteResponseBase : IResponseBase
    {
        string Id { get; }
        string Index { get; }
        long PrimaryTerm { get; }
        Result Result { get; }
        long SequenceNumber { get; }
        ShardStatistics Shards { get; }
        string Type { get; }
        long Version { get; }
    }
    public interface IIndexResponse : IWriteResponseBase { }

    public interface IExistsResponse : IResponse
    {
        bool Exists { get; }
    }

    public interface IBulkResponse : IResponseBase
    {
        bool Errors { get; }
        IReadOnlyList<BulkResponseItemBase> Items { get; }
        IEnumerable<BulkResponseItemBase> ItemsWithErrors { get; }
        long Took { get; }
    }

    public interface IAcknowledgedResponseBase : IResponseBase
    {
        bool Acknowledged { get; }
    }

    public interface ICreateIndexResponse : IAcknowledgedResponseBase
    {
        bool ShardsAcknowledged { get; set; }
        string Index { get; set; }
    }
}