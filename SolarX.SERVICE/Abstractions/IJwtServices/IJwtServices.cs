using System.Security.Claims;

namespace SolarX.SERVICE.Abstractions.IJwtServices;

public interface IJwtServices
{
    public string GenerateAccessToken(IEnumerable<Claim> claims);
    
    public string GenerateUpdateToken(IEnumerable<Claim> claims);

    public ClaimsPrincipal? VerifyUpdateToken(string token);
}