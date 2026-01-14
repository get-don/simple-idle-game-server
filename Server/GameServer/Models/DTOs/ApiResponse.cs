namespace GameServer.Models.DTOs;

public enum ErrorCode
{
    Ok,

    // Error
    InternalServerError = 1000,
    ValidationFailed,
    RequestInProgress,

    // Auth
    EmailAlreadyExists = 2000,
    AccountNotExist,
    WrongPassword,
    NotLoggedIn,
    AuthTokenNotExists,

    // Game
    NotExistsPlayer = 3000,
}

public class ApiResponse<T>(bool ok = true) where T : class
{
    public bool Ok { get; set; } = ok;

    private ErrorCode _errorCode = ErrorCode.Ok;

    public ErrorCode ErrorCode
    {
        get => _errorCode;
        set
        {
            _errorCode = value;
            ErrorCodeName = value.ToString();
        }
    }
        
    public string ErrorCodeName { get; private set; } = nameof(ErrorCode.Ok);
    public string? ErrorMessage { get; set; }

    public T? Result { get; set; }
}

public class ApiResponse(bool ok = true) : ApiResponse<object>(ok) { }
