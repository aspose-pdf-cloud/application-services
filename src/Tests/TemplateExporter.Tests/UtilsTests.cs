using System;
using Xunit;
using Aspose.Cloud.Marketplace.Report.Utils;

namespace Aspose.Cloud.Marketplace.App.TemplateExporter.Tests
{
    [Trait("TemplateExporterLibrary", "Utils_Tests")]
    public class Utils_Tests
    {
        enum ETest {
            def,
            EnumItem1,
            EnumItem2
        }
        [Fact]
        public void Extensions_ToEnum_Test()
        {
            Assert.Equal(ETest.EnumItem1, "EnumItem1".ToEnum(ETest.def));
            Assert.Equal(ETest.EnumItem2, "EnumItem2".ToEnum(ETest.def));

            Assert.Equal(ETest.def, "EnumItem NA".ToEnum(ETest.def));
            Assert.Equal(ETest.def, string.Empty.ToEnum(ETest.def));
        }
    }
}
