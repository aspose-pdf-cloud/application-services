using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class Account
    {
        public string Self { get; set; }
        public string AccountId { get; set; }

        public Dictionary<string, string> AvatarUrls { get; set; }

        public string DisplayName { get; set; }

        public bool Active { get; set; }

        public string TimeZone { get; set; }
        public string AccountType { get; set; }

        public override string ToString()
        {
            return $"{DisplayName}";
        }
    }
}
