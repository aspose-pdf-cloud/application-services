using Microsoft.Extensions.Configuration;
using System;
using Xunit;
using Moq;
using Aspose.Cloud.Marketplace.Services;

namespace Aspose.Cloud.Marketplace.App.ConfigurationExpression.Tests
{
    [Trait("ConfigurationExpression", "ElasticsearchLoggingTests")]
    public class ConfigurationExpressionServiceTest
    {
        IConfigurationExpression _config;
        UserObject _userObj;
        public class UserObject
        {
            public int intValue { get; set; }
            public string stringValue { get; set; }
        }
        public ConfigurationExpressionServiceTest()
        {
            var configuration = new Mock<IConfiguration>();
            IConfigurationSection confresult(string v)  {
                var sec = new Mock<IConfigurationSection>();
                sec.Setup(a => a.Value).Returns(v);
                return sec.Object;
            }
            configuration.Setup(a => a.GetSection("TestDateKey")).Returns(confresult("prefix-{DateTime.Now.ToString(\"yyyy.MM.dd\")}"));
            configuration.Setup(a => a.GetSection("TestExpressionKey")).Returns(confresult("{1+1}"));
            configuration.Setup(a => a.GetSection("TestStringUserObjKey")).Returns(confresult("{userObj.stringValue + \"_1\"}"));
            configuration.Setup(a => a.GetSection("TestIntUserObjKey")).Returns(confresult("{userObj.intValue + 10}"));

            _userObj = new UserObject()
            {
                intValue = 17,
                stringValue = "teststring"
            };

            _config = new Aspose.Cloud.Marketplace.Services.ConfigurationExpression(configuration.Object, _userObj);
        }
        [Fact]
        public void DateTimeTest()
        {
            string todayExpected = DateTime.Now.ToString("yyyy.MM.dd");
            string todayActual = _config.Get("TestDateKey", "default value NA");
            Assert.Equal($"prefix-{todayExpected}", todayActual);
        }

        [Fact]
        public void ExpressionTest()
        {
            Assert.Equal($"2", _config.Get("TestExpressionKey", "default value NA"));
        }

        [Fact]
        public void UserObjStringTest()
        {
            Assert.Equal($"teststring_1", _config.Get("TestStringUserObjKey", "default value NA"));
        }

        [Fact]
        public void UserObjIntTest()
        {
            Assert.Equal($"27", _config.Get("TestIntUserObjKey", "default value NA"));
        }
    }
}
