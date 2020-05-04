using System;
using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class IssueFields
    {
        public DateTime? StatusCategoryChangeDate { get; set; }
        public IssueType IssueType { get; set; }
        public int? TimeSpent { get; set; }
        public int? AggregateTimeSpent { get; set; }
        public int? WorkRatio { get; set; }
        public int? TimeOriginalEstimate { get; set; }
        public Project Project { get; set; }
        public List<Version> FixVersions { get; set; }
        public List<string> Labels { get; set; }
        public List<Component> Components { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? DueDate { get; set; }
        public TimeTracking Timetracking { get; set; }
        public List<Attachment> Attachment { get; set; }
        public Document Description { get; set; }
        public string Summary { get; set; }
        public Account Assignee { get; set; }
        public Account Creator { get; set; }

        public Account Reporter { get; set; }

        public List<JiraIssue> Subtasks { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        
        public Comment Comment { get; set; }
        public ProgressType Progress { get; set; }
        public ProgressType AggregateProgress { get; set; }
        public Vote Votes { get; set; }
        public Worklog Worklog { get; set; }

        public override string ToString()
        {
            return Summary;
        }
    }
}
