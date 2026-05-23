using Mystreet.Domain.Entities;

namespace Mystreet.Infrastructure.Auth;

public interface IJwtTokenService
{
    string CreateToken(User user);
}