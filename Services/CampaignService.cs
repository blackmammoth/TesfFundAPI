using MongoDB.Driver;
using MongoDB.Bson;
using TesfaFundApp.Models;

namespace TesfaFundApp.Services;

/// <summary>
/// Defines the contract for managing campaigns.
/// </summary>
public interface ICampaignService
{
    /// <summary>
    /// Creates a new campaign.
    /// </summary>
    /// <param name="campaign">The campaign data, assumed to be valid (validated by the caller).</param>
    /// <returns>A tuple containing an error message (or null on success) and the created campaign object (or null on error).</returns>
    Task<(string? ErrorMessage, Campaign? CreatedCampaign)> CreateCampaignAsync(Campaign campaign);

    /// <summary>
    /// Retrieves a campaign by its ID.
    /// </summary>
    /// <param name="id">The ID of the campaign to retrieve.</param>
    /// <returns>The campaign object, or null if not found.</returns>
    Task<Campaign?> GetCampaignByIdAsync(string? id);

    /// <summary>
    /// Retrieves all campaigns, optionally filtered by specified parameters.
    /// </summary>
    /// <param name="filterParams">The filter parameters (can be null for all campaigns).</param>
    /// <returns>A collection of campaigns.</returns>
    Task<IEnumerable<Campaign>> GetAllCampaignsAsync(CampaignFilterParams? filterParams);

    /// <summary>
    /// Updates an existing campaign.
    /// </summary>
    /// <param name="id">The ID of the campaign to update.</param>
    /// <param name="campaign">The updated campaign data, assumed to be valid (validated by the caller).</param>
    /// <returns>A tuple containing an error message (or null on success) and the updated campaign object (or null on error).</returns>
    Task<(string? ErrorMessage, Campaign? UpdatedCampaign)> UpdateCampaignAsync(string id, Campaign campaign);

    /// <summary>
    /// Deletes a campaign by its ID.
    /// </summary>
    /// <param name="id">The ID of the campaign to delete.</param>
    /// <returns>True if the campaign was deleted successfully, false otherwise.</returns>
    Task<bool> DeleteCampaignAsync(string id); // Corrected return type

    /// <summary>
    /// Retrieves the donation progress for a campaign.
    /// </summary>
    /// <param name="campaignId">The ID of the campaign to retrieve donation progress for.</param>
    /// <returns>The donation progress information, or null if not found.</returns>
    Task<DonationProgress?> GetCampaignDonationProgressAsync(string campaignId);
}

public class CampaignService : ICampaignService
{
    private readonly IMongoCollection<Campaign> _campaignCollection;
    private readonly Lazy<IRecipientService> _recipientService;
    private readonly Lazy<IDonationService> _donationService;
    public CampaignService(MongoDbService mongoDbService, Lazy<IRecipientService> recipientService, Lazy<IDonationService> donationService)
    {
        _campaignCollection = mongoDbService.GetCollection<Campaign>("Campaigns")
                               ?? throw new InvalidOperationException("Failed to get Campaigns collection.");
        _recipientService = recipientService;
        _donationService = donationService;
    }

    /// <inheritdoc/>
    public async Task<(string? ErrorMessage, Campaign? CreatedCampaign)> CreateCampaignAsync(Campaign campaign)
    {
        // Check if the recipient exists *before* creating the campaign.
        var recipient = await _recipientService.Value.GetRecipientByIdAsync(campaign.RecipientId);
        if (recipient == null)
        {
            return ("Recipient with provided RecipientId does not exist.", null);
        }

        try
        {
            campaign.Id = Guid.NewGuid().ToString();
            await _campaignCollection.InsertOneAsync(campaign);
            return (null, campaign);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating campaign: {ex}");
            return ("An error occurred while creating the campaign.", null);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteCampaignAsync(string id)
    {
        try
        {
            var result = await _campaignCollection.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting campaign: {ex}");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Campaign>> GetAllCampaignsAsync(CampaignFilterParams? filters)
    {
        var filterBuilder = Builders<Campaign>.Filter;
        var filter = filterBuilder.Empty;

        if (filters == null)
        {
            // Return all if filters is null
            return await _campaignCollection.Find(filter).ToListAsync();
        }

        if (!string.IsNullOrEmpty(filters.Title))
        {
            filter &= filterBuilder.Regex(c => c.Title, new BsonRegularExpression(filters.Title, "i"));
        }

        if (!string.IsNullOrEmpty(filters.RecipientId))
        {
            filter &= filterBuilder.Eq(c => c.RecipientId, filters.RecipientId);
        }

        if (filters.MinFundraisingGoal.HasValue)
        {
            filter &= filterBuilder.Gte(c => c.FundraisingGoal, filters.MinFundraisingGoal.Value);
        }

        if (filters.MaxFundraisingGoal.HasValue)
        {
            filter &= filterBuilder.Lte(c => c.FundraisingGoal, filters.MaxFundraisingGoal.Value);
        }

        try
        {
            return await _campaignCollection.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting campaigns: {ex}");
            return new List<Campaign>();
        }
    }

    /// <inheritdoc/>
    public async Task<Campaign?> GetCampaignByIdAsync(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        try
        {
            var campaign = await _campaignCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
            return campaign;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting campaign by ID: {ex}");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<DonationProgress?> GetCampaignDonationProgressAsync(string campaignId)
    {
        try
        {
            int totalDonationAmount = await _donationService.Value.GetTotalDonationsForCampaignAsync(campaignId);

            var campaign = await GetCampaignByIdAsync(campaignId);
            if (campaign == null)
            {
                return null;
            }

            int fundraisingGoal = campaign.FundraisingGoal ?? 0;

            double progressPercentage = 0;
            if (fundraisingGoal > 0) // Avoid division by zero
            {
                progressPercentage = (double)totalDonationAmount / fundraisingGoal * 100;
            }

            var donationProgress = new DonationProgress
            {
                CampaignId = campaignId,
                TotalDonations = totalDonationAmount,
                FundraisingGoal = fundraisingGoal,
                ProgressPercentage = progressPercentage
            };

            return donationProgress;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting donation progress: {ex}");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<(string? ErrorMessage, Campaign? UpdatedCampaign)> UpdateCampaignAsync(string id, Campaign campaign)
    {
        try
        {
            // Check if the related Recipient exists
            var recipient = await _recipientService.Value.GetRecipientByIdAsync(campaign.RecipientId);
            if (recipient == null)
            {
                return ("Recipient with provided RecipientId does not exist.", null);
            }

            var filter = Builders<Campaign>.Filter.Eq(c => c.Id, id);

            campaign.Id = id;

            var result = await _campaignCollection.ReplaceOneAsync(filter, campaign);

            if (result.ModifiedCount > 0)
            {
                var updatedCampaign = await _campaignCollection.Find(filter).FirstOrDefaultAsync();
                return (null, updatedCampaign);
            }
            else
            {
                return ("Campaign not found or not modified.", null);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating campaign: {ex}");
            return ("An error occurred while updating the campaign.", null);
        }
    }
}