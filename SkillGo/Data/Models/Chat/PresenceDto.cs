namespace SkillGo.Data.Models.Chat;

public class PresenceDto
{
    public int ConversationId { get; set; }
    public string UserId { get; set; } = "";
    public bool IsOnline { get; set; }
    public DateTime SeenAtUtc { get; set; }
}