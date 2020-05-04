using System.Collections.Generic;
using System.Linq;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    /// <summary>
    /// https://developer.atlassian.com/cloud/jira/platform/apis/document/structure/
    /// https://developer.atlassian.com/cloud/jira/platform/apis/document/nodes/doc/
    /// </summary>
    public class Document
    {
        public int? Version { get; set; }
        public string Type { get; set; }

        public List<ContentItem> Content { get; set; }
        public override string ToString()
        {
            return Content == null? "" : string.Join(",", Content.Select(i => i.ToString()).ToArray());
        }
    }
}
