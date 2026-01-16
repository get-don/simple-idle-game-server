namespace GameServer.Repositories.Queries;

public static class PlayerQuery
{
    public const string InsertPlayer = """
        INSERT INTO t_player (account_id, player_level, gold_level, stage, gold)
        VALUES (@AccountId, @PlayerLevel, @GoldLevel, @Stage, @Gold);
        """;

    public const string LoadPlayer = """
        SELECT
            account_id AS AccountId,
            player_level AS PlayerLevel,
            gold_level AS GoldLevel,
            stage AS Stage,
            gold AS Gold
        FROM t_player
        WHERE account_id = @AccountId;
        """;

    public const string UpdatePlayer = """
        UPDATE t_player
        SET 
            player_level=@PlayerLevel, 
            gold_level=@GoldLevel, 
            stage=@Stage, 
            gold=@Gold
        WHERE account_id = @AccountId;
        """;

    // 간단한 계산이므로 SQL에 보상 계산 넣음. (딱 이정도의 단순한 계산만 사용할 것)
    public const string UpdateStageAndGold = """
        SET @RewardGold := 0;
        UPDATE t_player
        SET
            stage = stage + @StageDelta,
            gold = gold + (@RewardGold := (10 + @CurrentStage + gold_level * 5))
        WHERE account_id = @AccountId
          AND stage = @CurrentStage;

        SELECT 
            @RewardGold AS RewardGold, 
            gold AS TotalGold 
        FROM t_player 
        WHERE account_id = @AccountId 
          AND (SELECT ROW_COUNT()) > 0;
        """;

    public const string UpdatePlayerLevelAndGold = """
        UPDATE t_player 
        SET 
            player_level = player_level + @PlayerLevelDelta, 
            gold = gold - @Cost 
        WHERE account_id = @AccountId 
          AND player_level = @CurrentPlayerLevel 
          AND gold >= @Cost;

        SELECT 
            gold 
        FROM t_player 
        WHERE account_id = @AccountId
          AND (SELECT ROW_COUNT()) > 0;
        """;

    public const string UpdateGoldLevelAndGold = """
        UPDATE t_player 
        SET 
            gold_level = gold_level + @GoldLevelDelta,
            gold = gold - @Cost 
        WHERE account_id = @AccountId 
          AND gold_level = @CurrentGoldLevel
          AND gold >= @Cost;
    
        SELECT 
            gold 
        FROM t_player 
        WHERE account_id = @AccountId
          AND (SELECT ROW_COUNT()) > 0;
        """;

    //public const string UpdateStageAndGold = """
    //    UPDATE t_player
    //    SET
    //        stage = stage + @StageDelta,
    //        gold = gold + (10 + @CurrentStage + gold_level * 5)
    //    WHERE account_id = @AccountId
    //    AND stage = @CurrentStage
    //    RETURNING 
    //        (10 + @CurrentStage + gold_level * 5) AS RewardGold,
    //        gold AS TotalGold;
    //    """;

    //public const string UpdatePlayerLevelAndGold = """
    //    UPDATE t_player
    //    SET
    //        player_level = player_level + @PlayerLevelDelta,
    //        gold = gold - @Cost
    //    WHERE account_id = @AccountId
    //    AND player_level = @CurrentLevel
    //    AND gold >= @Cost
    //    RETURNING gold;
    //    """;

    //public const string UpdateGoldLevelAndGold = """
    //    UPDATE t_player
    //    SET
    //        gold_level = gold_level + @GoldLevelDelta,
    //        gold = gold - @Cost
    //    WHERE account_id = @AccountId
    //    AND gold_level = @CurrentGoldLevel
    //    AND gold >= @Cost
    //    RETURNING gold;
    //    """;
}

