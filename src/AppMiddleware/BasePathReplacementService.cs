using System;

namespace Aspose.Cloud.Marketplace.Services
{
    /// <summary>
    /// Path replacement interface
    /// </summary>
    public interface IBasePathReplacement
    {
        string ReplaceBaseUrl(string url);
        Uri ReplaceBaseUrl(Uri url);
    }
    /// <summary>
    /// Replaces base url for the given url
    /// </summary>
    public class BasePathReplacementService : IBasePathReplacement
    {
        protected Uri _baseUri;

        public BasePathReplacementService(string baseUrl = null)
        {
            _baseUri = string.IsNullOrEmpty(baseUrl) ? null : new Uri(baseUrl);
        }

        public Uri ReplaceBaseUrl(Uri url) =>
            null == _baseUri ? url : new Uri(_baseUri, url.PathAndQuery);

        public string ReplaceBaseUrl(string url) => null == url ? null : ReplaceBaseUrl(new Uri(url)).ToString();
    }
}
