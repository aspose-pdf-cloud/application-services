using System.Collections.Generic;
using System.Linq;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class ContentItem
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public List<ContentItem> Content { get; set; }
        public override string ToString()
        {
            List<string> items = new List<string>();
            if (!string.IsNullOrEmpty(Text))
                items.Add(Text);
            if (Content != null)
                items.AddRange(Content.Select(i => i.ToString()).ToArray());

            return string.Join(",", items);
        }
    }
}
