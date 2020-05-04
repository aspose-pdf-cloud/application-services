using Nest;
using System;
using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.Services.Model.Elasticsearch
{
    public class ElasticsearchErrorInfoDocument
    {
        public string ErrorText { get; set; }

        [Keyword(Index = false)]
        public string ErrorTrace { get; set; }
        
        [Object(Enabled = false)]
        public ElasticsearchErrorInfoDocument InnerError { get; set; }
    }
    public class ElasticsearchErrorDocument : ElasticsearchBaseDocument, IClonneableTo<ElasticsearchErrorDocument>
    {
        [Object(Enabled = false)]
        public ElasticsearchErrorInfoDocument Error { get; set; }

        [Binary(Store = true)]
        public string InputData { get; set; }

        public ElasticsearchErrorDocument() : base()
        {
            LogName = "error_log";
        }
        public ElasticsearchErrorDocument(string id, Exception ex, string logName = null, string appName = null, string message = null, string path = null
            , string controllerName = null, string actionName = null
            , Dictionary<string, string> parameters = default, double? elapsedSeconds = null, int? resultCode = null, byte[] inputData = null)
            : base(id : id, logName: logName, appName : appName, message : message, path : path, controllerName:controllerName, actionName:actionName
                  , elapsedSeconds : elapsedSeconds, parameters: parameters, resultCode : resultCode)
        {
            if (string.IsNullOrEmpty(logName))
                LogName = "error_log";
            Error = error(ex);
            InputData = null == inputData ? null : Convert.ToBase64String(inputData);

        }
        public void CloneTo(ElasticsearchErrorDocument d)
        {
            CloneTo(d as ElasticsearchBaseDocument);
            d.Error = Error;
            d.InputData = InputData;
        }
        private static ElasticsearchErrorInfoDocument error(Exception ex, int recursionCounter = 0, int maxRecursionCounter = 5)
        {
            if (null == ex || recursionCounter >= maxRecursionCounter)
                return null;
            var result = new ElasticsearchErrorInfoDocument();
            result.ErrorText = ex.Message;
            result.ErrorTrace = ex.StackTrace;
            result.InnerError = error(ex.InnerException, recursionCounter + 1, maxRecursionCounter);
            return result;
        }
    }

    
}
