namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class Version
    {
        public string Self { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool? Archived { get; set; }
        public bool? Released { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
