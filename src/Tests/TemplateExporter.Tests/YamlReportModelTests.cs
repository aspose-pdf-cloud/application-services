using Aspose.Cloud.Marketplace.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Aspose.Cloud.Marketplace.App.TemplateExporter.Tests
{
    [Trait("TemplateExporterLibrary", "TamlReportModel_Tests")]
    public class TamlReportModel_Tests
    {
        [Fact]
        public void Test1()
        {
            string yaml = @"
Format: A4

DefaultFont:
  Name: ""{{defaultFontName}}""
  Size: 12
  Style: {{defaultFontStyle}}
Content:
{{#items}}
- Page:
  - Text: ""value {{itemval}}""
    HorizontalAlignment: {{horal}}
    VerticalAlignment: {{veral}}
    Font:
      Name: ""{{font}}""
      Size: {{fontsz}}
      Style: BoldItalic
{{/items}}
";
            var model = new YamlReportModel(yaml);
            var document = model.CreateReportModel(new {
                defaultFontName = "Arial",
                defaultFontStyle = "Italic",
                items = new List<dynamic>
                {
                    new {itemval = "item01", horal = "left", veral = "top", font = "Times", fontsz = 11},
                    new {itemval = "item02", horal = "right", veral = "bottom", font = "Consolas", fontsz = 12},
                }
            });
            Assert.Equal("Arial", document.DefaultFont.Name);
            Assert.Equal("Italic", document.DefaultFont.Style);
            Assert.Equal(2, document.Content.Count);
            Assert.Single(document.Content[0].Page);
            
            var p = document.Content[0].Page[0];
            Assert.Equal("value item01", p.Text);
            Assert.Equal("left", p.HorizontalAlignment);
            Assert.Equal("top", p.VerticalAlignment);
            Assert.Equal("Times", p.Font.Name);
            Assert.Equal(11, p.Font.Size);

            p = document.Content[1].Page[0];
            Assert.Equal("value item02", p.Text);
            Assert.Equal("right", p.HorizontalAlignment);
            Assert.Equal("bottom", p.VerticalAlignment);
            Assert.Equal("Consolas", p.Font.Name);
            Assert.Equal(12, p.Font.Size);
        }


        public void StringTest(string fmt, string text)
        {
            StringBuilder yaml = new StringBuilder();
            yaml.AppendLine("Content:")
                .AppendLine("- Page:")
                .AppendLine($"  - Text: {fmt}")
                .AppendLine ("    HorizontalAlignment: {{horal}}");
            var model = new YamlReportModel(yaml.ToString());
            var document = model.CreateReportModel(new
            {
                text = text,
            });
            Assert.Single(document.Content[0].Page);

            var p = document.Content[0].Page[0];
            Assert.Equal(text, p.Text);

        }

        [Fact]
        public void String_Test1()
        {
            //StringTest(@"""{{text}}""", "just \\the text");
            StringTest(@"'{{text}}'", "just \\the text");

        }
    }
}
