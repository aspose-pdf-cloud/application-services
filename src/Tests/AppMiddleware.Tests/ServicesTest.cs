using Microsoft.AspNetCore.Http;
using System;
using Xunit;
using Aspose.Cloud.Marketplace.App.Middleware;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using Aspose.Cloud.Marketplace.Services;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;

namespace Aspose.Cloud.Marketplace.App.AppMiddleware.Tests
{
    [Trait("AppMiddleware", "Services_Tests")]
    public class Services_Tests
    {
        internal IBasePathReplacement BasePathReplacementService;
        internal string mockhost;
        internal string localPath, localPathQuery;
        internal string mockPath, mockPathQuery;
        public Services_Tests()
        {
            mockhost = "http://mock.host:1234";
            string path = "/1/2/3", pathQuery = "/1/2/3?a=mock&b=mock1";
            BasePathReplacementService = new BasePathReplacementService(mockhost);
            localPath = $"http://localhost{path}";
            localPathQuery = $"http://localhost{pathQuery}";
            mockPath = $"{mockhost}{path}";
            mockPathQuery = $"{mockhost}{pathQuery}";
        }
        [Fact]
        public void ReplaceBaseUrl_UriTest()
        {
            Assert.Equal(new Uri(mockPath),
                BasePathReplacementService.ReplaceBaseUrl(new Uri(localPath)));
            Assert.Equal(new Uri(mockPathQuery),
                BasePathReplacementService.ReplaceBaseUrl(new Uri(localPathQuery)));
        }
        [Fact]
        public void ReplaceBaseUrl_Test()
        {
            Assert.Equal(mockPathQuery,
                BasePathReplacementService.ReplaceBaseUrl(localPathQuery));
            Assert.Equal(mockPath,
                BasePathReplacementService.ReplaceBaseUrl(localPath));
        }
    }
}