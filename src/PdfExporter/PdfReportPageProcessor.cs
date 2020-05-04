using Aspose.Pdf.Cloud.Sdk.Api;
using Aspose.Pdf.Cloud.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Aspose.Cloud.Marketplace.Report.Utils;
using Aspose.Cloud.Marketplace.Common;
namespace Aspose.Cloud.Marketplace.Report
{
    public partial class PdfReport : IReport<IPdfApi, IBarcodeApi>
    {
        /// <summary>
        /// Page processor for PdfReport (reports single page)
        /// </summary>
        internal class PdfReportPageProcessor
        {
            internal readonly PdfReport _parent;
            internal Dims _dims => _parent._dims;
            internal IPdfApi _pdfApi => _parent._pdfApi;
            internal IBarcodeApi BarcodeApi => _parent._barcodeApi;
            internal Model.DocumentOptions _documentOptions => _parent._documentOptions;
            internal Model.DocumentOptions _defaultDocumentOptions => _parent._defaultDocumentOptions;
            internal Model.Font _defaultFont => _parent._defaultFont;
            private string _folder;
            internal string _templateFilePath => _parent._templateFilePath;
            internal string _templateStorageName => _parent._templateStorageName;

            TaskMeasurer _taskMeasurer => _parent._taskMeasurer;

            private string _fileName;
            public string FileName => _fileName;

            public string FilePath => $"{Folder}/{FileName}";
            private string Folder => _folder;
            private string Storage => _parent.Storage;

            // instance-specific 
            public PdfReportPageProcessor(PdfReport parent, string fileName, string folder)
            {
                _parent = parent;
                _fileName = fileName;
                _folder = folder;
            }


            public async Task PrepareDocument()
            {
                DocumentResponse documentResponse;

                if (!string.IsNullOrEmpty(_templateFilePath))
                {
                    await _taskMeasurer.Run(() => _pdfApi.CopyFileAsync(srcPath: _templateFilePath, destPath: FilePath, srcStorageName: _templateStorageName, destStorageName: Storage), "010_CopyFile", commentObj: new
                    {
                        templateFile = _templateFilePath
                    });

                    documentResponse = await _taskMeasurer.Run(() => _pdfApi.GetDocumentAsync(name: FileName, storage: Storage, folder: Folder), "011_GetDocument", commentObj: new
                    {
                        fileName = FileName
                    });
                }
                else
                {
                    documentResponse = await _taskMeasurer.Run(() => _pdfApi.PutCreateDocumentAsync(name: FileName, storage: Storage, folder: Folder), "010_CreateDocument");
                }

                await _taskMeasurer.Run(() =>
                   _pdfApi.PostDocumentTextHeaderAsync(name: FileName, storage: Storage, folder: Folder
                       , textHeader: new TextHeader(HorizontalAlignment: HorizontalAlignment.Right, RightMargin: 10
                       , Value: $"Generated {DateTime.Now.ToShortDateString()} by Aspose.PDF Exporter", TextState: new TextState(FontSize: 10d, Font: "Arial", FontStyle: FontStyles.Italic)))
                    , "040_DocumentTextHeader");
                await _taskMeasurer.Run(() =>
                    _pdfApi.PostDocumentTextFooterAsync(name: FileName, storage: Storage, folder: Folder
                    , textFooter: new TextFooter(HorizontalAlignment: HorizontalAlignment.Right, RightMargin: 10
                    , Value: "Powered by Aspose.PDF for Cloud, Aspose.Barcode for Cloud", TextState: new TextState(FontSize: 10d, Font: "Arial", FontStyle: FontStyles.Italic)))
                    , "041_DocumentTextFooter");
            }

            public async Task<bool> Report(Model.ContentObject contentObject)
            {
                /*
                var documentProperties = _api.GetDocument(fileName);

                var pageInfo = _api.GetPage(fileName, 1);
                */

                //int pagesCount = documentResponse.Document.Pages.List.Count;
                int pageNumber = 1; // we starting with pageNumber = 1
                
                foreach (var contentItem in contentObject.Page)
                {
                    await AddContentItem(pageNumber, contentItem);
                }

                return true;
            }

