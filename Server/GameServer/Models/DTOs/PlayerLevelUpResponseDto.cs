namespace GameServer.Models.DTOs;

public class PlayerLevelUpResponseDto
{
    public int NextLevel { get; set; }
    public long Cost { get; set; }
    public long TotalGold { get; set; }
}
