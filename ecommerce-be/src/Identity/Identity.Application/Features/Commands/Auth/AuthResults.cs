namespace Identity.Application.Features.Commands.Auth;

public record RegisterResult(Guid UserId, string Username, string Email);
public record LoginResult(
    Guid UserId,
    string Username,
    string Email,
    string AccessToken,
    string RefreshToken
);
public record AuthResponse(Guid UserId, string Username, string Email, string AccessToken);
