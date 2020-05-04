using Xunit;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Tests
{
    public class QueryStringHasher_Tests
    {
        [Fact]
        public void CalculateHash_Test()
        {
            var cc = QueryStringHasher.GenerateCanonicalRequest("GET", "a/b/c", "a=b");
            Assert.Equal("d860647f69c1c87236975fce4e5c206fd03d09b9ce114d1079e1cb1c1e520a5b", QueryStringHasher.CalculateHash("GET", "a/b/c", "a=b"));
        }

        [Fact]
        public void GenerateCanonicalRequest_Test()
        {
            Assert.Equal("GET&/a/b/c&a=b", QueryStringHasher.GenerateCanonicalRequest("GET", "a/b/c", "a=b"));
            Assert.Equal("GET&/a/b/c&a=b", QueryStringHasher.GenerateCanonicalRequest("GET", "a/b/c", "?a=b"));
            Assert.Equal("GET&/a/b/c&a=b", QueryStringHasher.GenerateCanonicalRequest("GET", "/a/b/c", "?a=b"));
        }
    }
}