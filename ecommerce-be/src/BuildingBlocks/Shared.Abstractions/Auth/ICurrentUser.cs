namespace Shared.Abstractions.Auth;

public interface ICurrentUser
{
    Guid? UserId { get; }

}
