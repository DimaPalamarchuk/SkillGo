namespace SkillGo.Data.Models.Chat;

public class MessageDto
{
	public int Id { get; set; }
	public int ConversationId { get; set; }
	public string SenderId { get; set; } = "";
	public string? Body { get; set; }
	public DateTime CreatedAtUtc { get; set; }
}