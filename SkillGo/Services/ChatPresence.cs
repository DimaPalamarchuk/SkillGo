using System.Collections.Concurrent;
using SkillGo.Data.Models.Chat;

namespace SkillGo.Services;

public sealed class ChatPresence : IDisposable
{
    private readonly ChatBroadcast _broadcast;

    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, DateTime>> _seen = new();
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, bool>> _online = new();

    private readonly TimeSpan _onlineTtl = TimeSpan.FromSeconds(20);
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(5));
    private readonly CancellationTokenSource _cts = new();

    public ChatPresence(ChatBroadcast broadcast)
    {
        _broadcast = broadcast;
        _ = Task.Run(CleanupLoop);
    }

    public bool IsOnline(int conversationId, string userId)
    {
        if (_online.TryGetValue(conversationId, out var dict) && dict.TryGetValue(userId, out var on))
            return on;

        return false;
    }

    public DateTime? LastSeenUtc(int conversationId, string userId)
    {
        if (_seen.TryGetValue(conversationId, out var dict) && dict.TryGetValue(userId, out var dt))
            return dt;

        return null;
    }

    public void Ping(int conversationId, string userId)
    {
        var now = DateTime.UtcNow;

        var seenDict = _seen.GetOrAdd(conversationId, _ => new ConcurrentDictionary<string, DateTime>());
        seenDict[userId] = now;

        var onlineDict = _online.GetOrAdd(conversationId, _ => new ConcurrentDictionary<string, bool>());
        var wasOnline = onlineDict.TryGetValue(userId, out var prev) && prev;

        if (!wasOnline)
        {
            onlineDict[userId] = true;

            _broadcast.PublishPresence(new PresenceDto
            {
                ConversationId = conversationId,
                UserId = userId,
                IsOnline = true,
                SeenAtUtc = now
            });
        }
    }

    public void Leave(int conversationId, string userId)
    {
        var now = DateTime.UtcNow;

        if (_online.TryGetValue(conversationId, out var onlineDict))
        {
            if (onlineDict.TryGetValue(userId, out var prev) && prev)
            {
                onlineDict[userId] = false;

                _broadcast.PublishPresence(new PresenceDto
                {
                    ConversationId = conversationId,
                    UserId = userId,
                    IsOnline = false,
                    SeenAtUtc = now
                });
            }
        }

        if (_seen.TryGetValue(conversationId, out var seenDict))
            seenDict[userId] = now;
    }

    private async Task CleanupLoop()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                var now = DateTime.UtcNow;

                foreach (var convPair in _seen)
                {
                    var convId = convPair.Key;
                    var seenDict = convPair.Value;

                    foreach (var userPair in seenDict)
                    {
                        var userId = userPair.Key;
                        var last = userPair.Value;

                        if (now - last > _onlineTtl)
                        {
                            if (_online.TryGetValue(convId, out var onlineDict))
                            {
                                if (onlineDict.TryGetValue(userId, out var prev) && prev)
                                {
                                    onlineDict[userId] = false;

                                    _broadcast.PublishPresence(new PresenceDto
                                    {
                                        ConversationId = convId,
                                        UserId = userId,
                                        IsOnline = false,
                                        SeenAtUtc = last
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException) { }
        catch { }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _timer.Dispose();
    }
}