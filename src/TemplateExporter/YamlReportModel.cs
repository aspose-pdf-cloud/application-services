using System.IO;
using Newtonsoft.Json;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Settings;
using Stubble.Extensions.JsonNet;
using Stubble.Helpers;
using YamlDotNet.Serialization;

namespace Aspose.Cloud.Marketplace.Report
{
    /// <summary>
    /// Creates docuemnt model from the given yaml
    /// </summary>
    public class YamlReportModel : IReportModel
    {
        protected string _yamlTemplateContent;
        protected StubbleVisitorRenderer _stubbleRenderer;
        public bool GenerateQRCode { get; set; } = true;
        public string RenderedYamlDocument { get; set; }
        public string RenderedJsonDocument { get; set; }

        public YamlReportModel(string yamlTemplateContent)
        {
            _yamlTemplateContent = yamlTemplateContent;

            var helpers = new Stubble.Helpers.Helpers()
                .Register("PrintListWithComma", (context) => string.Join(", ", context.Lookup<int[]>("List")));


            _stubbleRenderer = new StubbleBuilder()
              .Configure(settings => {
                  settings.SetIgnoreCaseOnKeyLookup(true);
                  settings.SetMaxRecursionDepth(512);
                  settings.AddHelpers(helpers);
                  settings.AddJsonNet();
              })
              .Build();
        }
        
        public Model.Document CreateReportModel(dynamic data)
        {
            var yamlDeserializer = new DeserializerBuilder().Build();
            var yamlSerializerJson = new SerializerBuilder().JsonCompatible().Build();
            RenderedYamlDocument = _stubbleRenderer.Render(_yamlTemplateContent, data);
            var renderedYamlObj = yamlDeserializer.Deserialize<object>(RenderedYamlDocument);
            RenderedJsonDocument = yamlSerializerJson.Serialize(renderedYamlObj);
            return JsonConvert.DeserializeObject<Model.Document>(RenderedJsonDocument);
        }
    }
}
