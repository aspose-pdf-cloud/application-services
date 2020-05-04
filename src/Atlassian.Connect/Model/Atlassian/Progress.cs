namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class ProgressType
    {
        public int? Progress { get; set; }
        public int? Total { get; set; }
        public int? Percent { get; set; }

        public override string ToString()
        {
            return $"{Progress}";
        }
    }
}
