using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Data.Models.Chat;
using SkillGo.Services;
using System.Security.Claims;

namespace SkillGo.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public ChatHub(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task JoinConversation(int conversationId)
    {
        var meId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(meId))
            throw new HubException("Unauthorized");

        await using var db = await _dbFactory.CreateDbContextAsync();

        var conv = await db.Conversations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == conversationId);

        if (conv == null)
            throw new HubException("Chat not found");

        var isMember = conv.UserAId == meId || conv.UserBId == meId;
        if (!isMember)
            throw new HubException("Access denied");

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversationId));
    }

    public async Task SendMessage(int conversationId, string body)
    {
        var meId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(meId))
            throw new HubException("Unauthorized");

        if (string.IsNullOrWhiteSpace(body))
            return;

        body = body.Trim();
        if (body.Length == 0)
            return;

        await using var db = await _dbFactory.CreateDbContextAsync();

        var conv = await db.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);
        if (conv == null)
            throw new HubException("Chat not found");

        var isMember = conv.UserAId == meId || conv.UserBId == meId;
        if (!isMember)
            throw new HubException("Access denied");

        var msg = new Message
        {
            ConversationId = conversationId,
            SenderId = meId,
            Body = body,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Messages.Add(msg);
        conv.LastMessageAtUtc = msg.CreatedAtUtc;

        await db.SaveChangesAsync();

        var dto = new MessageDto
        {
            Id = msg.Id,
            ConversationId = msg.ConversationId,
            SenderId = msg.SenderId,
            Body = msg.Body,
            OrderId = msg.OrderId,
            Order = null,
            CreatedAtUtc = msg.CreatedAtUtc,
            EditedAtUtc = msg.EditedAtUtc,
            IsDeleted = msg.IsDeleted,
            DeletedAtUtc = msg.DeletedAtUtc
        };

        await Clients.Group(GroupName(conversationId)).SendAsync("MessageAdded", dto);
    }

    private static string GroupName(int conversationId) => $"conv:{conversationId}";
}