            internal async Task AddContentItem(int pageNumber, Model.PageContentItem contentItem)
            {
                if (contentItem == null)
                    throw new ArgumentException($"empty content item");
                if (!string.IsNullOrEmpty(contentItem?.Text) || contentItem?.TextLines?.Count > 0)
                {
                    if (contentItem.Location == null || (contentItem?.Location?.Empty() ?? false))
                        throw new ArgumentException($"no location for ${contentItem?.Text}");
                    var ha = contentItem?.HorizontalAlignment?.ToEnum(TextHorizontalAlignment.Left) ?? TextHorizontalAlignment.Left;
                    var va = contentItem?.VerticalAlignment?.ToEnum(VerticalAlignment.None) ?? VerticalAlignment.None;
                    var wrap = contentItem?.WrapMode?.ToEnum(WrapMode.NoWrap) ?? WrapMode.NoWrap;

                    List<Segment> segments = new List<Segment>();
                    if (!string.IsNullOrEmpty(contentItem?.Text))
                        segments.Add(new Segment(Value: HttpUtility.HtmlDecode(contentItem?.Text), TextState: GetFontState(contentItem?.Font)));
                    if (contentItem?.TextLines?.Count > 0)
                        segments.AddRange(contentItem?.TextLines.Select(s => new Segment(Value: HttpUtility.HtmlDecode(s), TextState: GetFontState(contentItem?.Font))));

                    var paragraph = new Paragraph(Lines: new List<TextLine>() { new TextLine(Segments: segments) },
                            Rectangle: Rect(contentItem.Location), HorizontalAlignment: ha, WrapMode: wrap, VerticalAlignment: va);
                    await _taskMeasurer.Run(() => _pdfApi.PutAddTextAsync(pageNumber: pageNumber, paragraph: paragraph, name: FileName, storage: Storage, folder: Folder)
                    , "050_AddText");
                }
                else if (!string.IsNullOrEmpty(contentItem?.Url))
                {
                    if (contentItem.Location == null || (contentItem?.Location?.Empty() ?? false))
                        throw new ArgumentException($"no location for ${contentItem.Url}");
                    string imageStorageFile = null;
                    string imageStorageFolder = Folder;
                    await using (var imageStream = new MemoryStream())
                    {
                        var uri = new Uri(contentItem.Url);

                        var queryParams = HttpUtility.ParseQueryString(uri.Query);
                        if (uri.Scheme.ToLower() == "file")
                        {
                            switch (uri.Host)
                            {
                                case "issue-link-qr":
                                    //if (_documentOptions?.GenerateQRCode ?? _defaultDocumentOptions.GenerateQRCode)
                                    {
                                        imageStorageFile = $"{Guid.NewGuid()}.png";
                                        imageStorageFolder = _parent.TmpFolderPath;
                                        var linkValues = queryParams.GetValues("link");
                                        if (null != linkValues && linkValues.Count() > 0)
                                        {
                                            await GenerateQr(imageStorageFile, linkValues[0], null, imageStorageFolder);
                                        }
                                        //var barcodeImageWidthPoints = _dims.points(barcodeImage.Width, barcodeImage.HorizontalResolution);
                                        //var barcodeImageHeightPoints = _dims.points(barcodeImage.Height, barcodeImage.VerticalResolution);
                                    }

                                    break;
                            }
                        }
                        else
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                using (var response = await client.GetAsync(contentItem.Url))
                                {
                                    response.EnsureSuccessStatusCode();

                                    await using (var stream = await response.Content.ReadAsStreamAsync())
                                    {
                                        await stream.CopyToAsync(imageStream);
                                    }
                                }
                            }

                        }
                        if (imageStream.Length > 0 || !string.IsNullOrEmpty(imageStorageFile))
                        {
                            imageStream.Position = 0;
                            string imageFilePath = imageStorageFile;
                            if (!string.IsNullOrEmpty(imageFilePath) && !string.IsNullOrEmpty(imageStorageFolder))
                                imageFilePath = $"{imageStorageFolder}/{imageStorageFile}";

                            await _taskMeasurer.Run(() => _pdfApi.PostInsertImageAsync(pageNumber: pageNumber
                                , llx: Convert.ToInt32(Math.Truncate(Llx(contentItem.Location).Value))
                                , lly: Convert.ToInt32(Math.Truncate(Lly(contentItem.Location).Value))
                                , urx: Convert.ToInt32(Math.Truncate(Urx(contentItem.Location).Value))
                                , ury: Convert.ToInt32(Math.Truncate(Ury(contentItem.Location).Value))
                                , imageFilePath: imageFilePath
                                , image: string.IsNullOrEmpty(imageStorageFile) ? imageStream : null
                                , name: FileName, storage: Storage, folder: Folder)
                            , "060_InsertImage");
                        }
                    }
                }
                else if (contentItem?.Rows != null)
                {
                    await _taskMeasurer.Run(() => _pdfApi.PostPageTablesAsync(pageNumber: pageNumber, tables: new List<Table>() { CreateTable(contentItem) }
                        , name: FileName, storage: Storage, folder: Folder)
                    , "070_AddPageTables");
                }
            }

