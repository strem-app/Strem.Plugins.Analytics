using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Services.Repositories;
using Strem.Plugins.Analytics.Twitch.Types;
using Strem.Plugins.Analytics.Twitch.Variables;
using Strem.Plugins.Analytics.Types;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client.Events;

namespace Strem.Plugins.Analytics.Twitch.Plugin;

public class TwitchAnalyticsPluginStartup : IPluginStartup, IDisposable
{
    public const string TwitchPlatformContext = "twitch"; 
    
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public ILogger<TwitchAnalyticsPluginStartup> Logger { get; }
    public IObservableTwitchClient TwitchClient { get; }
    public ITwitchAPI TwitchApiClient { get; }
    public IAnalyticsEventRepository AnalyticsEventRepository { get; }
    
    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public TwitchAnalyticsPluginStartup(IEventBus eventBus, IAppState appState, ILogger<TwitchAnalyticsPluginStartup> logger, IObservableTwitchClient twitchClient, ITwitchAPI twitchApiClient, IAnalyticsEventRepository analyticsEventRepository)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        TwitchClient = twitchClient;
        TwitchApiClient = twitchApiClient;
        AnalyticsEventRepository = analyticsEventRepository;
    }

    public Task SetupPlugin() => Task.CompletedTask;
    
    public async Task StartPlugin()
    {
        Logger.Information("Starting Twitch Analytics Tracking Setup");
        
        TwitchClient.OnMessageReceived
            .Subscribe(TrackMessageMetrics)
            .AddTo(_subs);
        
        TwitchClient.OnNewSubscriber
            .Subscribe(TrackNewSubscriptionMetric)
            .AddTo(_subs);
        
        TwitchClient.OnReSubscriber
            .Subscribe(TrackReSubscriptionMetric)
            .AddTo(_subs);
        
        TwitchClient.OnGiftedSubscription
            .Subscribe(TrackGiftSubscriptionMetric)
            .AddTo(_subs);

        TwitchClient.OnUserJoined
            .Subscribe(TrackJoiningMetric)
            .AddTo(_subs);

        TwitchClient.OnUserLeft
            .Subscribe(TrackLeftMetric)
            .AddTo(_subs);
        
        AppState.AppVariables.OnVariableChanged
            .Where(x => x.Key == TwitchAnalyticsViewerVars.Channels)
            .Subscribe(x => JoinRequiredChannels())
            .AddTo(_subs);
        
        Observable.Interval(TimeSpan.FromMinutes(TwitchAnalyticsPluginSettings.RefreshViewerCountInMinutes))
            .Subscribe(x => TrackViewerMetrics())
            .AddTo(_subs);
        
        JoinRequiredChannels();

        Logger.Information("Finished Twitch Analytics Tracking Setup");
    }
    
    public bool MatchesAnalyticsChannels(string channel)
    {
        var channelsToMatch = AppState.AppVariables.Get(TwitchAnalyticsViewerVars.Channels);
        return channelsToMatch.Contains(channel, StringComparison.OrdinalIgnoreCase);
    }

    public int GetMonthsFromString(string monthsString)
    {
        int.TryParse(monthsString, out var numberOfMonths);
        if (numberOfMonths == 0) { numberOfMonths = 1; }
        return numberOfMonths;
    }
    
    public void JoinRequiredChannels()
    {
        if (!TwitchClient.Client.IsConnected) { return; }

        var channelsToJoin = AppState.AppVariables.Get(TwitchAnalyticsViewerVars.Channels);
        if(string.IsNullOrEmpty(channelsToJoin)) { return; }

        var channels = channelsToJoin.Replace(" ","").Split(",");
        foreach (var channel in channels)
        {
            if (TwitchClient.Client.HasJoinedChannel(channel)) { continue; }
            
            Logger.Information($"Twitch Analytics Tracking Channel: {channel}");
            TwitchClient.Client.JoinChannel(channel);
        }
    }

    private async Task TrackViewerMetrics()
    {
        if (!TwitchClient.Client.IsConnected) { return; }
        
        var analyticsChannel = AppState.AppVariables.Get(TwitchAnalyticsViewerVars.Channels);
        var streamInfo = await TwitchApiClient.Helix.Streams.GetStreamsAsync(userLogins: new List<string>() { analyticsChannel });
        if(streamInfo.Streams.Length == 0) { return; }

        var stream = streamInfo.Streams[0];
        var metric = new AnalyticsEvent()
        {
            EventType = TwitchAnalyticEventTypes.ViewerCount,
            EventDateTime = DateTime.Now,
            EventValue = stream.ViewerCount,
            SourceContext = stream.UserName,
            PlatformContext = TwitchPlatformContext,
            Metadata = new Dictionary<string, string>()
            {
                { "category", stream.GameName },
                { "title", stream.Title }
            }
        };
        
        AnalyticsEventRepository.Create(metric.Id, metric);
    }

    private void TrackJoiningMetric(OnUserJoinedArgs args)
    {
        if (!MatchesAnalyticsChannels(args.Channel)) { return; }
        
        var interaction = new AnalyticsEvent
        {
            EventType = TwitchAnalyticEventTypes.UserJoined,
            SourceContext = args.Channel,
            PlatformContext = TwitchPlatformContext,
            UserContext = args.Username,
            EventDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>() {}
        };
        
        AnalyticsEventRepository.Create(interaction.Id, interaction);
    }

    private void TrackNewSubscriptionMetric(OnNewSubscriberArgs args)
    {
        if (!MatchesAnalyticsChannels(args.Channel)) { return; }

        var metric = new AnalyticsEvent
        {
            EventType = TwitchAnalyticEventTypes.Subscriptions,
            SourceContext = args.Channel,
            PlatformContext = TwitchPlatformContext,
            UserContext = args.Subscriber.DisplayName,
            EventDateTime = DateTime.Now,
            EventValue = 1,
            Metadata = new Dictionary<string, string>()
            {
                { "subs-to-date", args.Subscriber.MsgParamCumulativeMonths },
                { "sub-type", args.Subscriber.SubscriptionPlanName }
            }
        };
        
        AnalyticsEventRepository.Create(metric.Id, metric);
    }

    private void TrackReSubscriptionMetric(OnReSubscriberArgs args)
    {
        if (!MatchesAnalyticsChannels(args.Channel)) { return; }
        
        var metric = new AnalyticsEvent
        {
            EventType = TwitchAnalyticEventTypes.Subscriptions,
            SourceContext = args.Channel,
            PlatformContext = TwitchPlatformContext,
            UserContext = args.ReSubscriber.DisplayName,
            EventDateTime = DateTime.Now,
            EventValue = 1,
            Metadata = new Dictionary<string, string>()
            {
                { "subs-to-date",  args.ReSubscriber.MsgParamCumulativeMonths },
                { "sub-type",  args.ReSubscriber.SubscriptionPlanName }
            }
        };
        
        AnalyticsEventRepository.Create(metric.Id, metric);
    }
    
    private void TrackGiftSubscriptionMetric(OnGiftedSubscriptionArgs args)
    {
        if (!MatchesAnalyticsChannels(args.Channel)) { return; }
        
        var numberOfMonths = GetMonthsFromString(args.GiftedSubscription.MsgParamMultiMonthGiftDuration);
        var metric = new AnalyticsEvent
        {
            EventType = TwitchAnalyticEventTypes.Subscriptions,
            SourceContext = args.Channel,
            PlatformContext = TwitchPlatformContext,
            UserContext = args.GiftedSubscription.MsgParamRecipientUserName,
            EventValue = numberOfMonths,
            EventDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>()
            {
                { "sub-type",  args.GiftedSubscription.MsgParamSubPlanName },
                { "gifter", args.GiftedSubscription.DisplayName }
            }
        };
        
        AnalyticsEventRepository.Create(metric.Id, metric);
    }
    
    private void TrackLeftMetric(OnUserLeftArgs args)
    {
        if (!MatchesAnalyticsChannels(args.Channel)) { return; }
        
        var interaction = new AnalyticsEvent
        {
            EventType = TwitchAnalyticEventTypes.UserLeft,
            SourceContext = args.Channel,
            PlatformContext = TwitchPlatformContext,
            UserContext = args.Username,
            EventDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>(){}
        };
        
        AnalyticsEventRepository.Create(interaction.Id, interaction);
    }

    private void TrackMessageMetrics(OnMessageReceivedArgs args)
    {
        if (!MatchesAnalyticsChannels(args.ChatMessage.Channel)) { return; }
        
        var interaction = new AnalyticsEvent
        {
            EventType = TwitchAnalyticEventTypes.ChatMessage,
            SourceContext = args.ChatMessage.Channel,
            PlatformContext = TwitchPlatformContext,
            UserContext = args.ChatMessage.Username,
            EventDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>() { }
        };
        AnalyticsEventRepository.Create(interaction.Id, interaction);

        if (args.ChatMessage.Bits == 0)
        { return; }

        var metric = new AnalyticsEvent
        {
            EventType = TwitchAnalyticEventTypes.Bits,
            UserContext = args.ChatMessage.Username,
            PlatformContext = TwitchPlatformContext,
            SourceContext = args.ChatMessage.Channel,
            EventValue = args.ChatMessage.Bits,
            EventDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>() { }
        };
        AnalyticsEventRepository.Create(metric.Id, metric);
    }

    public void Dispose()
    { _subs?.Dispose(); }
}