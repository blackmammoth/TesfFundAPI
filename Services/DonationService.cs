using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TesfaFundApp.Models;

namespace TesfaFundApp.Services;

/// <summary>
/// Defines the contract for managing donations.
/// </summary>
public interface IDonationService
{
    /// <summary>
    /// Retrieves a donation by its ID.
    /// </summary>
    /// <param name="id">The ID of the donation to retrieve.</param>
    /// <returns>The donation object, or null if not found.</returns>
    Task<Donation?> GetDonationByIdAsync(string? id);

    /// <summary>
    /// Creates a new donation.
    /// </summary>
    /// <param name="donation">The donation data, assumed to be valid (validated by the caller).</param>
    /// <returns>A tuple containing an error message (or null on success) and the created donation object (or null on error).</returns>
    Task<(string? ErrorMessage, Donation? MadeDonation)> MakeDonationAsync(Donation donation);

    /// <summary>
    /// Retrieves donations, optionally filtered by specified parameters.
    /// </summary>
    /// <param name="filters">The filter parameters (can be null for all donations).</param>
    /// <returns>A collection of donations.</returns>
    Task<IEnumerable<Donation>> GetFilteredDonationsAsync(DonationFilterParams? filters);

    /// <summary>
    /// Retrieves the total amount of donations made for a specific campaign.
    /// </summary>
    /// <param name="campaignId">The ID of the campaign.</param>
    /// <returns>The total amount of donations, or 0 if no donations have been made for the campaign.</returns>
    Task<int> GetTotalDonationsForCampaignAsync(string? campaignId);
}

public class DonationService : IDonationService
{
    private readonly IMongoCollection<Donation> _donationCollection;
    private readonly ICampaignService _campaignService;
    private readonly IRecipientService _recipientService;


    public DonationService(MongoDbService mongoDbService, ICampaignService campaignService, IRecipientService recipientService)
    {
        _donationCollection = mongoDbService.GetCollection<Donation>("Donations")
                             ?? throw new InvalidOperationException("Failed to get Donations collection.");
        _campaignService = campaignService;
        _recipientService = recipientService;
    }

    /// <inheritdoc/>
    public async Task<Donation?> GetDonationByIdAsync(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        try
        {
            var donation = await _donationCollection.Find(d => d.Id == id).FirstOrDefaultAsync();
            return donation;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting donation by ID: {ex}");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Donation>> GetFilteredDonationsAsync(DonationFilterParams? filters)
    {
        var filterBuilder = Builders<Donation>.Filter;
        var filter = filterBuilder.Empty;

        if (filters == null)
        {
            return await _donationCollection.Find(filter).ToListAsync();
        }

        if (!string.IsNullOrEmpty(filters.CampaignId))
        {
            filter &= filterBuilder.Eq(d => d.CampaignId, filters.CampaignId);
        }

        if (filters.MinAmount.HasValue)
        {
            filter &= filterBuilder.Gte(d => d.Amount, filters.MinAmount.Value);
        }

        if (filters.MaxAmount.HasValue)
        {
            filter &= filterBuilder.Lte(d => d.Amount, filters.MaxAmount.Value);
        }

        if (filters.StartDate.HasValue)
        {
            filter &= filterBuilder.Gte(d => d.TimeStamp, filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            // Add one day to EndDate to include donations made on that day
            filter &= filterBuilder.Lte(d => d.TimeStamp, filters.EndDate.Value.AddDays(1));
        }

        try
        {
            return await _donationCollection.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting filtered donations: {ex}");
            return new List<Donation>();
        }
    }

    /// <inheritdoc/>
    public async Task<int> GetTotalDonationsForCampaignAsync(string? campaignId)
    {
        if (string.IsNullOrEmpty(campaignId))
        {
            return 0;
        }

        try
        {
            var filter = Builders<Donation>.Filter.Eq(d => d.CampaignId, campaignId);
            var donations = await _donationCollection.Find(filter).ToListAsync();

            int totalDonations = 0;
            foreach (var donation in donations)
            {
                totalDonations += donation.Amount ?? 0;
            }

            return totalDonations;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting total donations: {ex}");
            return 0;
        }
    }

    /// <inheritdoc/>
    public async Task<(string? ErrorMessage, Donation? MadeDonation)> MakeDonationAsync(Donation donation)
    {
        // Make sure CampaignId exists
        var campaign = await _campaignService.GetCampaignByIdAsync(donation.CampaignId);
        if (campaign == null)
        {
            return ($"Campaign with CampaignId: {donation.CampaignId} does not exist.", null);
        }

        try
        {
            await _donationCollection.InsertOneAsync(donation);
            return (null, donation);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error making donation: {ex}");
            return ("An error occurred while making the donation.", null);
        }
    }
}