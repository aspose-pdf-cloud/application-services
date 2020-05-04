namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class Vote
    {
        public string Self { get; set; }
        public int? Votes { get; set; }
        public bool? HasVoted { get; set; }

        public override string ToString()
        {
            return $"{Votes}";
        }
    }
}
