using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TesfaFundApp.Models;

/// <summary>
/// Represents a donation made to a campaign.
/// </summary>
public class Donation
{
    /// <summary>
    /// The unique identifier for the donation (UUID).
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }

    /// <summary>
    /// The amount of the donation.
    /// </summary>
    [Required(ErrorMessage = "Donation amount is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Donation amount must be greater than 0.")]
    public int? Amount { get; set; }

    /// <summary>
    /// The timestamp of the donation. Defaults to the current UTC time.
    /// </summary>
     [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The ID of the campaign the donation is associated with.
    /// </summary>
    [Required(ErrorMessage = "CampaignId is required.")]
    [ValidGuid(ErrorMessage = "CampaignId must be a valid GUID.")]
    public string? CampaignId { get; set; }
}

/// <summary>
/// Represents the donation progress for a campaign.
/// </summary>
public class DonationProgress
{
    /// <summary>
    /// The ID of the campaign.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public string? CampaignId { get; set; }

    /// <summary>
    /// The total amount of donations received for the campaign.
    /// </summary>
    public int TotalDonations { get; set; }

    /// <summary>
    /// The target fundraising goal for the campaign.
    /// </summary>
    public int FundraisingGoal { get; set; }

    /// <summary>
    /// The progress percentage of the fundraising goal.
    /// </summary>
    public double ProgressPercentage { get; set; }
}

/// <summary>
/// Represents the parameters used to filter donations.
/// </summary>
public class DonationFilterParams
{
    /// <summary>
    /// Filter donations by the ID of the campaign.
    /// </summary>
    [ValidGuid(ErrorMessage = "CampaignId must be a valid GUID.")]
    public string? CampaignId { get; set; }

    /// <summary>
    /// Filter donations with an amount greater than or equal to this value.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum amount must be between {1} and {2}.")]
    public int? MinAmount { get; set; }

    /// <summary>
    /// Filter donations with an amount less than or equal to this value.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum amount must be between {1} and {2}.")]
    public int? MaxAmount { get; set; }

    /// <summary>
    /// Filter donations starting from this date and time.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter donations ending before this date and time.
    /// </summary>
    public DateTime? EndDate { get; set; }
}