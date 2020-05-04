using System;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model.Atlassian
{
    public class Attachment
    {
        public string Self { get; set; }
        public string Id { get; set; }
        public string FileName { get; set; }
        public Account Author { get; set; }
        public DateTime? Created { get; set; }
        public int? Size { get; set; }
        public string MimeType { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; }
        public override string ToString()
        {
            return $"{FileName}";
        }
    }
}
