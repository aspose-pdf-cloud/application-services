using Aspose.Cloud.Marketplace.Common;
using Aspose.Cloud.Marketplace.Services.Model.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Aspose.Cloud.Marketplace.App.ElasticsearchLogging.Tests
{
    [Trait("ElasticsearchLogging", "ElasticsearchLoggingTests")]
    public class Model_Tests
    {
        internal readonly ElasticsearchBaseDocument _basedoc;
        internal readonly ElasticsearchAccessLogDocument _accesslogdoc;
        internal readonly ElasticsearchErrorDocument _errorlogdoc;
        public Model_Tests()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["param"] = "paramvalue";

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["header"] = "headervalue";


            List<StatisticalDocument> stat = new List<StatisticalDocument>();
            stat.Add(new StatisticalDocument()
            {
                Call = "testcall",
                Comment = "testcomment",
                ElapsedSeconds = 23
            });

            _basedoc = new ElasticsearchBaseDocument("id", "logname", "appname", message: "message", path: "path"
            , controllerName: "controller", actionName: "action"
            , elapsedSeconds: 0.1, parameters: parameters, resultCode: 200);

            _accesslogdoc = new ElasticsearchAccessLogDocument("id", "logname", "appname", message: "message", path: "path"
            , controllerName: "controller", actionName: "action"
            , elapsedSeconds: 0.1, parameters: parameters, resultCode: 200, stat:stat, headers:headers);

            _errorlogdoc = new ElasticsearchErrorDocument("id", new Exception("extest"), "logname", "appname", message: "message", path: "path"
            , controllerName: "controller", actionName: "action"
            , elapsedSeconds: 0.1, parameters: parameters, resultCode: 200, inputData: Encoding.UTF8.GetBytes("input\x0001data\x0002"));
        }
        [Fact]
        public void BaseDocumentTest01()
        {
            var d = new ElasticsearchBaseDocument("id", "logname", "appname");
            Assert.True(d.Timestamp < DateTime.Now && d.Timestamp > DateTime.Now.AddSeconds(-1), "Base document timestamp is not valid");
            Assert.Equal("id", d.Id);
            Assert.Equal("logname", d.LogName);
            Assert.Equal("appname", d.AppName);
        }

        private void check_basedoc(ElasticsearchBaseDocument d)
        {
            Assert.True(d.Timestamp < DateTime.Now && d.Timestamp > DateTime.Now.AddSeconds(-1), "Base document timestamp is not valid");
            Assert.Equal("id", d.Id);
            Assert.Equal("logname", d.LogName);
            Assert.Equal("appname", d.AppName);
            Assert.Equal("message", d.Message);

            Assert.Equal("path", d.Path);
            Assert.Equal("controller", d.ControllerName);
            Assert.Equal("action", d.ActionName);
            Assert.Equal(0.1, d.ElapsedSeconds);
            Assert.Equal(200, d.ResultCode);

            Assert.Single(d.RequestParameters);
            Assert.Equal("paramvalue", d.RequestParameters["param"]);
        }

        private void check_accesslogdoc(ElasticsearchAccessLogDocument d)
        {
            check_basedoc(d);
            Assert.Single(d.Stat);
            Assert.Equal("testcall", d.Stat[0].Call);

            Assert.Single(d.Headers);
            Assert.Equal("headervalue", d.Headers["header"]);
        }

        private void check_errorlogdoc(ElasticsearchErrorDocument d)
        {
            check_basedoc(d);
            Assert.Equal("extest", d.Error.ErrorText);
            Assert.Equal(Convert.ToBase64String(Encoding.UTF8.GetBytes("input\x0001data\x0002")), d.InputData);
        }
        [Fact]
        public void BaseDocumentTest02()
        {
            check_basedoc(_basedoc);
        }

        [Fact]
        public void BaseDocumentTestClone()
        {
            var d = new ElasticsearchBaseDocument();
            _basedoc.CloneTo(d);
            check_basedoc(d);
        }

        [Fact]
        public void AccesslogDocumentTestDefault()
        {
            var d = new ElasticsearchAccessLogDocument();
            Assert.Equal("access_log", d.LogName);
        }

        [Fact]
        public void AccesslogDocumentTest01()
        {
            check_accesslogdoc(_accesslogdoc);
        }

        [Fact]
        public void AccesslogDocumentTest02()
        {
            var d = new ElasticsearchAccessLogDocument();
            _accesslogdoc.CloneTo(d);
            check_accesslogdoc(d);
        }


        [Fact]
        public void ErrorlogDocumentTestDefault()
        {
            var d = new ElasticsearchErrorDocument();
            Assert.Equal("error_log", d.LogName);
        }

        [Fact]
        public void ErrorlogDocumentTest01()
        {
            check_errorlogdoc(_errorlogdoc);
        }

        [Fact]
        public void ErrorlogDocumentTest02()
        {
            var d = new ElasticsearchErrorDocument();
            _errorlogdoc.CloneTo(d);
            check_errorlogdoc(d);
        }
    }
}
