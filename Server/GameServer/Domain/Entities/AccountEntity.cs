using System.Reflection.Metadata.Ecma335;

namespace GameServer.Domain.Entities;

public class AccountEntity
{
    public long AccountId { get; set; }
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginTime { get; set; }
}
