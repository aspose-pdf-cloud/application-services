using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.Report.Model
{
    public class PageContentItem
    {
        public string Text { get; set; }
        public List<string> TextLines { get; set; }
        public string Url { get; set; }
        public Location Location { get; set; }
        public string Table { get; set; }
        public List<TableRow> Rows { get; set; }

        public string HorizontalAlignment { get; set; }
        public string VerticalAlignment { get; set; }
        public string WrapMode { get; set; }

        public Font Font { get; set; }

        public Border Border { get; set; }
        public List<string> ColumnWidths { get; set; }
        public Margin DefaultContentMargin { get; set; }
        public string DefaultContentHorizontalAlignment { get; set; }
        public string DefaultContentVerticalAlignment { get; set; }
    }
}
