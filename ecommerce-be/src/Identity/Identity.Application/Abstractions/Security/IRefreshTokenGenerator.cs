namespace Identity.Application.Abstractions.Security;

public interface IRefreshTokenGenerator
{
    (string tokenPlain, string tokenHash) Generate(Guid userId);
}
