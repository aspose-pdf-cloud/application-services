using System.Collections.Generic;

namespace Octokit.ModelExtension
{
    /// <summary>
    /// Missed models from Octokit library
    /// </summary>
    public class HookConfig
    {
        public HookConfig()
        {
        }

        public HookConfig(string url, string contentType, string secret, string insecureSsl)
        {
            Url = url;
            ContentType = contentType;
            Secret = secret;
            InsecureSsl = insecureSsl;
        }

        public string Url { get; protected set; }
        public string ContentType { get; protected set; }
        public string Secret { get; protected set; }
        public string InsecureSsl { get; protected set; }
    }

    public class HookLastResponse
    {
        public HookLastResponse()
        {
        }

        public HookLastResponse(int? code, string status, string message)
        {
            Code = code;
            Status = status;
            Message = message;
        }

        public int? Code { get; protected set; }
        public string Status { get; protected set; }
        public string Message { get; protected set; }
    }

    public class Hook
    {
        public Hook()
        {
        }

        public Hook(string type, int? id, string name, bool? active, List<string> events, HookConfig config, string url, string testUrl, string pingUrl)
        {
            Type = type;
            Id = id;
            Name = name;
            Active = active;
            Events = events;
            Config = config;
            Url = url;
            TestUrl = testUrl;
            PingUrl = pingUrl;
        }

        public string Type { get; protected set; }
        public int? Id { get; protected set; }
        public string Name { get; protected set; }
        public bool? Active { get; protected set; }
        public List<string> Events { get; protected set; }
        public HookConfig Config { get; protected set; }
        public string Url { get; protected set; }
        public string TestUrl { get; protected set; }
        public string PingUrl { get; protected set; }
    }
}
