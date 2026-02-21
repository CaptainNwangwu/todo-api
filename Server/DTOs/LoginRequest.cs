using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Server.DTOs;

/// <summary>
/// Represents a user login request containing the credentials needed to authenticate an existing user.
/// </summary>
/// <remarks>
/// This DTO is used when a user wants to sign in to the application. All fields are required
/// and must be valid before the login authentication can be processed.
/// </remarks>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the user's email address used for authentication.
    /// </summary>
    /// <remarks>
    /// Required field that must be a valid email format. This email serves as the unique identifier
    /// for user login and must match the email registered during account creation.
    /// </remarks>
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's password for account authentication.
    /// </summary>
    /// <remarks>
    /// Required field containing the user's account password. This must match the password
    /// set during registration to successfully authenticate the user.
    /// </remarks>
    [Required]
    [MinLength(8)]
    public required string Password { get; set; }
}
