using System.Collections.Concurrent;
using SkillGo.Data.Models.Presence;

namespace SkillGo.Services;

public sealed class UserPresenceService : IDisposable
{
    private readonly UserPresenceBroadcast _broadcast;

    private readonly ConcurrentDictionary<string, DateTime> _lastSeenUtc = new();
    private readonly ConcurrentDictionary<string, bool> _online = new();

    private readonly TimeSpan _ttl = TimeSpan.FromSeconds(25);
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(5));
    private readonly CancellationTokenSource _cts = new();

    public UserPresenceService(UserPresenceBroadcast broadcast)
    {
        _broadcast = broadcast;
        _ = Task.Run(CleanupLoop);
    }

    public bool IsOnline(string userId)
    {
        if (_online.TryGetValue(userId, out var on)) return on;
        return false;
    }

    public DateTime? LastSeenUtc(string userId)
    {
        if (_lastSeenUtc.TryGetValue(userId, out var dt)) return dt;
        return null;
    }

    public void Ping(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;

        var now = DateTime.UtcNow;
        _lastSeenUtc[userId] = now;

        var wasOnline = _online.TryGetValue(userId, out var prev) && prev;
        if (!wasOnline)
        {
            _online[userId] = true;

            _broadcast.Publish(new UserPresenceDto
            {
                UserId = userId,
                IsOnline = true,
                SeenAtUtc = now
            });
        }
    }

    public void Leave(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;

        var now = DateTime.UtcNow;
        _lastSeenUtc[userId] = now;

        if (_online.TryGetValue(userId, out var prev) && prev)
        {
            _online[userId] = false;

            _broadcast.Publish(new UserPresenceDto
            {
                UserId = userId,
                IsOnline = false,
                SeenAtUtc = now
            });
        }
    }

    private async Task CleanupLoop()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                var now = DateTime.UtcNow;

                foreach (var pair in _lastSeenUtc)
                {
                    var userId = pair.Key;
                    var last = pair.Value;

                    if (now - last > _ttl)
                    {
                        if (_online.TryGetValue(userId, out var prev) && prev)
                        {
                            _online[userId] = false;

                            _broadcast.Publish(new UserPresenceDto
                            {
                                UserId = userId,
                                IsOnline = false,
                                SeenAtUtc = last
                            });
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