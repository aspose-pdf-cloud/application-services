using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aspose.Cloud.Marketplace.Report;
using Aspose.Cloud.Marketplace.Report.Model;
using Aspose.Pdf.Cloud.Sdk.Api;
using Aspose.Pdf.Cloud.Sdk.Model;
using Moq;
using Xunit;

namespace Aspose.Cloud.Marketplace.App.PdfExporter.Tests
{
    [Trait("PdfExporter", "PdfReport_Tests")]
    public class PdfReport_Tests : PdfReportPageProcessor_Tests
    {
        public PdfReport_Tests(PdfReportPageProcessorFixture fixture) : base(fixture)
        {
            Setup(PdfApiMock);
        }

        public static Mock<IPdfApi> Setup(Mock<IPdfApi> mock)
        {
            mock.Setup(f => f.PutMergeDocumentsAsync(It.IsAny<string>(), It.IsAny<MergeDocuments>()
                    , It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new DocumentResponse(200, "OK")));
            return mock;
        }

        [Fact]
        public void Base_Tests()
        {
            //pdfApi.Setup()
            PdfReport report = new PdfReport();
            Assert.Equal(210, report._dims.Width, 2);
            Assert.Equal(210 * 2.83f, report._dims.MaxX, 2);

            Assert.Equal(297, report._dims.Height, 2);
            Assert.Equal(297 * 2.83f, report._dims.MaxY, 2);
            Assert.False(report._debug, "expected debug = false");

            report = new PdfReport(new PdfReport.Dims { Height = 210, Width = 297 }, filePath:"tmp_file.txt");

            Assert.Equal(210, report._dims.Height, 2);
            Assert.Equal(210 * 2.83f, report._dims.MaxY, 2);

            Assert.Equal(297, report._dims.Width, 2);
            Assert.Equal(297 * 2.83f, report._dims.MaxX, 2);

            Assert.Equal("tmp_file.txt", report.FilePath);
            Assert.Null(report.Folder);
            Assert.True(string.IsNullOrEmpty(report.Folder), $"Folder must be empty, but it is {report.Folder}");

            report = new PdfReport(filePath: "a/b/c/tmp_file.txt", storageName:"default storage");
            Assert.Equal("a/b/c/tmp_file.txt", report.FilePath);
            Assert.Equal("a/b/c", report.Folder);
            Assert.Equal("tmp_file.txt", report.FileName);
            Assert.Equal("default storage", report.Storage);

            Assert.Equal("tmp_tmp_file.txt", report.TmpFolderName);
            Assert.Equal("a/b/c/tmp_tmp_file.txt", report.TmpFolderPath);
        }

        [Fact]
        public async void IReport_Configure_Tests()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");

            PdfApiMock.Verify(e => e.CreateFolderAsync("a", null), Times.Exactly(2));
            PdfApiMock.Verify(e => e.CreateFolderAsync("a/b", null), Times.Exactly(2));
            PdfApiMock.Verify(e => e.CreateFolderAsync("a/b/c", null), Times.Exactly(2));
            PdfApiMock.Verify(e => e.CreateFolderAsync("a/b/c/tmp_file.pdf", null), Times.Once);

            PdfApiMock.Verify(e => e.ObjectExistsAsync("a", null, null), Times.Exactly(2));
            PdfApiMock.Verify(e => e.ObjectExistsAsync("a/b", null, null), Times.Exactly(2));
            PdfApiMock.Verify(e => e.ObjectExistsAsync("a/b/c", null, null), Times.Exactly(2));
            PdfApiMock.Verify(e => e.ObjectExistsAsync("a/b/c/tmp_file.pdf", null, null), Times.Once);
        }

        [Fact]
        public async void IReport_Download_Tests()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");
            byte[] actualContent;
            byte[] expectedContent = {0x01, 0x02, 0x03};
            
            await using (MemoryStream ms = new MemoryStream())
            {
                await report.Download(ms);
                actualContent = ms.ToArray();
            }
            PdfApiMock.Verify(e => e.DownloadFileAsync("a/b/c/file.pdf", null, null), Times.Exactly(1));
            Assert.True(actualContent.SequenceEqual(expectedContent), "Download bytes do not match");
            
            PdfApiMock.Invocations.Clear();

            await using (MemoryStream ms = new MemoryStream())
            {
                await report.Download(PdfApiMock.Object, ms, "a/b/c/file.pdf", null);
                actualContent = ms.ToArray();
            }

