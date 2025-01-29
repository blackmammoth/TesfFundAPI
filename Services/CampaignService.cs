using MongoDB.Driver;

namespace TesfaFundApp.Services;

public interface ICampaignService
{
    Task<Campaign> CreateCampaignAsync(Campaign campaign, string userId);
    Task<Campaign?> GetCampaignByIdAsync(string id);
    Task<IEnumerable<Campaign>> GetAllCampaignsAsync(CampaignFilterParams filterParams);
    Task<bool> UpdateCampaignAsync(string id, Campaign campaign);
    Task<bool> DeleteCampaignAsync(string id);
    Task<DonationProgress?> GetCampaignDonationProgressAsync(string campaignId);
}

public class CampaignService : ICampaignService
{
    private readonly IMongoCollection<Campaign> _campaignCollection;
    public CampaignService(MongoDbService mongoDbService)
    {
        _campaignCollection = mongoDbService.GetCollection<Campaign>("Campaigns")
                               ?? throw new InvalidOperationException("Failed to get Campaigns collection.");
    }

    public async Task<Campaign> CreateCampaignAsync(Campaign campaign, string userId)
    {
        // Handle the case without throwing exceptions, return null or appropriate result if needed
        campaign.UserId = userId;
        await _campaignCollection.InsertOneAsync(campaign);
        return campaign;
    }

    public async Task<Campaign?> GetCampaignByIdAsync(string id)
    {
    var filter = Builders<Campaign>.Filter.Eq(c => c.Id, id);
    var campaign = await _campaignCollection.Find(filter).FirstOrDefaultAsync();
    return campaign;
    }


    public async Task<IEnumerable<Campaign>> GetAllCampaignsAsync(CampaignFilterParams filters)
    {
        var filterBuilder = Builders<Campaign>.Filter;
        var filter = filterBuilder.Empty; // Start with an empty filter

        if (!string.IsNullOrEmpty(filters.Title))
        {
            filter &= filterBuilder.Regex(c => c.Title, new MongoDB.Bson.BsonRegularExpression(filters.Title, "i"));
        }

        if (!string.IsNullOrEmpty(filters.UserId))
        {
            filter &= filterBuilder.Eq(c => c.UserId, filters.UserId);
        }

        if (filters.MinFundraisingGoal.HasValue)
        {
            filter &= filterBuilder.Gte(c => c.FundraisingGoal, filters.MinFundraisingGoal);
        }

        if (filters.MaxFundraisingGoal.HasValue)
        {
            filter &= filterBuilder.Lte(c => c.FundraisingGoal, filters.MaxFundraisingGoal);
        }

        return await _campaignCollection.Find(filter).ToListAsync(); // Returns an empty list if no matches
    }

    public async Task<bool> UpdateCampaignAsync(string id, Campaign campaign)
    {
        var filter = Builders<Campaign>.Filter.Eq(c => c.Id, id);
        var update = Builders<Campaign>.Update
            .Set(c => c.Title, campaign.Title)
            .Set(c => c.Description, campaign.Description)
            .Set(c => c.FundraisingGoal, campaign.FundraisingGoal);

        var result = await _campaignCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteCampaignAsync(string id)
    {
        // Return false if the deletion fails, instead of throwing an exception
        return false; // Replace with actual implementation
    }

    public async Task<DonationProgress?> GetCampaignDonationProgressAsync(string campaignId)
    {
        // Return null if the progress can't be calculated, instead of throwing an exception
        return null; // Replace with actual implementation
    }
}