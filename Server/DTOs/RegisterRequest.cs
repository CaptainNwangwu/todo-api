using System.ComponentModel.DataAnnotations;

namespace Server.DTOs;

/// <summary>
/// Represents a user registration request containing the necessary information to create a new user account.
/// </summary>
/// <remarks>
/// This DTO is used when a new user wants to sign up for the application. All fields are required
/// and must meet specific validation criteria before the registration can be processed.
/// </remarks>
public class RegisterRequest
{
    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    /// <remarks>
    /// Required field with a maximum length of 255 characters.
    /// </remarks>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address, which serves as the unique identifier for login.
    /// </summary>
    /// <remarks>
    /// Required field that must be a valid email format and have a maximum length of 255 characters.
    /// This email will be used as the username for authentication purposes.
    /// </remarks>
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password for account authentication.
    /// </summary>
    /// <remarks>
    /// Required field with a minimum length of 8 characters. Passwords should be complex
    /// and secure to protect user accounts.
    /// </remarks>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}