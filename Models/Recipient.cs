using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TesfaFundApp.Models;

/// <summary>
/// Represents a recipient of donations.
/// </summary>
public class Recipient
{
    /// <summary>
    /// The unique identifier for the recipient (UUID stored as a string in MongoDB).
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }

    /// <summary>
    /// The first name of the recipient.
    /// </summary>
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(200, ErrorMessage = "First name cannot exceed 200 characters.")]
    public string? FirstName { get; set; }

    /// <summary>
    /// The middle name of the recipient.
    /// </summary>
    [StringLength(200, ErrorMessage = "Middle name cannot exceed 200 characters.")] // Optional
    public string? MiddleName { get; set; }

    /// <summary>
    /// The last name of the recipient.
    /// </summary>
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(200, ErrorMessage = "Last name cannot exceed 200 characters.")]
    public string? LastName { get; set; }

    /// <summary>
    /// The email address of the recipient.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    /// <summary>
    /// The password hash of the recipient.  
    /// <para>This property stores the *hashed* password, not the plain text password.</para>
    /// </summary>
    [Required(ErrorMessage = "Password hash is required.")]
    public string? PasswordHash { get; set; }
}

/// <summary>
/// Represents the parameters used to filter recipients.
/// </summary>
public class RecipientFilterParams
{
    /// <summary>
    /// Filter recipients by first name.
    /// </summary>
    [StringLength(200, ErrorMessage = "First name cannot exceed 200 characters.")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Filter recipients by middle name.
    /// </summary>
    [StringLength(200, ErrorMessage = "Middle name cannot exceed 200 characters.")]
    public string? MiddleName { get; set; }

    /// <summary>
    /// Filter recipients by last name.
    /// </summary>
    [StringLength(200, ErrorMessage = "Last name cannot exceed 200 characters.")]
    public string? LastName { get; set; }

    /// <summary>
    /// Filter recipients by email address.
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }
}