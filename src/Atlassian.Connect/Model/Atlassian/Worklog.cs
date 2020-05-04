using System.Collections.Generic;
using System.Linq;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class Worklog
    {
        public List<WorklogItem> Worklogs { get; set; }

        public override string ToString()
        {
            return Worklogs == null ? "" : string.Join(";", Worklogs.Select(i => i.ToString()).ToArray());
        }
    }
}
