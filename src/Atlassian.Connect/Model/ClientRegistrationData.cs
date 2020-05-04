namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model
{
    /// <summary>
    /// Client registration data
    /// https://developer.atlassian.com/cloud/jira/software/app-descriptor/#lifecycle
    /// </summary>
    public class ClientRegistrationData : Interface.IRegistrationData
    {
        public string Key { get; set; }
        public string ClientKey { get; set; }
        public string PublicKey { get; set; }
        public string SharedSecret { get; set; }
        public string ServerVersion { get; set; }
        public string PluginsVersion { get; set; }
        public string BaseUrl { get; set; }
        public string ProductType { get; set; }
        public string Description { get; set; }

        public string EventType { get; set; }
    }
}
