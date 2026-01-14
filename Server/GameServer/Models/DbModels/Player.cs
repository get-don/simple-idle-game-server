namespace GameServer.Models.DbModels;

public class PlayerInfo
{
    public long PlayerId { get; set; }
    public long AccountId { get; set; }
    public int Level { get; set; }
    public int GoldLevel { get; set; }
    public int Stage { get; set; }
    public long Gold { get; set; }
}
