namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class TimeTracking
    {
        public string OriginalEstimate { get; set; }
        public string RemainingEstimate { get; set; }
        public string TimeSpent { get; set; }
        public int? OriginalEstimateSeconds { get; set; }
        public int? RemainingEstimateSeconds { get; set; }
        public int? TimeSpentSeconds { get; set; }

        public override string ToString()
        {
            return $"{TimeSpent}";
        }
    }
}
