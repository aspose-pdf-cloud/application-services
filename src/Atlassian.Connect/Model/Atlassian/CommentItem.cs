using System;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class CommentItem
    {
        public string Self { get; set; }
        public string Id { get; set; }

        public Account Author { get; set; }

        public Document Body { get; set; }

        public Account UpdateAuthor { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public bool JsdPublic { get; set; }

        public override string ToString()
        {
            return Body == null ? "" : Body.ToString();
        }
    }
}