            public double? Llx(Model.Location l)
            {
                if (l.EmptyTop())
                    return l.LLX;
                if (l.EmptyLow())
                {
                    return l.Left >= 0 ? l.Left : (_dims.MaxX + l.Left);
                }
                return null;
            }

            public double? Lly(Model.Location l)
            {
                if (l.EmptyTop())
                    return l.LLY;
                if (l.EmptyLow())
                {
                    return l.Bottom >= 0 ? (_dims.MaxY - l.Bottom) : (l.Bottom * -1);
                }
                return null;
            }

            public double? Urx(Model.Location l)
            {
                if (l.EmptyTop())
                    return l.URX;
                if (l.EmptyLow())
                {
                    return l.Right >= 0 ? l.Right : (_dims.MaxX + l.Right);
                }
                return null;
            }

            public double? Ury(Model.Location l)
            {
                if (l.EmptyTop())
                    return l.URY;
                if (l.EmptyLow())
                {
                    return l.Top >= 0 ? (_dims.MaxY - l.Top) : (l.Top * -1);
                }
                return null;
            }

            public Rectangle Rect(Model.Location l)
            {
                if (l.EmptyTop())
                    return new Rectangle(l.LLX, l.LLY, l.URX, l.URY);
                if (l.EmptyLow())
                {
                    return new Rectangle(l.Left, _dims.MaxY - l.Bottom, l.Right > 0 ? l.Right : (_dims.MaxX + l.Right), _dims.MaxY - l.Top);
                }
                return null;
            }

            public TextState GetFontState(Model.Font item)
            {
                Model.Font font = new Model.Font
                {
                    Name = GetValue(item?.Name, _defaultFont?.Name),
                    Size = GetValue(item?.Size, _defaultFont?.Size),
                    Style = GetValue(item?.Style, _defaultFont?.Style)
                };
                return new TextState(FontSize: font.Size, Font: font.Name, FontStyle: font.Style?.ToEnum(FontStyles.Regular) ?? FontStyles.Regular);
            }


            
            public Table CreateTable(Model.PageContentItem item)
            {
                if (item?.Rows == null)
                    return null;
                
                HorizontalAlignment defaultContentHorizontalAlignment = item.DefaultContentHorizontalAlignment?.ToEnum(HorizontalAlignment.None) ?? HorizontalAlignment.None;
                VerticalAlignment defaultContentVerticalAlignment = item.DefaultContentVerticalAlignment?.ToEnum(VerticalAlignment.None) ?? VerticalAlignment.None;
                List<Row> rows = new List<Row>();
                foreach (var r in item.Rows)
                {
                    List<Cell> cells = new List<Cell>();
                    foreach (var c in r.Cells)
                    {
                        List<TextRect> paragraphs = new List<TextRect>();
                        if (!string.IsNullOrEmpty(c.Text))
                            paragraphs.Add(new TextRect(HttpUtility.HtmlDecode(c.Text)));
                        if (c.TextLines != null && c.TextLines.Count > 0)
                            paragraphs.AddRange(c.TextLines.Select(l => new TextRect(HttpUtility.HtmlDecode(l))));

                        //if (paragraphs.Count == 0)
                        //    paragraphs.Add(new TextRect(""));

                        Cell cell = new Cell(Paragraphs: paragraphs)
                        {
                            DefaultCellTextState = GetFontState(c.Font),
                            RowSpan = c.RowSpan,
                            ColSpan = c.ColSpan,
                            Alignment = c.HorizontalAlignment?.ToEnum(HorizontalAlignment.None) ?? defaultContentHorizontalAlignment,
                            VerticalAlignment = c.VerticalAlignment?.ToEnum(VerticalAlignment.None) ?? defaultContentVerticalAlignment,
                            //BackgroundImageFile = "c:\\_\\code_img.png"
                        };
                        if (c.Border != null)
                            cell.Border = BorderInfo(new[] { c.Border, item.Border });
                        if (c.Margin != null)
                            cell.Margin = MarginInfo(c?.Margin, item?.DefaultContentMargin);
                        cells.Add(cell);
                    }
                    rows.Add(new Row(Cells: cells));
                }
                var max_cells = item.Rows.Select(c => c.Cells.Count).Max();
                Table table = new Table(Rows: rows);
                if (item?.DefaultContentMargin != null)
                    table.DefaultCellPadding = MarginInfo(item?.DefaultContentMargin);
                if (item?.ColumnWidths != null && item?.ColumnWidths.Count > 0)
                    table.ColumnWidths = string.Join(" ", item?.ColumnWidths);
                else
                    table.DefaultColumnWidth = "100";
                table.ColumnAdjustment = ColumnAdjustment.AutoFitToWindow;
                //table.Left = 20;
                //table.Top = 50;
                //table.ColumnWidths = string.Join(' ', Enumerable.Repeat(Convert.ToInt32(((_dims.MaxX /*- 30*/) / max_cells)).ToString(), max_cells));
                if (item.Border != null)
                {
                    table.DefaultCellBorder = BorderInfo(item.Border);
                }
                return table;
            }

