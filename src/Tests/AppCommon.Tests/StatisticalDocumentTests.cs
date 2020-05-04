using System;
using Xunit;

namespace Aspose.Cloud.Marketplace.Common.Tests
{
    [Trait("AppCommon", "StatisticalDocument")]
    public class StatisticalDocumentTests
    {
        [Fact]
        public void StatisticalDocument_CreationTest()
        {
            var sd = new StatisticalDocument()
            {
                Call = "testcall",
                Comment = "testcomment",
                ElapsedSeconds = 23
            };

            Assert.Equal("testcall", sd.Call);
            Assert.Equal("testcomment", sd.Comment);
            Assert.Equal(23, sd.ElapsedSeconds);
        }
    }
}
