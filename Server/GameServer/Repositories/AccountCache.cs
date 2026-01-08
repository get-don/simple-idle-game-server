using GameServer.Domain.Models;
using GameServer.Repositories.Interfaces;
using System.Security.Principal;

namespace GameServer.Repositories;

public class AccountCache : IAccountCache
{
    // 테스트 코드
    private static Dictionary<string, UserSession> _testUserSession = [];
    private static Dictionary<long, string> _testSessionToken = [];

    public async Task<UserSession?> GetSession(string sessionToken)
    {
        await Task.Delay(100);

        if (_testUserSession.TryGetValue(sessionToken, out var session))
            return session;

        return null;
    }

    public async Task<string?> GetSessionTokenByAccountId(long accountId)
    {
        await Task.Delay(100);

        if (_testSessionToken.TryGetValue(accountId, out var token))
            return token;

        return null;
    }

    public async Task<bool> SaveSession(UserSession session, TimeSpan ttl)
    {
        await Task.Delay(100);
        // 1. 세션 저장
        // 2. accountId를 키로해서 SessionKey를 값으로 저장

        // 이미 존재하는 토큰이면 false;
        //  //토큰 생성 → Redis에 SET token value NX EX<ttl> 로 “이미 있으면 재시도”

        if (await GetSession(session.Token) != null)
            return false;

        _testUserSession.Add(session.Token, session);
        _testSessionToken.Add(session.AccountId, session.Token);

        return true;
    }
}
