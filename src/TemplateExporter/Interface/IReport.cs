using System.IO;
using System.Threading.Tasks;

namespace Aspose.Cloud.Marketplace.Report
{
    public interface IReport<ProductApi, BarcodeApi>
    {
        string FileName { get; }
        string Folder { get; }
        string FilePath { get; }
        string Storage { get; }

        Task<bool> Report(Model.Document model);
        Task<bool> Configure(ProductApi pdfApi, BarcodeApi barcodeApi);

        Task Download(Stream outputStream);
        Task Download(ProductApi pdfApi, Stream outputStream, string filePath, string storageName = null);

        Task CreateFolder(string folderName);
        Task RemoveFolder(string folderName);
        Task RemoveFile(string filePath);
    }
}
