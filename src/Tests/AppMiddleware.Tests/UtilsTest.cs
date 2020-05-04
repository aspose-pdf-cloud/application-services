using Microsoft.AspNetCore.Http;
using System;
using Xunit;
using Aspose.Cloud.Marketplace.App.Middleware;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;

namespace Aspose.Cloud.Marketplace.App.AppMiddleware.Tests
{
    [Trait("AppMiddleware", "Utils_Tests")]
    public class Utils_Tests
    {
        [Fact]
        public void GetRequestParameters_Test()
        {
            HttpContext c = new DefaultHttpContext();
            c.Request.Headers["device-id"] = "20317";
            c.Request.Query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                {"q1", new StringValues("q1_value") }
            });
            c.Request.Form = new FormCollection(new Dictionary<string, StringValues>()
            {
                {"f1", new StringValues("f1_value") }
            });
            RoutingFeature routingFeature = new RoutingFeature();
            routingFeature.RouteData = new RouteData( new RouteValueDictionary(new Dictionary<string, string>()
            {
                {"controller", "ctest" },
                {"action", "atest" }
            }));
            c.Features[typeof(IRoutingFeature)] = routingFeature;

            var (controller, action, parameters) = Utils.GetRequestParameters(c);

            Assert.Equal("ctest", controller);
            Assert.Equal("atest", action);
            Assert.Equal(2, parameters.Count);
            Assert.Equal("q1_value", parameters["q1"]);
            Assert.Equal("f1_value", parameters["f1"]);
        }
    }
}
