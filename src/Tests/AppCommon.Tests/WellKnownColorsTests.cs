using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Aspose.Cloud.Marketplace.Common.Tests
{
    [Trait("AppCommon", "WellKnownColors")]
    public class WellKnownColorsTests
    {
        [Fact]
        public void WellKnownColorsTests_GetTest()
        {
            Assert.Equal("#FF5F9EA0", WellKnownColors.FindColor("cadetblue"));
        }
    }
}
