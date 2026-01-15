namespace GameServer.Models;

public class UserSession
{
    public long AccountId { get; set; }
    public string Email { get; set; } = "";
    public string Token { get; set; } = "";
    public DateTime LoginTime { get; set; }
}
