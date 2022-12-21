﻿using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Services.Repositories;
using Strem.Plugins.Analytics.Types;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Variables;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client.Events;

namespace Strem.Plugins.Analytics.Twitch.Plugin;

public class TwitchAnalyticsPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public ILogger<TwitchAnalyticsPluginStartup> Logger { get; }
    public IObservableTwitchClient TwitchClient { get; }
    public ITwitchAPI TwitchApiClient { get; }
    public IStreamInteractionRepository InteractionRepository { get; }
    public IStreamMetricRepository MetricRepository { get; }
    
    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public TwitchAnalyticsPluginStartup(IEventBus eventBus, IAppState appState, ILogger<TwitchAnalyticsPluginStartup> logger, IObservableTwitchClient twitchClient, ITwitchAPI twitchApiClient, IStreamInteractionRepository interactionRepository, IStreamMetricRepository metricRepository)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        TwitchClient = twitchClient;
        TwitchApiClient = twitchApiClient;
        InteractionRepository = interactionRepository;
        MetricRepository = metricRepository;
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

        Observable.Interval(TimeSpan.FromMinutes(TwitchAnalyticsPluginSettings.RefreshViewerCountInMinutes))
            .Subscribe(x => TrackViewerMetrics())
            .AddTo(_subs);
        
        Logger.Information("Finished Twitch Analytics Tracking Setup");
    }

    private async Task TrackViewerMetrics()
    {
        if (!AppState.HasTwitchOAuth()) { return; }

        var twitchUserId = AppState.GetTwitchUserId();
        var userIds = new List<string>() { twitchUserId };
        var streamInfo = await TwitchApiClient.Helix.Streams.GetStreamsAsync(userIds: userIds);
        if(streamInfo.Streams.Length == 0) { return; }

        var stream = streamInfo.Streams[0];
        var metric = new StreamMetric
        {
            MetricType = MetricTypes.ViewerCount,
            MetricDateTime = DateTime.Now,
            MetricValue = stream.ViewerCount,
            SourceContext = stream.UserName,
            PlatformContext = "twitch",
            Metadata = new Dictionary<string, string>()
            {
                { "category", stream.GameName },
                { "title", stream.Title }
            }
        };
        
        MetricRepository.Create(metric.Id, metric);
    }

    private void TrackJoiningMetric(OnUserJoinedArgs args)
    {
        var interaction = new StreamInteraction
        {
            InteractionType = InteractionTypes.UserJoined,
            SourceContext = args.Channel,
            PlatformContext = "twitch",
            UserContext = args.Username,
            InteractionDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>()
            {
                { "category", AppState.TransientVariables.Get(TwitchVars.ChannelGame) },
                { "title", AppState.TransientVariables.Get(TwitchVars.ChannelTitle) }
            }
        };
        
        InteractionRepository.Create(interaction.Id, interaction);
    }

    private void TrackNewSubscriptionMetric(OnNewSubscriberArgs args)
    {
        var metric = new StreamMetric
        {
            MetricType = MetricTypes.Currency,
            SourceContext = args.Channel,
            PlatformContext = "twitch",
            UserContext = args.Subscriber.DisplayName,
            MetricDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>()
            {
                { "category", AppState.TransientVariables.Get(TwitchVars.ChannelGame) },
                { "title", AppState.TransientVariables.Get(TwitchVars.ChannelTitle) },
                { "sub-length",  "1" },
                { "sub-type",  args.Subscriber.SubscriptionPlanName }
            }
        };
        
        MetricRepository.Create(metric.Id, metric);
    }
    
    private void TrackReSubscriptionMetric(OnReSubscriberArgs args)
    {
        var metric = new StreamMetric
        {
            MetricType = MetricTypes.Currency,
            SourceContext = args.Channel,
            PlatformContext = "twitch",
            UserContext = args.ReSubscriber.DisplayName,
            MetricDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>()
            {
                { "category", AppState.TransientVariables.Get(TwitchVars.ChannelGame) },
                { "title", AppState.TransientVariables.Get(TwitchVars.ChannelTitle) },
                { "sub-length",  args.ReSubscriber.Months.ToString() },
                { "sub-type",  args.ReSubscriber.SubscriptionPlan.ToString() }
            }
        };
        
        MetricRepository.Create(metric.Id, metric);
    }
    
    private void TrackGiftSubscriptionMetric(OnGiftedSubscriptionArgs args)
    {
        var metric = new StreamMetric
        {
            MetricType = MetricTypes.Currency,
            SourceContext = args.Channel,
            PlatformContext = "twitch",
            UserContext = args.GiftedSubscription.MsgParamRecipientUserName,
            MetricDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>()
            {
                { "category", AppState.TransientVariables.Get(TwitchVars.ChannelGame) },
                { "title", AppState.TransientVariables.Get(TwitchVars.ChannelTitle) },
                { "sub-length",  args.GiftedSubscription.MsgParamMultiMonthGiftDuration },
                { "sub-type",  args.GiftedSubscription.MsgParamSubPlan.ToString() },
                { "gifter", args.GiftedSubscription.DisplayName}
            }
        };
        
        MetricRepository.Create(metric.Id, metric);
    }
    
    private void TrackLeftMetric(OnUserLeftArgs args)
    {
        var interaction = new StreamInteraction
        {
            InteractionType = InteractionTypes.UserLeft,
            SourceContext = args.Channel,
            PlatformContext = "twitch",
            UserContext = args.Username,
            InteractionDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>()
            {
                { "category", AppState.TransientVariables.Get(TwitchVars.ChannelGame) },
                { "title", AppState.TransientVariables.Get(TwitchVars.ChannelTitle) }
            }
        };
        
        InteractionRepository.Create(interaction.Id, interaction);
    }

    private void TrackMessageMetrics(OnMessageReceivedArgs args)
    {
        Logger.Information("Tracking message metric");
        var interaction = new StreamInteraction
        {
            InteractionType = InteractionTypes.ChatMessage,
            SourceContext = args.ChatMessage.Channel,
            PlatformContext = "twitch",
            UserContext = args.ChatMessage.Username,
            InteractionDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>()
            {
                { "category", AppState.TransientVariables.Get(TwitchVars.ChannelGame) },
                { "title", AppState.TransientVariables.Get(TwitchVars.ChannelTitle) }
            }
        };
        InteractionRepository.Create(interaction.Id, interaction);

        if (args.ChatMessage.Bits == 0)
        { return; }

        var metric = new StreamMetric
        {
            MetricType = MetricTypes.Currency,
            UserContext = args.ChatMessage.Username,
            PlatformContext = "twitch",
            SourceContext = args.ChatMessage.Channel,
            MetricDateTime = DateTime.Now,
            Metadata = new Dictionary<string, string>()
            {
                { "category", AppState.TransientVariables.Get(TwitchVars.ChannelGame) },
                { "title", AppState.TransientVariables.Get(TwitchVars.ChannelTitle) }
            }
        };
        MetricRepository.Create(metric.Id, metric);
    }

    public void Dispose()
    { _subs?.Dispose(); }
}