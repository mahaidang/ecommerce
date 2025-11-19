using Identity.Api.Contracts.Auth;
using Identity.Application.Features.Commands.Auth;
using Identity.Application.Features.Users.Queries.GetCurrentUser;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Auth;

namespace Identity.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUser _currentUser;

    public AuthController(ISender sender, ICurrentUser currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        // Map Request -> Dto -> Command
        var dto = request.Adapt<RegisterDto>();
        var cmd = new RegisterCommand(dto);
        var result = await _sender.Send(cmd, ct);

        // Map Result -> Response
        var response = result.Adapt<RegisterResponse>();
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var dto = request.Adapt<LoginDto>();
        var cmd = new LoginCommand(dto);
        var result = await _sender.Send(cmd, ct);
        var response = result.Adapt<LoginResponse>();
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var result = await _sender.Send(new GetCurrentUserQuery(_currentUser.UserId.Value), ct);
        return Ok(result.Adapt<ProfileResponse>());
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest req, CancellationToken ct)
    {
        var result = await _sender.Send(new RefreshTokenCommand(req.RefreshToken), ct);
        return Ok(result); // includes new access + refresh
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(RevokeRequest req, CancellationToken ct)
    {
        // optional: if no token passed, revoke all for current user
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
            await _sender.Send(new RevokeAllTokensCommand(userId), ct);
            return NoContent();
        }

        await _sender.Send(new RevokeTokenCommand(req.RefreshToken), ct);
        return NoContent();
    }

}
