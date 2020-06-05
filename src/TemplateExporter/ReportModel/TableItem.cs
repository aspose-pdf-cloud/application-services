using System.Collections.Generic;

namespace Aspose.Cloud.Marketplace.Report.Model
{
    public class TableRow
    {
        public List<TableCell> Cells { get; set; }
    }
    public class TableCell
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public List<string> TextLines { get; set; }
        public Font Font { get; set; }
        public Border Border { get; set; }
        public int? ColSpan { get; set; }
        public int? RowSpan { get; set; }
        public string HorizontalAlignment { get; set; }
        public string VerticalAlignment { get; set; }

        public Margin Margin { get; set; }
    }
}
