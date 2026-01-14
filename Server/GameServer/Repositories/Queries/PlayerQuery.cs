namespace GameServer.Repositories.Queries;

public static class PlayerQuery
{
    public const string InsertPlayer = """
        INSERT INTO t_player (account_id, level, gold_level, stage, gold)
        VALUES (@AccountId, @Level, @GoldLevel, @Stage, @Gold);
        """;

    public const string LoadPlayer = """
        SELECT
            player_id AS PlayerId,
            level AS Level,
            gold_level AS GoldLevel,
            stage AS Stage,
            gold AS Gold
        FROM t_player
        WHERE account_id = @AccountId;
        """;

    public const string UpdatePlayer = """
        UPDATE t_player
        SET level=@Level, gold_level=@GoldLevel, stage=@Stage, gold=@Gold
        WHERE player_id = @PlayerId;
        """;
}

