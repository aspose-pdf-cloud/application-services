namespace Aspose.Cloud.Marketplace.Common
{
    /// <summary>
    /// Statistical document model
    /// </summary>
    public class StatisticalDocument
    {
        public string Call { get; set; }
        public double? ElapsedSeconds { get; set; }
        public string Comment { get; set; }
        public override string ToString()
        {
            return $"{Call}: {ElapsedSeconds} ({Comment})";
        }
    }
}
