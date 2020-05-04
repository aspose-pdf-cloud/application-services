namespace Octokit.ModelExtension
{
    /// <summary>
    /// Missed models from Octokit library
    /// </summary>
    public class PingEvent
    {
        public PingEvent() { }
        public PingEvent(string zen, int? hookId, Hook hook, Repository repository, User sender)
        {
            Zen = zen;
            HookId = hookId;
            Hook = hook;
            Repository = repository;
            Sender = sender;
        }

        public string Zen { get; protected set; }
        public int? HookId { get; protected set; }
        public Hook Hook { get; protected set; }
        public Repository Repository { get; protected set; }
        public User Sender { get; protected set; }
    }
}
