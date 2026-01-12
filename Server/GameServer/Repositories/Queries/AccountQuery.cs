namespace GameServer.Repositories.Queries;

public static class AccountQuery
{
    public const string InsertAccount = """
        INSERT INTO t_account (email, password) VALUES (@Email, @Password);
        SELECT LAST_INSERT_ID();
        """;

    public const string ExistsAccount = """
        SELECT EXISTS (
        SELECT 1
        FROM t_account
        WHERE email = @Email
        ) AS is_exists;
        """;

    public const string LoadAccount = """
        SELECT
            aid,
            email,
            password,
            create_time,
            last_login_time
        FROM t_account
        WHERE email = @Email;
        """;

    public const string UpdateLoginTime = """
        UPDATE t_account
        SET last_login_time = CURRENT_TIMESTAMP
        WHERE aid = @Aid
        """;
}
