namespace GameServer.Models.DTOs;

public class GoldLevelUpResponseDto
{
    public int PrevGoldLevel { get; set; }
    public int NextGoldLevel { get; set; }
    public long Cost { get; set; }
    public long TotalGold { get; set; }
}
