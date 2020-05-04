using System.Collections.Generic;
using System.Linq;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class Comment
    {
        public List<CommentItem> Comments { get; set; }
        public override string ToString()
        {
            return Comments == null ? "" : string.Join(";", Comments.Select(i => i.ToString()).ToArray());
        }
    }
}
