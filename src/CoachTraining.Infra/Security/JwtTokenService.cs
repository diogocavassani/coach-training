using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoachTraining.App.Abstractions.Security;
using CoachTraining.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoachTraining.Infra.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public TokenResult GerarToken(Professor professor)
    {
        ArgumentNullException.ThrowIfNull(professor);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(_options.ExpirationHours <= 0 ? 8 : _options.ExpirationHours);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, professor.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, professor.Email),
            new(JwtRegisteredClaimNames.Name, professor.Nome),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("professor_id", professor.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResult
        {
            Token = tokenString,
            ExpiraEmUtc = expiration
        };
    }
}