            PdfApiMock.Verify(e => e.DownloadFileAsync("a/b/c/file.pdf", null, null), Times.Exactly(1));
            Assert.True(actualContent.SequenceEqual(expectedContent), "Download bytes do not match");
        }

        [Fact]
        public async void IReport_CreateFolder_Tests()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");
            
            PdfApiMock.Invocations.Clear();
            
            await report.CreateFolder("mock_test");
            PdfApiMock.Verify(e => e.CreateFolderAsync("a/b/c/mock_test", null));
        }

        [Fact]
        public async void IReport_CreateFolderPath_Tests()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");

            PdfApiMock.Invocations.Clear();

            await report.CreateFolderPath("1/2/3/mock_test");
            PdfApiMock.Verify(e => e.CreateFolderAsync("1/2/3/mock_test", null));
        }

        [Fact]
        public async void IReport_RemoveFolder_Tests()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");

            PdfApiMock.Invocations.Clear();

            await report.RemoveFolder("mock_test");
            PdfApiMock.Verify(e => e.DeleteFolderAsync("a/b/c/mock_test", null, It.IsAny<bool?>()));
        }

        [Fact]
        public async void IReport_RemoveFile_Tests()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");

            PdfApiMock.Invocations.Clear();

            await report.RemoveFile("a/b/c/file.mock");
            PdfApiMock.Verify(e => e.DeleteFileAsync("a/b/c/file.mock", null, null));
        }


        [Fact]
        public async void IReport_ReportSingle_Tests()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");

            var result = await report.Report(new Report.Model.Document()
            {
                DefaultFont = new Font {
                    Name = "name1",
                    Size = 10,
                    Style = "italic"
                }, Content = new List<ContentObject>()
                {
                    new ContentObject
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
                    }
                }
            });
            Assert.True(result, "Invalid Report result");
            Assert.Equal("name1", report._defaultFont.Name);
            Assert.Equal(10, report._defaultFont.Size);
            Assert.Equal("italic", report._defaultFont.Style);

            PdfApiMock.Verify(e => e.PostDocumentTextHeaderAsync("file.pdf", It.Is<TextHeader>(h =>
                    Regex.IsMatch(h.Value, "Generated.*by Aspose.PDF Exporter"))
                , It.IsAny<int?>(), It.IsAny<int?>(), null, "a/b/c"), Times.Once);
            PdfApiMock.Verify(e => e.PostDocumentTextFooterAsync("file.pdf", It.Is<TextFooter>(h =>
                    Regex.IsMatch(h.Value, "Powered by Aspose.PDF for Cloud, Aspose.Barcode for Cloud"))
                , It.IsAny<int?>(), It.IsAny<int?>(), null, "a/b/c"), Times.Once);

            PdfApiMock.Verify(e => e.PutAddTextAsync("file.pdf", 1, It.Is<Paragraph>(p =>
                p.Lines.Exists(l => l.Segments.Exists(s => s.Value == "mock text"))
                && IsEqual(p.Rectangle.LLX, 10) && IsEqual(p.Rectangle.LLY, 440) && IsEqual(p.Rectangle.URX, 300) && IsEqual(p.Rectangle.URY, 820)
            ), "a/b/c", null), Times.Exactly(1));

            PdfApiMock.Verify(e => e.PostInsertImageAsync("file.pdf", 1
                , It.Is<double>(m => IsEqual(m, 50))
                , It.Is<double>(m => IsEqual(m, 40))
                , It.Is<double>(m => IsEqual(m, 700))
                , It.Is<double>(m => IsEqual(m, 780))
                , It.IsAny<string>(), null, "a/b/c", It.IsAny<Stream>()), Times.Exactly(1));

            PdfApiMock.Verify(e => e.PostPageTablesAsync("file.pdf", 1, It.Is<List<Table>>(ts =>
                ts.Count == 1 && ts[0].Rows.Count == 1
                              && ts[0].Rows[0].Cells[0].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[0].Paragraphs[0].Text == "cell 11"
                              && ts[0].Rows[0].Cells[1].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[1].Paragraphs[0].Text == "cell 12"
            ), null, "a/b/c"), Times.Exactly(1));
        }

        [Fact]
        public async void IReport_ReportMultiple_Tests()
        {
            PdfReport report = new PdfReport("a/b/c/file.pdf");
            Assert.True(await report.Configure(PdfApiMock.Object, BarcodeMock.Object), "Configure should succeed");

            var result = await report.Report(new Report.Model.Document()
            {
                DefaultFont = new Font
                {
                    Name = "name1",
                    Size = 10,
                    Style = "italic"
                },
                Content = new List<ContentObject>()
                {
                    new ContentObject
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
                    }, new ContentObject
                    {
                        Page = new List<PageContentItem>()
                        {
                            new PageContentItem()
                            {
                                Text = "mock text2",
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
                                        new TableCell() {Text = "cell 11-1"},
                                        new TableCell() {Text = "cell 12-1"}
                                    }}
                                }
                            }
                        }
                    }
                }
            });
            Assert.True(result, "Invalid Report result");
            Assert.Equal("name1", report._defaultFont.Name);
            Assert.Equal(10, report._defaultFont.Size);
            Assert.Equal("italic", report._defaultFont.Style);

            PdfApiMock.Verify(e => e.PutCreateDocumentAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), null, "a/b/c/tmp_file.pdf"), Times.Exactly(2));

            PdfApiMock.Verify(e => e.PostDocumentTextHeaderAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), It.Is<TextHeader>(h =>
                    Regex.IsMatch(h.Value, "Generated.*by Aspose.PDF Exporter"))
                , It.IsAny<int?>(), It.IsAny<int?>(), null, "a/b/c/tmp_file.pdf"), Times.Exactly(2));


            PdfApiMock.Verify(e => e.PostDocumentTextHeaderAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), It.Is<TextHeader>(h =>
                    Regex.IsMatch(h.Value, "Generated.*by Aspose.PDF Exporter"))
                , It.IsAny<int?>(), It.IsAny<int?>(), null, "a/b/c/tmp_file.pdf"), Times.Exactly(2));
            PdfApiMock.Verify(e => e.PostDocumentTextFooterAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), It.Is<TextFooter>(h =>
                    Regex.IsMatch(h.Value, "Powered by Aspose.PDF for Cloud, Aspose.Barcode for Cloud"))
                , It.IsAny<int?>(), It.IsAny<int?>(), null, "a/b/c/tmp_file.pdf"), Times.Exactly(2));
            
            PdfApiMock.Verify(e => e.PutAddTextAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), 1, It.Is<Paragraph>(p =>
                p.Lines.Exists(l => l.Segments.Exists(s => s.Value == "mock text"))
                && IsEqual(p.Rectangle.LLX, 10) && IsEqual(p.Rectangle.LLY, 440) && IsEqual(p.Rectangle.URX, 300) && IsEqual(p.Rectangle.URY, 820)
            ), "a/b/c/tmp_file.pdf", null), Times.Once);
            PdfApiMock.Verify(e => e.PutAddTextAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), 1, It.Is<Paragraph>(p =>
                p.Lines.Exists(l => l.Segments.Exists(s => s.Value == "mock text2"))
                && IsEqual(p.Rectangle.LLX, 10) && IsEqual(p.Rectangle.LLY, 440) && IsEqual(p.Rectangle.URX, 300) && IsEqual(p.Rectangle.URY, 820)
            ), "a/b/c/tmp_file.pdf", null), Times.Once);

            
            PdfApiMock.Verify(e => e.PostInsertImageAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), 1
                , It.Is<double>(m => IsEqual(m, 50))
                , It.Is<double>(m => IsEqual(m, 40))
                , It.Is<double>(m => IsEqual(m, 700))
                , It.Is<double>(m => IsEqual(m, 780))
                , It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), null, "a/b/c/tmp_file.pdf", It.IsAny<Stream>()), Times.Exactly(2));
            
            PdfApiMock.Verify(e => e.PostPageTablesAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), 1, It.Is<List<Table>>(ts =>
                ts.Count == 1 && ts[0].Rows.Count == 1
                              && ts[0].Rows[0].Cells[0].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[0].Paragraphs[0].Text == "cell 11"
                              && ts[0].Rows[0].Cells[1].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[1].Paragraphs[0].Text == "cell 12"
            ), null, "a/b/c/tmp_file.pdf"), Times.Once);
            PdfApiMock.Verify(e => e.PostPageTablesAsync(It.IsRegex("[-0-9A-Fa-f]*\\.pdf"), 1, It.Is<List<Table>>(ts =>
                ts.Count == 1 && ts[0].Rows.Count == 1
                              && ts[0].Rows[0].Cells[0].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[0].Paragraphs[0].Text == "cell 11-1"
                              && ts[0].Rows[0].Cells[1].Paragraphs.Count == 1 && ts[0].Rows[0].Cells[1].Paragraphs[0].Text == "cell 12-1"
            ), null, "a/b/c/tmp_file.pdf"), Times.Once);

            PdfApiMock.Verify(e => e.PutMergeDocumentsAsync("file.pdf",  It.Is<MergeDocuments>(f => 
                f.List.Count == 2), null, "a/b/c"), Times.Once);
        }
    }
}
