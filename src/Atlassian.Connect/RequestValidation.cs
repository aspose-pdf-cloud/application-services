using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
using System;
using JWT;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect
{
    /// <summary>
    /// Validates request from jira cloud instance protected by jwt token
    /// jwt token must be in Authorization header or in query string
    /// </summary>
    public class RequestValidation
    {
        private readonly string _jwt;
        private readonly string _method;
        private readonly string _path;
        private readonly string _queryString;
        public string ClientKey => Token?.iss;
        public Model.Jwt Token { get; }
        public bool TokenExists => Token != null;

        public RequestValidation(HttpRequest request)
        {
            Token = null;
            var q = QueryHelpers.ParseQuery(request.QueryString.Value);
            _jwt = q.Where(x => x.Key == "jwt").Select(x => x.Value).FirstOrDefault();
            if (null == _jwt && request.Headers.ContainsKey(HeaderNames.Authorization)) // try Authorization header
            {
                var auth = AuthenticationHeaderValue.Parse(request.Headers[HeaderNames.Authorization]);
                if (auth.Scheme.ToLower() == "jwt")
                    _jwt = auth.Parameter;
            }

            if (null != _jwt)
            {
                Token = Utils.DecodeToken(_jwt);
                _method = request.Method;
                _path = request.Path;
                _queryString = request.QueryString.Value;
            }
        }

        public bool Validate(Interface.IRegistrationData registrationData)
        {
            if (TokenExists)
            {
                Model.Jwt token;
                try
                {
                    token = Utils.DecodeTokenVerify(_jwt, registrationData.SharedSecret);
                }
                catch (Exception ex)
                {
                    throw new TokenExpiredException($"{ex.Message} for {registrationData.ClientKey}");
                }
                
                if (!token.isValidDate)
                    throw new TokenExpiredException($"Token Expired for {registrationData.ClientKey}");

                var hash = QueryStringHasher.CalculateHash(_method, _path, _queryString);
                if (hash != token.qsh)
                    throw new SignatureVerificationException($"Invalid URL signature for {ClientKey}");
            }
            return TokenExists;
        }

    }
}
