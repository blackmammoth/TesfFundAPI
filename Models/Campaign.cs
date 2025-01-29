using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TesfaFundApp.Models;

/// <summary>
/// A fundraising campaign that is created by a recipient.
/// </summary>
public class Campaign
{
    /// <summary>
    /// The unique identifier for the campaign (MongoDB ObjectId).
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }

    /// <summary>
    /// The title of the campaign.
    /// </summary>
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string? Title { get; set; }

    /// <summary>
    /// A detailed description of the campaign.
    /// </summary>
    [Required(ErrorMessage = "Description is required.")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters.")]
    public string? Description { get; set; }

    /// <summary>
    /// The target fundraising goal for the campaign.
    /// </summary>
    [Required(ErrorMessage = "Fundraising goal is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Fundraising goal must be between {1} and {2}")]
    public int? FundraisingGoal { get; set; }

    /// <summary>
    /// The ID of the recipient who created the campaign (must be a valid GUID).
    /// </summary>
    [Required(ErrorMessage = "RecipientId is required.")]
    [ValidGuid(ErrorMessage = "RecipientId must be a valid GUID.")]
    public string? RecipientId { get; set; }
}

/// <summary>
/// Filter and search parameters for campaigns.
/// </summary>
public class CampaignFilterParams
{
    /// <summary>
    /// Filter campaigns by title (case-insensitive).
    /// </summary>
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string? Title { get; set; }

    /// <summary>
    /// Filter campaigns by the ID of the user who created the campaign.
    /// </summary>
    [ValidGuid(ErrorMessage = "RecipientId must be a valid UUID.")]
    public string? RecipientId { get; set; }

    /// <summary>
    /// Filter campaigns with a fundraising goal greater than or equal to this value.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum fundraising goal must be between {1} and {2}")]
    public int? MinFundraisingGoal { get; set; }

    /// <summary>
    /// Filter campaigns with a fundraising goal less than or equal to this value.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum fundraising goal must be between {1} and {2}")]
    public int? MaxFundraisingGoal { get; set; }
}