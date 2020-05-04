using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class Project
    {
        public string Self { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ProjectTypeKey { get; set; }
        public bool? Simplified { get; set; }
        public Dictionary<string, string> AvatarUrls { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
