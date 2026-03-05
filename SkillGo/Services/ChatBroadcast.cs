using System.Collections.Concurrent;
using SkillGo.Data.Models.Chat;

namespace SkillGo.Services;

public sealed class ChatBroadcast
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<Guid, Action<MessageDto>>> _subs = new();

    public IDisposable Subscribe(int conversationId, Action<MessageDto> handler)
    {
        var id = Guid.NewGuid();
        var dict = _subs.GetOrAdd(conversationId, _ => new ConcurrentDictionary<Guid, Action<MessageDto>>());
        dict[id] = handler;

        return new Unsubscriber(() =>
        {
            if (_subs.TryGetValue(conversationId, out var handlers))
            {
                handlers.TryRemove(id, out _);
                if (handlers.IsEmpty)
                    _subs.TryRemove(conversationId, out _);
            }
        });
    }

    public void Publish(MessageDto message)
    {
        if (_subs.TryGetValue(message.ConversationId, out var handlers))
        {
            foreach (var h in handlers.Values)
            {
                try { h(message); } catch { }
            }
        }
    }

    private sealed class Unsubscriber : IDisposable
    {
        private Action? _dispose;
        public Unsubscriber(Action dispose) => _dispose = dispose;
        public void Dispose()
        {
            Interlocked.Exchange(ref _dispose, null)?.Invoke();
        }
    }
}