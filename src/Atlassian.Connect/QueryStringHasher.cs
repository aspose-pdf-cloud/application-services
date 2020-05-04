using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect
{
    /// <summary>
    /// Thanks to https://bitbucket.org/atlassianlabs/atlassian-connect-.net/src/master/Atlassian.Connect/Internal/QueryStringHasher.cs
    /// Ported to .net standart
    /// </summary>
    public class QueryStringHasher
    {
        public static string GenerateCanonicalRequest(string method, string path, string queryString = "")
        {
            var result = new StringBuilder();

            // method
            result.Append(method.ToUpperInvariant());

            // path
            result.Append("&");
            if (string.IsNullOrEmpty(path))
            {
                path = "/";
            }
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            result.Append(path);

            // query string
            var queryStringItems = QueryHelpers.ParseQuery(queryString);
            var canonicalQueryString = String.Join("&", queryStringItems.Keys
                .Where(x => x != "jwt") // ignore JWT parameter, if there is one
                .Select(x => new KeyValuePair<string, string[]>(x, queryStringItems.GetValueOrDefault(x).ToArray())) // turn into KVP
                .OrderBy(x => x.Key)
                .Select(x => $"{x.Key}={string.Join(",", x.Value.OrderBy(i => i).Select(EscapeUriDataStringRfc3986))}"
                ));

            result.Append("&");
            if (!string.IsNullOrEmpty(canonicalQueryString))
            {
                result.Append(canonicalQueryString);
            }

            return result.ToString();
        }



        public static string CalculateHash(string method, string path, string queryString)
        {
            return CalculateHash(GenerateCanonicalRequest(method, path, queryString));
        }

        public static string CalculateHash(string canonicalRequest)
        {
            using var sha = SHA256.Create();
            var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes(canonicalRequest));
            return BitConverter.ToString(computedHash).Replace("-", "").ToLower();
        }

        // from http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986
        /// <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = { "!", "*", "'", "(", ")" };

        /// <summary>
        /// Escapes a string according to the URI data string rules given in RFC 3986.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <remarks>
        /// The <see cref="Uri.EscapeDataString"/> method is <i>supposed</i> to take on
        /// RFC 3986 behavior if certain elements are present in a .config file.  Even if this
        /// actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
        /// host actually having this configuration element present.
        /// </remarks>
        internal static string EscapeUriDataStringRfc3986(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            foreach (var t in UriRfc3986CharsToEscape)
                escaped.Replace(t, Uri.HexEscape(t.First()));
            
            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }
    }
}
