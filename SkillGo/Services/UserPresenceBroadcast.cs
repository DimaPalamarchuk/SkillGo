using System.Collections.Concurrent;
using SkillGo.Data.Models.Presence;

namespace SkillGo.Services;

public sealed class UserPresenceBroadcast
{
    private readonly ConcurrentDictionary<Guid, Action<UserPresenceDto>> _subs = new();

    public IDisposable Subscribe(Action<UserPresenceDto> handler)
    {
        var id = Guid.NewGuid();
        _subs[id] = handler;

        return new Unsubscriber(() =>
        {
            _subs.TryRemove(id, out _);
        });
    }

    public void Publish(UserPresenceDto dto)
    {
        foreach (var h in _subs.Values)
        {
            try { h(dto); } catch { }
        }
    }

    private sealed class Unsubscriber : IDisposable
    {
        private Action? _dispose;
        public Unsubscriber(Action dispose) => _dispose = dispose;
        public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }
}