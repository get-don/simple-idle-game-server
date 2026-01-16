namespace GameServer.Models.DTOs;

public class StageClearResponseDto
{
    public int ClearStage { get; set; }
    public int NextStage { get; set; }
    public long RewardGold { get; set; }
    public long TotalGold { get; set; }
}
