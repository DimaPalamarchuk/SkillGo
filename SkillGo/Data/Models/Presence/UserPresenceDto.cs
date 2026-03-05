namespace SkillGo.Data.Models.Presence;

public class UserPresenceDto
{
    public string UserId { get; set; } = "";
    public bool IsOnline { get; set; }
    public DateTime SeenAtUtc { get; set; }
}