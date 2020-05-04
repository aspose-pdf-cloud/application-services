using System;
using JWT.Algorithms;
using JWT.Builder;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect
{
    public static class Utils
    {
        /// <summary>
        /// Encodes JWT token
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="issuer"></param>
        /// <param name="subject"></param>
        /// <param name="expMinutes"></param>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="queryString"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string EncodeToken(string secret, string issuer, string subject, int expMinutes, string method, string path, string queryString = "", DateTimeOffset? dt = null)  => 
            new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret)
                .Issuer(issuer)
                .AddClaim("iat", (dt ?? DateTimeOffset.UtcNow).ToUnixTimeSeconds())
                .AddClaim("exp", (dt ?? DateTimeOffset.UtcNow).AddMinutes(expMinutes).ToUnixTimeSeconds())
                .AddClaim("qsh", QueryStringHasher.CalculateHash(method, path, queryString))
                .AddClaim("sub", subject)
                .Encode();
        /// <summary>
        /// Decodes JWT token without verification
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public static Model.Jwt DecodeToken(string jwt) => new JwtBuilder().Decode<Model.Jwt>(jwt);
        /// <summary>
        /// Decodes JWT token with verification
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static Model.Jwt DecodeTokenVerify(string jwt, string secret) =>
            new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret)
                .MustVerifySignature()
                .Decode<Model.Jwt>(jwt);
    }
}
