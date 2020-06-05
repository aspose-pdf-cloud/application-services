using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aspose.Pdf.Cloud.Sdk.Api;
using Aspose.Pdf.Cloud.Sdk.Model;
using Aspose.BarCode.Cloud.Sdk.Interfaces;
using Aspose.Cloud.Marketplace.Common;

[assembly: InternalsVisibleTo("acm.PdfExporter.Tests")]
namespace Aspose.Cloud.Marketplace.Report
{
    /// <summary>
    /// Perform pdf report based on the report model
    /// </summary>
    public partial class PdfReport : IReport<IPdfApi, IBarcodeApi>
    {
        public struct Dims
        {
            public double Height { get; set; }
            public double Width { get; set; }

            public double mm2points(double mm) => 2.83f * mm;

            public double MaxX => mm2points(Width);
            public double MaxY => mm2points(Height);

            public double points(int pixels, double dpi) => pixels * 72 / dpi;
        }
        public static Dims A4 = new Dims() { Width = 210, Height = 297 };
        internal readonly Dims _dims = A4;
        internal IPdfApi _pdfApi;
        internal IBarcodeApi _barcodeApi;
        //private Configuration _pdfConfig;
        //private Aspose.BarCode.Cloud.Sdk.Configuration _barcodeConfig;
        internal Model.Font _defaultFont;
        internal Model.DocumentOptions _documentOptions, _defaultDocumentOptions;
        internal bool _debug;
        internal string _templateFilePath;
        internal string _templateStorageName;
        internal string _filePath, _fileName;
        internal string _folderName;
        internal string _storageName;
        internal string _tmpFolderPath;
        internal string _tmpFolderName;
        TaskMeasurer _taskMeasurer;
        public List<StatisticalDocument> Stat => _taskMeasurer?.Stat;
        public string TmpFolderName => _tmpFolderName;
        public string TmpFolderPath => _tmpFolderPath;
        public string FileName => _fileName;
        public string Folder => _folderName;
        public string FilePath => _filePath;
        public string Storage => _storageName;

        public PdfReport(Dims dims, string filePath = null, string storageName = null
            , string templateFilePath = null, string templateStorageName = null, bool debug = false)
        {
            _dims = dims;
            _defaultFont = null;
            _debug = debug;
            _taskMeasurer = new TaskMeasurer();
            _defaultDocumentOptions = new Model.DocumentOptions()
            {
                //GenerateQRCode = true
            };

            _templateFilePath = templateFilePath;
            _templateStorageName = templateStorageName;

            _storageName = storageName;
            _filePath = filePath;
            // 
            if (!string.IsNullOrEmpty(_filePath))
            {
                var m = Regex.Match(_filePath, "^(.*)/([^/]*)$");
                if (m.Success)
                {
                    _folderName = m.Groups.Count > 1 ? m.Groups[1].Value : "";
                    _fileName = m.Groups.Count > 2 ? m.Groups[2].Value : "";
                }
            }
            //_fileName = Path.GetFileName(_filePath);
            //_folderName = Path.GetDirectoryName(_filePath);

            if (string.IsNullOrEmpty(_folderName))
                _folderName = null;
            if (string.IsNullOrEmpty(_storageName))
                _storageName = null;

            _tmpFolderName = $"tmp_{FileName}";
            if (!string.IsNullOrEmpty(Folder))
                _tmpFolderPath = $"{Folder}/{_tmpFolderName}";
        }

        public PdfReport(string filePath = null, string storageName = null
            , string templateFilePath = null, string templateStorageName = null, bool debug = false) : this(A4, filePath, storageName, templateFilePath, templateStorageName, debug)
        {
        }

        public async Task Download(Stream outputStream)
        {
            await using(Stream s = await _pdfApi.DownloadFileAsync(path: FilePath, storageName: Storage))
                await s.CopyToAsync(outputStream);
        }
        public async Task Download(IPdfApi pdfApi, Stream outputStream, string filePath, string storageName = null)
        {
            await using (Stream s = await pdfApi.DownloadFileAsync(path: filePath, storageName: storageName))
                await s.CopyToAsync(outputStream);
        }
        public async Task CreateFolder(string folderName)
        {
            // create subfolders first

            string[] folderParts = Folder.Split(new char[] { '/' });
            for (int i = 0; i < folderParts.Count(); i++)
            {
                await CreateFolderPath(string.Join("/", folderParts.Take(1 + i)));
            }

            if (!string.IsNullOrEmpty(folderName))
                await CreateFolderPath(string.Join("/", new string[] { Folder, folderName }.Where(x => x != null).ToArray()));
        }

