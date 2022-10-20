using System.Collections.Concurrent;
using Binance.Net.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using MediatR;
using Tradibit.Common.Events;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services.Binance;

public abstract class BaseBinanceStreamService: BaseBinanceService,
    INotificationHandler<UserLoginEvent>,
    INotificationHandler<UserLogoutEvent>
{
    protected BinanceSocketClient SocketClient { get; }
    
    private static readonly ConcurrentDictionary<Guid, List<int>> Subscriptions = new();

    protected BaseBinanceStreamService(ILogger logger, ICurrentUserProvider currentUserProvider) : base(logger, currentUserProvider)
    {
        SocketClient = new BinanceSocketClient(new BinanceSocketClientOptions { ApiCredentials = ApiCredentials });
    }

    public async Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
    {
        var subscriptions = await LoginHandle(cancellationToken);
        Subscriptions.TryAdd(CurrentUserProvider.CurrentUser.Id, subscriptions);
    }

    public async Task Handle(UserLogoutEvent notification, CancellationToken cancellationToken)
    {
        if (!Subscriptions.TryRemove(notification.UserId, out var subscriptions))
            return;
        
        foreach (var s in subscriptions) 
            await SocketClient.UnsubscribeAsync(s);
    }

    protected abstract Task<List<int>> LoginHandle(CancellationToken cancellationToken = default);
}