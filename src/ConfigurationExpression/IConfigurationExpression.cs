namespace Aspose.Cloud.Marketplace.Services
{
    /// <summary>
    /// Configuration expression interface
    /// </summary>
    public interface IConfigurationExpression
    {
        string this[string index] { get; }
        string Get(string name, string @default = null);
        string Evaluate(string value);
    }
}
