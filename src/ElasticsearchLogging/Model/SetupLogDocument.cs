using System;
using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.Services.Model.Elasticsearch
{
    public class ElasticsearchSetupDocument : ElasticsearchBaseDocument, IClonneableTo<ElasticsearchSetupDocument>
    {
        public string Action { get; set; }
        public string ActionId { get; set; }
        public string ActionOriginator { get; set; }
        public string Subscriber { get; set; }
        public ElasticsearchSetupDocument() : base()
        {
            LogName = "setup_log";
        }
        public ElasticsearchSetupDocument(string id, string action, string logName = null, string appName = null
            , string actionId = null, string actionOriginator = null, DateTimeOffset? actionDate = null, string subscriber = null
            , string message = null, string path = null
            , string controllerName = null, string actionName = null
            , double? elapsedSeconds = null, Dictionary<string, string> parameters = default, int? resultCode = null)
            : base(id: id, logName: logName, appName: appName, message: message, path: path,
                controllerName: controllerName, actionName: actionName
                , elapsedSeconds: elapsedSeconds, parameters: parameters, resultCode: resultCode)
        {
            Action = action;
            ActionId = actionId;
            ActionOriginator = actionOriginator;
            Subscriber = subscriber;
            if (string.IsNullOrEmpty(logName))
                LogName = "setup_log";
            Timestamp = actionDate ?? Timestamp;
        }
        public void CloneTo(ElasticsearchSetupDocument d)
        {
            CloneTo(d as ElasticsearchBaseDocument);

            d.Action = Action;
            d.ActionId = ActionId;
            d.ActionOriginator = ActionOriginator;
            d.Subscriber = Subscriber;
        }
    }
}
