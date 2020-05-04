namespace Aspose.Cloud.Marketplace.Report
{
    interface IReportModel
    {
        bool GenerateQRCode { get; }
        Model.Document CreateReportModel(dynamic data);
    }
}
