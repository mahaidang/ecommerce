using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Persistencel;
using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Identity.Application.Features.Commands.Auth;

public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _ph;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IRefreshTokenGenerator _rtGen;
    private readonly IRefreshTokenRepository _rtRepo;
    private readonly IConfiguration _config;

    public LoginHandler(
        IUserRepository users,
        IPasswordHasher ph,
        IJwtTokenGenerator jwt,
        IRefreshTokenGenerator rtGen,
        IRefreshTokenRepository rtRepo,
        IConfiguration config)
    {
        _users = users;
        _ph = ph;
        _jwt = jwt;
        _rtGen = rtGen;
        _rtRepo = rtRepo;
        _config = config;
    }

    public async Task<LoginResult> Handle(LoginCommand req, CancellationToken ct)
    {
        // 🔥 Load user cùng Roles
        var user = await _users.GetByUsernameOrEmailAsync(req.Dto.UsernameOrEmail, ct);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid username/email");

        // 🔥 Verify password
        if (!_ph.Verify(req.Dto.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedAccessException("Invalid password");

        // 🔥 Generate Access Token (JWT)
        var accessToken = _jwt.GenerateToken(user);

        // 🔥 Generate Refresh Token (plain + hash)
        var (plain, hash) = _rtGen.Generate(user.Id);
        var days = int.Parse(_config["Auth:RefreshTokenDays"]);

        var refreshEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = hash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(days)
        };

        await _rtRepo.AddAsync(refreshEntity, ct);
        await _rtRepo.SaveChangeAsync(ct);

        // 🔥 Return both tokens
        return new LoginResult(
            user.Id,
            user.Username,
            user.Email,
            accessToken,
            plain
        );
    }
}
