using System.Collections.Concurrent;
using SkillGo.Data.Models.Chat;

namespace SkillGo.Services;

public sealed class ChatBroadcast
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<Guid, Action<MessageDto>>> _msgSubs = new();
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<Guid, Action<PresenceDto>>> _presenceSubs = new();

    public IDisposable SubscribeMessages(int conversationId, Action<MessageDto> handler)
    {
        var id = Guid.NewGuid();
        var dict = _msgSubs.GetOrAdd(conversationId, _ => new ConcurrentDictionary<Guid, Action<MessageDto>>());
        dict[id] = handler;

        return new Unsubscriber(() =>
        {
            if (_msgSubs.TryGetValue(conversationId, out var handlers))
            {
                handlers.TryRemove(id, out _);
                if (handlers.IsEmpty) _msgSubs.TryRemove(conversationId, out _);
            }
        });
    }

    public IDisposable SubscribePresence(int conversationId, Action<PresenceDto> handler)
    {
        var id = Guid.NewGuid();
        var dict = _presenceSubs.GetOrAdd(conversationId, _ => new ConcurrentDictionary<Guid, Action<PresenceDto>>());
        dict[id] = handler;

        return new Unsubscriber(() =>
        {
            if (_presenceSubs.TryGetValue(conversationId, out var handlers))
            {
                handlers.TryRemove(id, out _);
                if (handlers.IsEmpty) _presenceSubs.TryRemove(conversationId, out _);
            }
        });
    }

    public void PublishMessage(MessageDto message)
    {
        if (_msgSubs.TryGetValue(message.ConversationId, out var handlers))
        {
            foreach (var h in handlers.Values)
            {
                try { h(message); } catch { }
            }
        }
    }

    public void PublishPresence(PresenceDto presence)
    {
        if (_presenceSubs.TryGetValue(presence.ConversationId, out var handlers))
        {
            foreach (var h in handlers.Values)
            {
                try { h(presence); } catch { }
            }
        }
    }

    private sealed class Unsubscriber : IDisposable
    {
        private Action? _dispose;
        public Unsubscriber(Action dispose) => _dispose = dispose;
        public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }
}