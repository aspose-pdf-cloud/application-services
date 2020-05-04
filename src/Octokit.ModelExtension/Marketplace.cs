using System;
using System.Collections.Generic;

namespace Octokit.ModelExtension
{
    /// <summary>
    /// Missed models from Octokit library
    /// </summary>
    public class MarketplacePlan
    {
        public MarketplacePlan() { }

        public MarketplacePlan(int? id, string name, string description, int? monthlyPriceInCents, int? yearlyPriceInCents, string priceModel, bool? hasFreeTrial, string unitName, List<string> bullets)
        {
            Id = id;
            Name = name;
            Description = description;
            MonthlyPriceInCents = monthlyPriceInCents;
            YearlyPriceInCents = yearlyPriceInCents;
            PriceModel = priceModel;
            HasFreeTrial = hasFreeTrial;
            UnitName = unitName;
            Bullets = bullets;
        }

        public int? Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public int? MonthlyPriceInCents { get; protected set; }
        public int? YearlyPriceInCents { get; protected set; }
        public string PriceModel { get; protected set; }
        public bool? HasFreeTrial { get; protected set; }
        public string UnitName { get; protected set; }
        public List<string> Bullets { get; protected set; }

    }
    public class Account : Octokit.Account
    {
        public Account() { }

        public Account(string avatarUrl, string bio, string blog, int collaborators, string company, DateTimeOffset createdAt, int diskUsage, string email, int followers, int following, bool? hireable, string htmlUrl, int totalPrivateRepos, int id, string location, string login, string name, string nodeId, int ownedPrivateRepos, Plan plan, int privateGists, int publicGists, int publicRepos, AccountType type, string url)
            : base(avatarUrl, bio, blog, collaborators, company, createdAt, diskUsage, email, followers, following, hireable, htmlUrl, totalPrivateRepos, id, location, login, name, nodeId, ownedPrivateRepos, plan, privateGists, publicGists, publicRepos, type, url)
        { }

    }

    public class MarketplacePurchase
    {
        public MarketplacePurchase() { }
        public MarketplacePurchase(MarketplacePlan plan, string billingCycle, int? unitCount, bool? onFreeTrial, DateTimeOffset? freeTrialEndsOn, DateTimeOffset? nextBillingDate, Account account)
        {
            Plan = plan;
            BillingCycle = billingCycle;
            UnitCount = unitCount;
            OnFreeTrial = onFreeTrial;
            FreeTrialEndsOn = freeTrialEndsOn;
            NextBillingDate = nextBillingDate;
            Account = account;
        }

        public MarketplacePlan Plan { get; protected set; }
        public string BillingCycle { get; protected set; }
        public int? UnitCount { get; protected set; }
        public bool? OnFreeTrial { get; protected set; }
        public DateTimeOffset? FreeTrialEndsOn { get; protected set; }
        public DateTimeOffset? NextBillingDate { get; protected set; }
        public Account Account { get; protected set; }
    }
    public class MarketplacePurchaseEvent
    {
        public MarketplacePurchaseEvent() { }
        public MarketplacePurchaseEvent(string action, DateTimeOffset? effectiveDate, User sender, MarketplacePurchase marketplacePurchase)
        {
            Action = action;
            EffectiveDate = effectiveDate;
            Sender = sender;
            MarketplacePurchase = marketplacePurchase;
        }

        public string Action { get; protected set; }
        public DateTimeOffset? EffectiveDate { get; protected set; }
        public User Sender { get; protected set; }
        public MarketplacePurchase MarketplacePurchase { get; protected set; }
        public MarketplacePurchase PreviousMarketplacePurchase { get; protected set; }

    }
}
