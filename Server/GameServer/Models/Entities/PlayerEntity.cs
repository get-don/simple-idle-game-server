namespace GameServer.Models.DbModels;

public class PlayerEntity
{
    public long AccountId { get; set; }
    public int PlayerLevel { get; set; }
    public int GoldLevel { get; set; }
    public int Stage { get; set; }
    public long Gold { get; set; }
}
