using Nest;
using System;
using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.Services.Model.Elasticsearch
{
    public interface IClonneableTo<T>
    {
        void CloneTo(T d);
    }
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class ElasticsearchBaseDocument : IClonneableTo<ElasticsearchBaseDocument>
    {
        public string Id { get; set; }
        public string LogName { get; set; }
        public string AppName { get; set; }

        [Date(Name = "@timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        public string Message { get; set; }
        public string Path { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }

        //[Object(Enabled = false)]
        //[Keyword(Index = false)]
        [Nested]
        public Dictionary<string, string> RequestParameters { get; set; }
        //public List<KeyValuePair<string, string>> RequestParameters { get; set; }

        public double? ElapsedSeconds { get; set; }
        public int? ResultCode { get; set; }

        public ElasticsearchBaseDocument()
        {
            Timestamp = DateTime.Now;
        }

        public ElasticsearchBaseDocument(string id, string logName, string appName, string message = null, string path = null
            , string controllerName = null, string actionName = null
            , double? elapsedSeconds = null, Dictionary<string, string> parameters = default, int? resultCode = null) : this()
        {
            Id = id;
            LogName = logName;
            AppName = appName;
            Message = message;
            Path = path;
            ControllerName = controllerName;
            ActionName = actionName;
            ElapsedSeconds = elapsedSeconds;
            //RequestParameters = parameters.ToList();
            RequestParameters = parameters;
            ResultCode = resultCode;
        }

        public void CloneTo(ElasticsearchBaseDocument d)
        {
            d.Id = Id;
            d.LogName = LogName;
            d.AppName = AppName;
            d.Timestamp = Timestamp;
            d.Message = Message;
            d.Path = Path;
            d.ControllerName = ControllerName;
            d.ActionName = ActionName;
            d.ElapsedSeconds = ElapsedSeconds;
            d.RequestParameters = RequestParameters;
            d.ResultCode = ResultCode;
        }
    }
}
