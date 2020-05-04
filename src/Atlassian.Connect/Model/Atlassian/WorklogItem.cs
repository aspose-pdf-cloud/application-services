using System;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class WorklogItem
    {
        public string Self { get; set; }
        public string Id { get; set; }
        public string IssueId { get; set; }
        public Account Author { get; set; }
        public Account UpdateAuthor { get; set; }
        public Document Comment { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Started { get; set; }
        public string TimeSpent { get; set; }
        public int? TimeSpentSeconds { get; set; }
        public override string ToString()
        {
            return $"{Comment} by {Author}";
        }
    }
}