            public static T GetValue<T>(T value, T defaultValue)
            {
                return value == null ? defaultValue : value;
            }

            public static MarginInfo MarginInfo(Model.Margin margin, Model.Margin defaultMargin = null)
            {
                var def = new Model.Margin()
                {
                    Left = GetValue(defaultMargin?.Left, 0),
                    Top = GetValue(defaultMargin?.Top, 0),
                    Right = GetValue(defaultMargin?.Right, 0),
                    Bottom = GetValue(defaultMargin?.Bottom, 0),
                };
                return new MarginInfo
                {
                    Left = GetValue(margin?.Left, def.Left),
                    Top = GetValue(margin?.Top, def.Top),
                    Right = GetValue(margin?.Right, def.Right),
                    Bottom = GetValue(margin?.Bottom, def.Bottom),
                };
            }

            public static BorderInfo BorderInfo(Model.Border item)
            {
                return new BorderInfo
                {
                    Top = GraphInfo(item?.Top, item?.All),
                    Right = GraphInfo(item?.Right, item?.All),
                    Bottom = GraphInfo(item?.Bottom, item?.All),
                    Left = GraphInfo(item?.Left, item?.All)
                };
            }

            public static BorderInfo BorderInfo(Model.Border[] items)
            {
                return new BorderInfo
                {
                    Top = GraphInfo(items.SelectMany(i => new [] { i?.Top, i?.All }).ToArray()),
                    Right = GraphInfo(items.SelectMany(i => new [] { i?.Right, i?.All }).ToArray()),
                    Bottom = GraphInfo(items.SelectMany(i => new [] { i?.Bottom, i?.All }).ToArray()),
                    Left = GraphInfo(items.SelectMany(i => new [] { i?.Left, i?.All }).ToArray())
                };
            }

            public static GraphInfo GraphInfo(Model.GraphInfo i, Model.GraphInfo defaultInfo)
            {
                return new GraphInfo(LineWidth: GetValue(i?.LineWidth, defaultInfo?.LineWidth), Color: HtmlColor(i?.Color ?? defaultInfo?.Color));
            }

            public static GraphInfo GraphInfo(Model.GraphInfo[] infos)
            {
                double? lineWidth = null;
                string color = null;
                foreach (var info in infos)
                {
                    if (info?.LineWidth != null && lineWidth == null)
                        lineWidth = info?.LineWidth;
                    if (info?.Color != null && color == null)
                        color = info?.Color;
                }
                return new GraphInfo(LineWidth: lineWidth, Color: HtmlColor(color));
            }

            public static Color HtmlColor(string htmlColor)
            {
                if (!htmlColor.StartsWith("#"))
                    htmlColor = WellKnownColors.FindColor(htmlColor);
                if (null == htmlColor)
                    htmlColor = "#FFFFFFFF";
                int argb = Int32.Parse(htmlColor.Replace("#", ""), NumberStyles.HexNumber);
                System.Drawing.Color col = System.Drawing.Color.FromArgb(argb);
                return new Color(col.A, col.R, col.G, col.B);
            }

            internal async Task GenerateQr(string fileName, string text, MemoryStream outputStream, string folder)
            {
                await using (var ms = new MemoryStream())
                {
                    _taskMeasurer.RunSync(() => BarcodeApi.BarCodePutBarCodeGenerateFile(new Aspose.BarCode.Cloud.Sdk.Model.Requests.BarCodePutBarCodeGenerateFileRequest(file: ms, text: text, type: "qr", format: "PNG", codeLocation: "None"/*, autoSize:"false", imageWidth: 100, imageHeight:100*/
                        , name: fileName, storage: Storage, folder: folder))
                    , "100_BarCodeGenerate");
                }
                if (outputStream != null)
                {
                    string filePath = fileName;
                    if (!string.IsNullOrEmpty(folder))
                        filePath = $"{folder}/{fileName}";
                    await using (Stream s = await _taskMeasurer.Run(() => _pdfApi.DownloadFileAsync(path:filePath, storageName:Storage), "110_DownloadBarcodeFile", commentObj: new
                    {
                        filePath = filePath
                    }))
                    {
                        await s.CopyToAsync(outputStream);
                    }
                }
            }
        }
    }
}
