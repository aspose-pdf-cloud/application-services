using Nest;
using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.Services.Model.Elasticsearch
{
    public class ElasticsearchAccessLogDocument : ElasticsearchBaseDocument, IClonneableTo<ElasticsearchAccessLogDocument>
    {
        //[Object(Enabled = false)]
        [Nested(IncludeInParent =true)]
        public List<Aspose.Cloud.Marketplace.Common.StatisticalDocument> Stat { get; set; }

        [Nested(IncludeInParent = true)]
        public Dictionary<string, string> Headers { get; set; }

        public ElasticsearchAccessLogDocument() : base()
        {
            LogName = "access_log";
        }

        public ElasticsearchAccessLogDocument(string id, string logName = null, string appName = null, string message = null, string path = null
            , string controllerName = null, string actionName = null
            , double? elapsedSeconds = null, Dictionary<string, string> parameters = default, int? resultCode = null, List<Common.StatisticalDocument> stat = null, Dictionary<string, string> headers = null) : 
            base(id : id, logName: logName, appName : appName, message : message, path : path
                , controllerName:controllerName, actionName:actionName, elapsedSeconds : elapsedSeconds, parameters : parameters, resultCode : resultCode)
        {
            if (string.IsNullOrEmpty(logName))
                LogName = "access_log";
            if (null != stat && stat.Count > 0)
                Stat = stat;
            if (null != headers && headers.Count > 0)
                Headers = headers;
        }
        
        public void CloneTo(ElasticsearchAccessLogDocument d)
        {
            CloneTo(d as ElasticsearchBaseDocument);
            d.Stat = Stat;
            d.Headers = Headers;
        }
    }
}
