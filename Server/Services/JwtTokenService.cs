/* This code snippet is defining a C# class named `JwtTokenService` that implements the `ITokenService`
interface. The class is responsible for generating JWT (JSON Web Token) tokens for a given user. */
using Server.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace Server.Services;

public class JwtTokenService(IConfiguration config) : ITokenService
{
    private readonly IConfiguration _config = config;

    /// <summary>
    /// The function `GenerateToken` creates a JWT token for a user with specified claims and expiration
    /// time.
    /// </summary>
    /// <param name="User">The `GenerateToken` method you provided generates a JWT (JSON Web Token) for
    /// a given `User` object. The JWT is created using the configuration settings for SecretKey,
    /// Issuer, Audience, and ExpirationHours from the app settings.</param>
    /// <returns>
    /// The `GenerateToken` method returns a JWT (JSON Web Token) generated for the specified `User`
    /// object.
    /// </returns>
    public string GenerateToken(User user)
    {
        string SecretKey = _config["JwtSettings:SecretKey"] ??
            throw new InvalidOperationException("JwtSettings:SecretKey is not configured");
        string Issuer = _config["JwtSettings:Issuer"] ??
            throw new InvalidOperationException("JwtSettings:Issuer is not configured.");
        string Audience = _config["JwtSettings:Audience"] ??
            throw new InvalidOperationException("JwtSettings:Audience is not configured.");
        int ExpirationHours = int.Parse(_config["JwtSettings:ExpirationHours"] ??
            throw new InvalidOperationException("JwtSettings:ExperationHours is not configured."));
        var Claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: Claims,
            expires: DateTime.UtcNow.AddHours(ExpirationHours),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
};