        public async Task CreateFolderPath(string folderPath)
        {
            //Trace.WriteLine($"CreateFolderPath {DateTime.Now} {folderPath} >");
            try
            {
                var exists = await _pdfApi.ObjectExistsAsync(path: folderPath, storageName: Storage);
                if (!exists?.Exists ?? false)
                    await _pdfApi.CreateFolderAsync(path: folderPath, storageName: Storage);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"CreateFolderPath ex {ex} <");
            }
            //Trace.WriteLine($"CreateFolderPath {DateTime.Now} {folderPath} <");
        }

        public async Task RemoveFolder(string folderName)
        {
            await _pdfApi.DeleteFolderAsync(path: string.Join("/", new string[] { Folder, folderName }.Where(x => x != null).ToArray())
                , storageName: Storage, recursive: true);
        }

        public async Task RemoveFile(string filePath)
        {
            await _pdfApi.DeleteFileAsync(path: filePath, storageName: Storage);
        }
        public async Task<bool> Configure(IPdfApi pdfApi, IBarcodeApi barcodeApi)
        {
            _pdfApi = pdfApi;
            _barcodeApi = barcodeApi;

            await _taskMeasurer.Run(async () =>
            {
                await CreateFolder("");
            }, "000_CreateClientFolder");

            await _taskMeasurer.Run(async () =>
            {
                await CreateFolder(TmpFolderName);
            }, "000_CreateTemporaryFolder");

            return true;
        }
        /*
        public async Task<bool> Configure(string apiKey, string appSid, string basePath = "")
        {
            _pdfConfig = string.IsNullOrEmpty(basePath) ? new Configuration(apiKey, appSid) : new Configuration(apiKey, appSid, basePath);
            _pdfApi = new PdfApi(_pdfConfig);

            _barcodeConfig = new Aspose.BarCode.Cloud.Sdk.Configuration();
            _barcodeConfig.DebugMode = _debug;
            _barcodeConfig.AppKey = apiKey;
            _barcodeConfig.AppSid = appSid;
            if (!string.IsNullOrEmpty(basePath))
                _barcodeConfig.ApiBaseUrl = basePath;
            _barcodeApi = new Aspose.BarCode.Cloud.Sdk.BarCodeApi(_barcodeConfig);

            await _taskMeasurer.Run(async () =>
            {
                await CreateFolder("");
            }, "000_CreateClientFolder");

            await _taskMeasurer.Run(async () =>
            {
                await CreateFolder(TmpFolderName);
            }, "000_CreateTemporaryFolder");

            return true;
        }
        */

        public async Task<bool> Report(Model.Document model)
        {
            _defaultFont = model.DefaultFont;
            _documentOptions = model.Options;
            if (null == model?.Content || 0 == model?.Content?.Count)
                throw new ArgumentException("No issues to export");
            if (model?.Content?.Count == 1) // single issue
            {
                PdfReportPageProcessor processor = new PdfReportPageProcessor(this, FileName, Folder);
                await processor.PrepareDocument();
                await processor.Report(model.Content[0]);
            } else // multiple issues
            {
                List<PdfReportPageProcessor> pageProcessors = model.Content.Select(i => new PdfReportPageProcessor(this, $"{Guid.NewGuid()}.pdf", TmpFolderPath)).ToList();
                await Task.WhenAll(pageProcessors.Select(async (p) =>
                {
                    await p.PrepareDocument();
                    await p.Report(model.Content[pageProcessors.IndexOf(p)]);
                }));
                await _taskMeasurer.Run(async () =>
                    await _pdfApi.PutMergeDocumentsAsync(mergeDocuments: new MergeDocuments(pageProcessors.Select(p => $"{TmpFolderPath}/{p.FileName}").ToList())
                    , name: FileName, storage: Storage, folder: Folder)
                , "990_MergeDocuments");
            }

            await _taskMeasurer.Run(async () =>
            {
                await RemoveFolder(TmpFolderName);
            }, "990_DeleteTemporaryFolder");
            return true;
        }

    }
}
