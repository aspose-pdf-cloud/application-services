using System;

namespace Aspose.Cloud.Marketplace.App.Atlassian.Connect.Model
{
    /// <summary>
    /// JWT token model
    /// </summary>
    public class Jwt
    {
        public long iat { get; set; }
        public DateTimeOffset iatDate => DateTimeOffset.FromUnixTimeSeconds(iat);
        public long exp { get; set; }
        public DateTimeOffset expDate => DateTimeOffset.FromUnixTimeSeconds(exp);

        public bool isValidDate => DateTimeOffset.UtcNow.AddSeconds(15) > iatDate && DateTimeOffset.UtcNow.AddSeconds(-15) < expDate;
        public string qsh { get; set; }
        public string sub { get; set; }

        public string iss { get; set; }
    }
}
