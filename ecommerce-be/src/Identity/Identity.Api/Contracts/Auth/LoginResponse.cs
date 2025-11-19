namespace Identity.Api.Contracts.Auth;

public sealed record LoginResponse(Guid UserId, string Username, string Email, string AccessToken, string RefreshToken);
