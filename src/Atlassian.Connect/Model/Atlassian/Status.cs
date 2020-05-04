namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class Status
    {
        public string Self { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public StatusCategory StatusCategory { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
