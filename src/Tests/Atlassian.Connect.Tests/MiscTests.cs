using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Moq;
using Xunit;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Tests
{
    public class Utils_Tests
    {
        internal DateTimeOffset iat = new DateTimeOffset(3000, 04, 16, 1, 1, 1, TimeSpan.Zero);
        internal string token =
            @"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJpc3N1ZXIiLCJpYXQiOjMyNTEyNzU1NjYxLCJleHAiOjMyNTEyNzU1OTYxLCJxc2giOiJkODYwNjQ3ZjY5YzFjODcyMzY5NzVmY2U0ZTVjMjA2ZmQwM2QwOWI5Y2UxMTRkMTA3OWUxY2IxYzFlNTIwYTViIiwic3ViIjoidGVzdHMifQ.twkgiX1oCfgM5uOTGOHX7B9czIYYC20gEI_KYjOCjgY";

        public Utils_Tests()
        {
            //token = Utils.EncodeToken("123", "issuer", "tests", 5, "GET", "/a/b/c", "a=b", iat);
        }
        [Fact]
        public void CreateToken_Test()
        {
            Assert.Equal(token, Utils.EncodeToken("123", "issuer", "tests", 5, "GET", "/a/b/c", "a=b", iat));
        }

        [Fact]
        public void DecodeToken_Test()
        {
            var tokenValue = Utils.DecodeToken(token);
            Assert.Equal("issuer", tokenValue.iss);
            Assert.Equal(iat, tokenValue.iatDate);
            Assert.Equal(iat.AddMinutes(5), tokenValue.expDate);
        }

        [Fact]
        public void DecodeTokenVerify_Test()
        {
            var tokenValue = Utils.DecodeTokenVerify(token, "123");
            Assert.Equal("issuer", tokenValue.iss);
            Assert.Equal(iat, tokenValue.iatDate);
            Assert.Equal(iat.AddMinutes(5), tokenValue.expDate);
        }

        [Fact]
        public void RequestValidation_Test()
        {
            HttpRequest request = new DefaultHttpRequest(new DefaultHttpContext());
            var jwt = Utils.EncodeToken("123", "mock_client_id", "tests", 1, "GET", "/a/b/c", "a=b");
            request.Path = "/a/b/c";
            request.Method = "GET";
            request.QueryString = new QueryString($@"?a=b&jwt={jwt}");
            var v = new RequestValidation(request);
            var reg = new Mock<Interface.IRegistrationData>();
            reg.SetupGet(e => e.ClientKey).Returns("mock_client_id");
            reg.SetupGet(e => e.SharedSecret).Returns("123");
            Assert.True(v.Validate(reg.Object), "Request is invalid");

        }
    }
}
