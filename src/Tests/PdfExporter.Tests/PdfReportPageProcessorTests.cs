using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BarcodeRequests = Aspose.BarCode.Cloud.Sdk.Model.Requests;
using Aspose.Cloud.Marketplace.Report;
using Aspose.Cloud.Marketplace.Report.Model;
using PdfApi = Aspose.Pdf.Cloud.Sdk.Api;
using PdfModel = Aspose.Pdf.Cloud.Sdk.Model;
using BarcodeIntf = Aspose.BarCode.Cloud.Sdk.Interfaces;
using Moq;
using Xunit;

namespace Aspose.Cloud.Marketplace.App.PdfExporter.Tests
{
    public class PdfReportPageProcessorFixture
    {
        public Mock<PdfApi.IPdfApi> PdfApiMock;
        public Mock<BarcodeIntf.IBarcodeApi> BarcodeMock;

        public PdfReportPageProcessorFixture()
        {
            PdfApiMock = Setup(new Mock<PdfApi.IPdfApi>());
            BarcodeMock = Setup(new Mock<BarcodeIntf.IBarcodeApi>());
        }
        public static Mock<PdfApi.IPdfApi> Setup(Mock<PdfApi.IPdfApi> mock)
        {
            mock.Setup(f => f.CreateFolderAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            mock.Setup(f => f.DeleteFolderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);
            mock.Setup(f => f.ObjectExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new PdfModel.ObjectExist(false, false)));

            mock.Setup(f => f.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(async () => await Task.FromResult((Stream)new MemoryStream(new byte[] { 0x01, 0x02, 0x03 })));
            mock.Setup(f => f.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mock.Setup(f => f.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mock.Setup(f => f.GetDocumentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new PdfModel.DocumentResponse(200, "", new PdfModel.Document(), new List<string>())));

            mock.Setup(f => f.PutCreateDocumentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new PdfModel.DocumentResponse(200, "", new PdfModel.Document(), new List<string>())));

            mock.Setup(f => f.PostDocumentTextHeaderAsync(It.IsAny<string>(), It.IsAny<PdfModel.TextHeader>()
                    , It.IsAny<int?>(), It.IsAny<int?>()
                    , It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new PdfModel.AsposeResponse(200, "OK")));

            mock.Setup(f => f.PostDocumentTextFooterAsync(It.IsAny<string>(), It.IsAny<PdfModel.TextFooter>()
                    , It.IsAny<int?>(), It.IsAny<int?>()
                    , It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new PdfModel.AsposeResponse(200, "OK")));

            mock.Setup(f => f.PostPageTablesAsync(It.IsAny<string>(), It.IsAny<int>()
                    , It.IsAny<List<PdfModel.Table>>(), It.IsAny<string>()
                    , It.IsAny<string>()))
                .Returns(Task.FromResult(new PdfModel.AsposeResponse(200, "OK")));

            mock.Setup(f => f.PutAddTextAsync(It.IsAny<string>(), It.IsAny<int>()
                    , It.IsAny<PdfModel.Paragraph>(), It.IsAny<string>()
                    , It.IsAny<string>()))
                .Returns(Task.FromResult(new PdfModel.AsposeResponse(200, "OK")));

            mock.Setup(f => f.PostInsertImageAsync(It.IsAny<string>(), It.IsAny<int>()
                    , It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>()
                    , It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.FromResult(new PdfModel.AsposeResponse(200, "OK")));
            return mock;
        }

        public static Mock<BarcodeIntf.IBarcodeApi> Setup(Mock<BarcodeIntf.IBarcodeApi> mock)
        {
            mock.Setup(f => f.PutBarcodeGenerateFile(It.IsAny<BarcodeRequests.PutBarcodeGenerateFileRequest>()))
                //.Verifiable();
                .Callback(() => { });
            return mock;
        }
    }
    [Trait("PdfExporter", "PdfReportPageProcessor_Tests")]
    public class PdfReportPageProcessor_Tests : IClassFixture<PdfReportPageProcessorFixture>
    {
        internal PdfReportPageProcessorFixture Fixture;
        internal Mock<PdfApi.IPdfApi> PdfApiMock => Fixture.PdfApiMock;
        internal Mock<BarcodeIntf.IBarcodeApi> BarcodeMock => Fixture.BarcodeMock;
        public PdfReportPageProcessor_Tests(PdfReportPageProcessorFixture fixture)
        {
            Fixture = fixture;
            BarcodeMock.Invocations.Clear();
            PdfApiMock.Invocations.Clear();
        }

        internal async Task<PdfReport.PdfReportPageProcessor> GetPageProcessor()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            report._defaultFont = new Font()
            {
                Name = "Arial",
                Size = 10,
                Style = "regular"
            };
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");

            return new PdfReport.PdfReportPageProcessor(report, "file_01.pdf", "a/b/c");
        }


        [Fact]
        public async void PrepareDocument_Tests()
        {
            var processor = await GetPageProcessor();

            await processor.PrepareDocument();

            PdfApiMock.Verify(e => e.PutCreateDocumentAsync("file_01.pdf", null, "a/b/c"), Times.Once);
            PdfApiMock.Verify(e => e.PostDocumentTextHeaderAsync("file_01.pdf", It.Is<PdfModel.TextHeader>(h => 
                    Regex.IsMatch(h.Value, "Generated.*by Aspose.PDF Exporter"))
                , It.IsAny<int?>(), It.IsAny<int?>(), null, "a/b/c"), Times.Once);
            PdfApiMock.Verify(e => e.PostDocumentTextFooterAsync("file_01.pdf", It.Is<PdfModel.TextFooter>(h =>
                    Regex.IsMatch(h.Value, "Powered by Aspose.PDF for Cloud, Aspose.Barcode for Cloud"))
                , It.IsAny<int?>(), It.IsAny<int?>(), null, "a/b/c"), Times.Once);
        }

        [Fact]
        public async void GenerateQr_Tests()
        {
            var processor = await GetPageProcessor();

            await processor.GenerateQr("qr.png", "dummy", null, "a/b/c");

            BarcodeMock.Verify(e => e.PutBarcodeGenerateFile(It.IsAny<BarcodeRequests.PutBarcodeGenerateFileRequest>()), Times.Once);

            BarcodeMock.Invocations.Clear();

            byte[] actualContent;
            byte[] expectedContent = { 0x01, 0x02, 0x03 };
            await using (var ms = new MemoryStream())
            {
                await processor.GenerateQr("qr.png", "dummy", ms, "a/b/c");
                actualContent = ms.ToArray();
            }
            BarcodeMock.Verify(e => e.PutBarcodeGenerateFile(It.IsAny<BarcodeRequests.PutBarcodeGenerateFileRequest>()), Times.Once);
            PdfApiMock.Verify(e => e.DownloadFileAsync("a/b/c/qr.png", null, null), Times.Exactly(1));
            Assert.True(actualContent.SequenceEqual(expectedContent), "Barcode bytes do not match");
        }

        [Fact]
        public void HtmlColor_Tests()
        {
            var actual = PdfReport.PdfReportPageProcessor.HtmlColor("cadetblue");
            var _expected = System.Drawing.Color.FromArgb(unchecked((int)0xFF5F9EA0));
            var expected = new PdfModel.Color(_expected.A, _expected.R, _expected.G, _expected.B);
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void GraphInfo_Tests()
        {
            var actual = PdfReport.PdfReportPageProcessor.GraphInfo(new []
            {
                new Report.Model.GraphInfo()
                {
                    LineWidth = null, Color = "red"
                }, new Report.Model.GraphInfo()
                {
                    LineWidth = 1, Color = null
                }
            });
            Assert.Equal(1, actual.LineWidth);
            Assert.Equal(new PdfModel.Color(255,255,0,0), actual.Color);
        }

        [Fact]
        public void GetValue_Tests()
        {
            Assert.Equal("default", PdfReport.PdfReportPageProcessor.GetValue(null, "default"));
            Assert.Equal("1", PdfReport.PdfReportPageProcessor.GetValue("1", "default"));

            Assert.Equal(12, PdfReport.PdfReportPageProcessor.GetValue<int?>(null, 12));
            Assert.Equal(1, PdfReport.PdfReportPageProcessor.GetValue<int?>(1, 12));
        }

        [Fact]
        public void GraphInfo_Tests2()
        {
            var actual = PdfReport.PdfReportPageProcessor.GraphInfo(
                new Report.Model.GraphInfo()
                {
                    LineWidth = null, Color = "red"
                }, new Report.Model.GraphInfo()
                {
                    LineWidth = 1, Color = null
                }
            );
            Assert.Equal(1, actual.LineWidth);
            Assert.Equal(new PdfModel.Color(255, 255, 0, 0), actual.Color);
        }


        [Fact]
        public void BorderInfo_Tests()
        {
            var actual = PdfReport.PdfReportPageProcessor.BorderInfo(new[]
            {
                null, new Report.Model.Border
                {
                    Top = new Report.Model.GraphInfo() {LineWidth = 5}
                }, new Report.Model.Border
                {
                    Top = new Report.Model.GraphInfo() {LineWidth = null, Color = "red"},
                    Bottom = new Report.Model.GraphInfo() {LineWidth = 1, Color = "green"},
                    Left = new Report.Model.GraphInfo() {LineWidth = 2, Color = "blue"},
                    Right = new Report.Model.GraphInfo() {LineWidth = 3, Color = "black"}
                }
            });
            Assert.Equal(5, actual.Top.LineWidth);
            Assert.Equal(3, actual.Right.LineWidth);
            Assert.Equal(2, actual.Left.LineWidth);
            Assert.Equal(new PdfModel.Color(255, 0, 0, 255), actual.Left.Color);
        }

        [Fact]
        public void BorderInfo_Tests02()
        {
            var actual = PdfReport.PdfReportPageProcessor.BorderInfo(new[]
            {
                null, new Report.Model.Border
                {
                    Top = new Report.Model.GraphInfo() {LineWidth = 5},
                    Bottom = new Report.Model.GraphInfo() {LineWidth = 3}
                }, new Report.Model.Border
                {
                    All = new Report.Model.GraphInfo() {LineWidth = 2, Color = "blue"},
                }
            });
            Assert.Equal(5, actual.Top.LineWidth);
            Assert.Equal(2, actual.Right.LineWidth);
            Assert.Equal(2, actual.Left.LineWidth);
            Assert.Equal(3, actual.Bottom.LineWidth);
            Assert.Equal(new PdfModel.Color(255, 0, 0, 255), actual.Top.Color);
            Assert.Equal(new PdfModel.Color(255, 0, 0, 255), actual.Bottom.Color);
            Assert.Equal(new PdfModel.Color(255, 0, 0, 255), actual.Left.Color);
            Assert.Equal(new PdfModel.Color(255, 0, 0, 255), actual.Right.Color);
        }

        [Fact]
        public void BorderInfo_Tests03()
        {
            var actual = PdfReport.PdfReportPageProcessor.BorderInfo(new Report.Model.Border
                {
                    Top = new Report.Model.GraphInfo() {LineWidth = 5, Color = "red" },
                    Bottom = new Report.Model.GraphInfo() {LineWidth = 3},
                    All = new Report.Model.GraphInfo() { LineWidth = 2, Color = "blue" },
            }
            );
            Assert.Equal(5, actual.Top.LineWidth);
            Assert.Equal(2, actual.Right.LineWidth);
            Assert.Equal(2, actual.Left.LineWidth);
            Assert.Equal(3, actual.Bottom.LineWidth);
            Assert.Equal(new PdfModel.Color(255, 255, 0, 0), actual.Top.Color);
            Assert.Equal(new PdfModel.Color(255, 0, 0, 255), actual.Bottom.Color);
            Assert.Equal(new PdfModel.Color(255, 0, 0, 255), actual.Left.Color);
            Assert.Equal(new PdfModel.Color(255, 0, 0, 255), actual.Right.Color);
        }

        [Fact]
        public void Margin_Tests()
        {
            var actual = PdfReport.PdfReportPageProcessor.MarginInfo(new Margin()
                {
                    Top = 5,
                    Bottom = 6,
                }, new Margin()
                {
                    Left = 1,
                    Top = 2,
                    Right = 3,
                    Bottom = 4
                }
            );
            Assert.Equal(5, actual.Top);
            Assert.Equal(6, actual.Bottom);
            Assert.Equal(1, actual.Left);
            Assert.Equal(3, actual.Right);
        }

        [Fact]
        public async void Font_Tests()
        {
            var processor = await GetPageProcessor();
            var actual = processor.GetFontState(new Font()
            {
                Name = "name1",
                Size = 10,
                Style = "italic"
            });
            Assert.Equal("name1", actual.Font);
            Assert.Equal(10, actual.FontSize);
            Assert.Equal(PdfModel.FontStyles.Italic, actual.FontStyle);
        }


        [Fact]
        public async void Location_Tests()
        {
            var processor = await GetPageProcessor();
            var start_ll = new Location() { LLX = 10, LLY = 10, URX = 100, URY = 100};
            Assert.True(start_ll.EmptyTop());
            Assert.False(start_ll.EmptyLow());

            var start_ur = new Location() { Left = 20, Top = 20, Right = 200, Bottom = 200};

            Assert.False(start_ur.EmptyTop());
            Assert.True(start_ur.EmptyLow());


            Assert.Equal(10, processor.Llx(start_ll));
            Assert.Equal(100, processor.Urx(start_ll));
            Assert.Equal(10, processor.Lly(start_ll));
            Assert.Equal(100, processor.Ury(start_ll));

            Assert.Equal(20, processor.Llx(start_ur));
            Assert.Equal(200, processor.Urx(start_ur));
            Assert.Equal(processor._parent._dims.MaxY - 200, processor.Lly(start_ur));
            Assert.Equal(processor._parent._dims.MaxY - 20, processor.Ury(start_ur));

            var start_ur_neg = new Location() { Left = -200, Top = -200, Right = -20, Bottom = -20 };
            Assert.Equal(processor._parent._dims.MaxX - 200, processor.Llx(start_ur_neg));
            Assert.Equal(processor._parent._dims.MaxX - 20, processor.Urx(start_ur_neg));
            Assert.Equal(20, processor.Lly(start_ur_neg));
            Assert.Equal(200, processor.Ury(start_ur_neg));

        }

        [Fact]
        public async void Rect_Tests()
        {
            var processor = await GetPageProcessor();

            var actual = processor.Rect(new Location() { LLX = 10, LLY = 10, URX = 100, URY = 100 });
            Assert.Equal(10, actual.LLX);
            Assert.Equal(10, actual.LLY);
            Assert.Equal(100, actual.URX);
            Assert.Equal(100, actual.URY);

            actual = processor.Rect(new Location() { Left = 20, Top = 20, Right = 200, Bottom = 200 });
            Assert.Equal(20, actual.LLX);
            Assert.Equal(processor._parent._dims.MaxY - 200, actual.LLY);
            Assert.Equal(200, actual.URX);
            Assert.Equal(processor._parent._dims.MaxY - 20, actual.URY);
        }

        [Fact]
        public async void CreateTable_Tests()
        {
            var processor = await GetPageProcessor();

            var actual = await processor.CreateTable(new PageContentItem()
            {
                Rows = new List<TableRow>()
                {
                    new TableRow(){Cells = new List<TableCell>()
                    {
                        new TableCell() {Text = "cell 11", HorizontalAlignment = "left", VerticalAlignment = "top"},
                        new TableCell() {Text = "cell 12", HorizontalAlignment = "right", VerticalAlignment = "bottom"}
                    }},
                    new TableRow(){Cells = new List<TableCell>()
                    {
                        new TableCell() {TextLines = new List<string>(){"cell 21", "data 21"}, HorizontalAlignment = "left", VerticalAlignment = "top", ColSpan = 2, RowSpan = 3},
                        new TableCell() {TextLines = new List<string>(){"cell 22", "data 22"}, VerticalAlignment = "bottom", ColSpan = 4, RowSpan = 5},
                        new TableCell() {TextLines = new List<string>(){"cell 23", "data 23"}, VerticalAlignment = "bottom", Border = new Report.Model.Border() {All = new Report.Model.GraphInfo() {LineWidth = 2, Color = "red"}}}
                    }}
                }
            });

            Assert.Equal(2, actual.Rows.Count);
            
            // 1-1
            Assert.Equal(2, actual.Rows[0].Cells.Count);
            Assert.Equal(PdfModel.HorizontalAlignment.Left, actual.Rows[0].Cells[0].Alignment);
            Assert.Equal(PdfModel.VerticalAlignment.Top, actual.Rows[0].Cells[0].VerticalAlignment);
            Assert.Single(actual.Rows[0].Cells[0].Paragraphs);
            Assert.Equal("cell 11", actual.Rows[0].Cells[0].Paragraphs[0].Text);

            // 1-2
            Assert.Equal(PdfModel.HorizontalAlignment.Right, actual.Rows[0].Cells[1].Alignment);
            Assert.Equal(PdfModel.VerticalAlignment.Bottom, actual.Rows[0].Cells[1].VerticalAlignment);
            Assert.Single(actual.Rows[0].Cells[1].Paragraphs);
            Assert.Equal("cell 12", actual.Rows[0].Cells[1].Paragraphs[0].Text);
            
            // 2-1
            Assert.Equal(3, actual.Rows[1].Cells.Count);
            Assert.Equal(PdfModel.HorizontalAlignment.Left, actual.Rows[1].Cells[0].Alignment);
            Assert.Equal(PdfModel.VerticalAlignment.Top, actual.Rows[1].Cells[0].VerticalAlignment);
            Assert.Equal(2, actual.Rows[1].Cells[0].ColSpan);
            Assert.Equal(3, actual.Rows[1].Cells[0].RowSpan);
            Assert.Equal(2, actual.Rows[1].Cells[0].Paragraphs.Count);
            Assert.Equal("cell 21", actual.Rows[1].Cells[0].Paragraphs[0].Text);
            Assert.Equal("data 21", actual.Rows[1].Cells[0].Paragraphs[1].Text);

            // 2-2
            Assert.Equal(PdfModel.VerticalAlignment.Bottom, actual.Rows[1].Cells[1].VerticalAlignment);
            Assert.Equal(2, actual.Rows[1].Cells[1].Paragraphs.Count);
            Assert.Equal("cell 22", actual.Rows[1].Cells[1].Paragraphs[0].Text);
            Assert.Equal("data 22", actual.Rows[1].Cells[1].Paragraphs[1].Text);
            
            // 2-3
            Assert.Equal(PdfModel.VerticalAlignment.Bottom, actual.Rows[1].Cells[2].VerticalAlignment);
            Assert.Equal(2, actual.Rows[1].Cells[2].Paragraphs.Count);
            Assert.Equal("cell 23", actual.Rows[1].Cells[2].Paragraphs[0].Text);
            Assert.Equal("data 23", actual.Rows[1].Cells[2].Paragraphs[1].Text);

            Assert.Equal(2, actual.Rows[1].Cells[2].Border.Top.LineWidth);
            Assert.Equal(2, actual.Rows[1].Cells[2].Border.Bottom.LineWidth);
            Assert.Equal(new PdfModel.Color(255, 255, 0, 0), actual.Rows[1].Cells[2].Border.Top.Color);
            Assert.Equal(new PdfModel.Color(255, 255, 0, 0), actual.Rows[1].Cells[2].Border.Bottom.Color);

        }


        /*public bool IsEqual(double? a, double b)
        {
            double MyEpsilon(int n) => Math.Pow(10, -(n + 1));

            // Avoiding division by zero
            if (Math.Abs(a.Value) <= double.Epsilon || Math.Abs(b) <= double.Epsilon)
                return Math.Abs(a.Value - b) <= double.Epsilon;

            // Comparison
            return Math.Abs(1.0 - a.Value / b) <= MyEpsilon(5);
        }*/
        public bool IsEqual(double? a, int b)
        {
            return Convert.ToInt32(Math.Truncate(a.Value)) == b;
        }

        [Fact]
        public async void AddContentItem_Text_Tests()
        {
            var processor = await GetPageProcessor();
            
            await processor.AddContentItem(1, new PageContentItem()
            {
                Text = "mock text",
                Location = new Location() { Left = 20, Top = 20, Right = 200, Bottom = 200 }
            });
            PdfApiMock.Verify(e => e.PutAddTextAsync("file_01.pdf", 1, It.Is<PdfModel.Paragraph>(p =>
                p.Lines.Exists(l => l.Segments.Exists(s => s.Value == "mock text")) 
                && IsEqual(p.Rectangle.LLX, 20) && IsEqual(p.Rectangle.LLY, 640) && IsEqual(p.Rectangle.URX, 200) && IsEqual(p.Rectangle.URY, 820)
                ),"a/b/c", null), Times.Exactly(1));
        }
        [Fact]
        public async void AddContentItem_Url_Tests()
        {
            var processor = await GetPageProcessor();

            await processor.AddContentItem(1, new PageContentItem()
            {
                Url = "file://issue-link-qr?link=123",
                Location = new Location() { Left = 20, Top = 20, Right = 200, Bottom = 200 }
            });
            PdfApiMock.Verify(e => e.PostInsertImageAsync("file_01.pdf", 1
                , It.Is<double>(m => IsEqual(m, 20))
                , It.Is<double>(m => IsEqual(m, 640))
                , It.Is<double>(m => IsEqual(m, 200))
                , It.Is<double>(m => IsEqual(m, 820))
                , It.IsAny<string>(), null, "a/b/c", It.IsAny<Stream>()), Times.Exactly(1));
        }

        [Fact]
        public async void AddContentItem_Rows_Tests()
        {
            var processor = await GetPageProcessor();

            await processor.AddContentItem(1, new PageContentItem()
            {
                Rows = new List<TableRow>()
                {
                    new TableRow(){Cells = new List<TableCell>()
                    {
                        new TableCell() {Text = "cell 11", HorizontalAlignment = "left", VerticalAlignment = "top"},
                        new TableCell() {Text = "cell 12", HorizontalAlignment = "right", VerticalAlignment = "bottom"}
                    }},
                    new TableRow(){Cells = new List<TableCell>()
                    {
                        new TableCell() {TextLines = new List<string>(){"cell 21", "data 21"}, HorizontalAlignment = "left", VerticalAlignment = "top", ColSpan = 2, RowSpan = 3},
                        new TableCell() {TextLines = new List<string>(){"cell 22", "data 22"}, VerticalAlignment = "bottom", ColSpan = 4, RowSpan = 5},
                        new TableCell() {TextLines = new List<string>(){"cell 23", "data 23"}, VerticalAlignment = "bottom", Border = new Report.Model.Border() {All = new Report.Model.GraphInfo() {LineWidth = 2, Color = "red"}}}
                    }}
                }
            });
            PdfApiMock.Verify(e => e.PostPageTablesAsync("file_01.pdf", 1, It.Is<List<PdfModel.Table>>( ts =>
                    ts.Count == 1 && ts[0].Rows.Count == 2 
                        && ts[0].Rows[0].Cells[0].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[0].Paragraphs[0].Text == "cell 11" 
                            && ts[0].Rows[0].Cells[0].Alignment == PdfModel.HorizontalAlignment.Left && ts[0].Rows[0].Cells[0].VerticalAlignment == PdfModel.VerticalAlignment.Top
                        && ts[0].Rows[0].Cells[1].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[1].Paragraphs[0].Text == "cell 12" 
                            && ts[0].Rows[0].Cells[1].Alignment == PdfModel.HorizontalAlignment.Right && ts[0].Rows[0].Cells[1].VerticalAlignment == PdfModel.VerticalAlignment.Bottom

                        && ts[0].Rows[1].Cells[0].Paragraphs.Count == 2 && ts[0].Rows[1].Cells[0].Paragraphs[0].Text == "cell 21" && ts[0].Rows[1].Cells[0].Paragraphs[1].Text == "data 21"
                            && ts[0].Rows[1].Cells[0].Alignment == PdfModel.HorizontalAlignment.Left && ts[0].Rows[1].Cells[0].VerticalAlignment == PdfModel.VerticalAlignment.Top
                            && ts[0].Rows[1].Cells[0].ColSpan == 2 && ts[0].Rows[1].Cells[0].RowSpan == 3
                        && ts[0].Rows[1].Cells[1].Paragraphs.Count == 2 && ts[0].Rows[1].Cells[1].Paragraphs[0].Text == "cell 22" && ts[0].Rows[1].Cells[1].Paragraphs[1].Text == "data 22"
                            && ts[0].Rows[1].Cells[1].VerticalAlignment == PdfModel.VerticalAlignment.Bottom
                            && ts[0].Rows[1].Cells[1].ColSpan == 4 && ts[0].Rows[1].Cells[1].RowSpan == 5
                        && ts[0].Rows[1].Cells[2].Paragraphs.Count == 2 && ts[0].Rows[1].Cells[2].Paragraphs[0].Text == "cell 23" && ts[0].Rows[1].Cells[2].Paragraphs[1].Text == "data 23"
                            && ts[0].Rows[1].Cells[2].VerticalAlignment == PdfModel.VerticalAlignment.Bottom
                            && IsEqual(ts[0].Rows[1].Cells[2].Border.Top.LineWidth, 2) && ts[0].Rows[1].Cells[2].Border.Top.Color.Equals(new PdfModel.Color(255, 255, 0, 0))
                            && IsEqual(ts[0].Rows[1].Cells[2].Border.Bottom.LineWidth, 2) && ts[0].Rows[1].Cells[2].Border.Bottom.Color.Equals(new PdfModel.Color(255, 255, 0, 0))
                            && IsEqual(ts[0].Rows[1].Cells[2].Border.Left.LineWidth, 2) && ts[0].Rows[1].Cells[2].Border.Left.Color.Equals(new PdfModel.Color(255, 255, 0, 0))
                            && IsEqual(ts[0].Rows[1].Cells[2].Border.Right.LineWidth, 2) && ts[0].Rows[1].Cells[2].Border.Right.Color.Equals(new PdfModel.Color(255, 255, 0, 0))
                ), null, "a/b/c"), Times.Exactly(1));

        }

        [Fact]
        public async void Report_Tests()
        {
            var processor = await GetPageProcessor();

            var result = await processor.Report(new Report.Model.ContentObject
            {
                Page = new List<PageContentItem>()
                {
                    new PageContentItem()
                    {
                        Text = "mock text",
                        Location = new Location() { Left = 10, Top = 20, Right = 300, Bottom = 400 }
                    }, new PageContentItem()
                    {
                        Url = "file://issue-link-qr?link=123",
                        Location = new Location() { Left = 50, Top = 60, Right = 700, Bottom = 800 }
                    }, new PageContentItem()
                    {
                        Rows = new List<TableRow>()
                        {
                            new TableRow(){Cells = new List<TableCell>()
                            {
                                new TableCell() {Text = "cell 11"},
                                new TableCell() {Text = "cell 12"}
                            }}
                        }
                    }
                }
            });
            Assert.True(result, "Invalid Report result");
            PdfApiMock.Verify(e => e.PutAddTextAsync("file_01.pdf", 1, It.Is<PdfModel.Paragraph>(p =>
                p.Lines.Exists(l => l.Segments.Exists(s => s.Value == "mock text"))
                && IsEqual(p.Rectangle.LLX, 10) && IsEqual(p.Rectangle.LLY, 440) && IsEqual(p.Rectangle.URX, 300) && IsEqual(p.Rectangle.URY, 820)
            ), "a/b/c", null), Times.Exactly(1));

            PdfApiMock.Verify(e => e.PostInsertImageAsync("file_01.pdf", 1
                , It.Is<double>(m => IsEqual(m, 50))
                , It.Is<double>(m => IsEqual(m, 40))
                , It.Is<double>(m => IsEqual(m, 700))
                , It.Is<double>(m => IsEqual(m, 780))
                , It.IsAny<string>(), null, "a/b/c", It.IsAny<Stream>()), Times.Exactly(1));
            
            PdfApiMock.Verify(e => e.PostPageTablesAsync("file_01.pdf", 1, It.Is<List<PdfModel.Table>>(ts =>
                   ts.Count == 1 && ts[0].Rows.Count == 1
                       && ts[0].Rows[0].Cells[0].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[0].Paragraphs[0].Text == "cell 11"
                       && ts[0].Rows[0].Cells[1].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[1].Paragraphs[0].Text == "cell 12"
                ), null, "a/b/c"), Times.Exactly(1));
        }
    }
}
