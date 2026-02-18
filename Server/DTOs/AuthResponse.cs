using System.ComponentModel.DataAnnotations;
namespace Server.DTOs;



/// <summary>
/// Represents the response returned after successful user authentication.
/// </summary>
/// <remarks>
/// This DTO is returned to the client upon successful login or registration, containing the authenticated user's
/// information along with a JWT token for subsequent authenticated requests.
/// </remarks>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets the authenticated user's full name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authenticated user's email address.
    /// </summary>

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT (JSON Web Token) used for authenticating subsequent API requests.
    /// </summary>
    /// <remarks>
    /// This token should be included in the Authorization header of subsequent requests to access protected resources.
    /// The token is issued by the authentication service and contains encoded user information and claims.
    /// </remarks>
    public string Token { get; set; } = string.Empty;
}