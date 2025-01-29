using MongoDB.Driver;
using TesfaFundApp;

namespace TesfaFundApp.Services;

public interface IDonationService
{
    Task<Donation?> GetDonationByIdAsync(string id);
    Task<Donation?> MakeDonationAsync(Donation donation);
    Task<IEnumerable<Donation>> GetFilteredDonationsAsync(DonationFilterParams filters);
}

public class DonationService : IDonationService
{
    private readonly IMongoCollection<Donation> _donationCollection;
    private readonly ICampaignService _campaignService;

    public DonationService(MongoDbService mongoDbService, ICampaignService campaignService)
    {
        _donationCollection = mongoDbService.GetCollection<Donation>("Donations")
                             ?? throw new InvalidOperationException("Failed to get Donations collection.");
        _campaignService = campaignService;
    }

    public async Task<Donation?> GetDonationByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
        {
            return null;
        }

        var filter = Builders<Donation>.Filter.Eq(d => d.Id, id);
        return await _donationCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Donation?> MakeDonationAsync(Donation donation)
    {
        if (donation == null || donation.Amount <= 0 || string.IsNullOrEmpty(donation.CampaignId) || !Guid.TryParse(donation.CampaignId, out _))
        {
            return null;
        }

        var campaign = await _campaignService.GetCampaignByIdAsync(donation.CampaignId);
        if (campaign == null)
        {
            return null;
        }

        await _donationCollection.InsertOneAsync(donation);
        return donation;
    }

    public Task<IEnumerable<Donation>> GetFilteredDonationsAsync(DonationFilterParams filters)
    {
        throw new NotImplementedException();
    }
}