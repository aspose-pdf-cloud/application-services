namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class JiraIssue
    {
        public string Expand { get; set; }
        public string Id { get; set; }

        public string Self { get; set; }
        public string Key { get; set; }

        public IssueFields Fields { get; set; }

        public override string ToString()
        {
            return Fields.ToString(); 
        }
    }
}
