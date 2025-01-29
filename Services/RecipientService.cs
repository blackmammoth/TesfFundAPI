
namespace TesfaFundApp.Services;

using MongoDB.Bson;
using MongoDB.Driver;
using TesfaFundApp.Models;

/// <summary>
/// Defines the contract for managing recipients.
/// </summary>
public interface IRecipientService
{
    /// <summary>
    /// Creates a new recipient.
    /// </summary>
    /// <param name="recipient">The recipient data.</param>
    /// <returns>A tuple containing an error message (or null on success) and the created recipient object (or null on error).</returns>
    Task<(string? ErrorMessage, Recipient? CreatedRecipient)> CreateRecipientAsync(Recipient recipient);

    /// <summary>
    /// Retrieves a recipient by their ID.
    /// </summary>
    /// <param name="id">The ID of the recipient to retrieve.</param>
    /// <returns>The recipient object, or null if not found.</returns>
    Task<Recipient?> GetRecipientByIdAsync(string? id);

    /// <summary>
    /// Updates an existing recipient.
    /// </summary>
    /// <param name="id">The ID of the recipient to update.</param>
    /// <param name="recipient">The updated recipient data.</param>
    /// <returns>A tuple containing an error message (or null on success) and the updated recipient object (or null on error).</returns>
    Task<(string? ErrorMessage, Recipient? UpdatedRecipient)> UpdateRecipientAsync(string id, Recipient recipient);

    /// <summary>
    /// Deletes a recipient by their ID.
    /// </summary>
    /// <param name="id">The ID of the recipient to delete.</param>
    /// <returns>True if the recipient was deleted, false otherwise.</returns>
    Task<string?> DeleteRecipientAsync(string id);

    /// <summary>
    /// Retrieves all recipients, optionally filtered by specified parameters.
    /// </summary>
    /// <param name="filterParams">The filter parameters (can be null for all recipients).</param>
    /// <returns>A collection of recipients.</returns>
    Task<IEnumerable<Recipient>> GetAllRecipientsAsync(RecipientFilterParams? filterParams);
}

public class RecipientService : IRecipientService
{
    private readonly IMongoCollection<Recipient> _recipientCollection;
    private readonly ICampaignService _campaignService;

    public RecipientService(MongoDbService mongoDbService, ICampaignService campaignService)
    {
        _recipientCollection = mongoDbService.GetCollection<Recipient>("Recipients") ?? throw new InvalidOperationException("Failed to get Recipients collection.");
        _campaignService = campaignService;
    }

    /// <inheritdoc/>
    public async Task<(string? ErrorMessage, Recipient? CreatedRecipient)> CreateRecipientAsync(Recipient recipient)
    {
        try
        {
            recipient.Email = recipient!.Email!.ToLower();
            await _recipientCollection.InsertOneAsync(recipient);
            return (null, recipient);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating recipient: {ex}");
            return ("An error occurred while creating the recipient.", null);
        }
    }

    /// <inheritdoc/>
    public async Task<string?> DeleteRecipientAsync(string id)
    {
        try
        {
            // Check if the recipient has any associated campaigns (using filters)
            var campaignFilter = new CampaignFilterParams { RecipientId = id };
            var campaigns = await _campaignService.GetAllCampaignsAsync(campaignFilter);

            if (campaigns.Any())
            {
                Console.WriteLine($"Recipient with ID {id} has associated campaigns. Deletion failed.");
                return ($"Recipient with ID {id} has associated campaigns. Deletion failed.");
            }

            // If no campaigns are associated, proceed with deletion
            var filter = Builders<Recipient>.Filter.Eq(r => r.Id, id);
            var result = await _recipientCollection.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                return null;
            }
            else
            {
                return $"Recipient with ID {id} not found.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting recipient: {ex}");
            return $"An error occurred while deleting the recipient: {ex.Message}";
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Recipient>> GetAllRecipientsAsync(RecipientFilterParams? filterParams)
    {
        var filter = Builders<Recipient>.Filter.Empty;

        if (filterParams != null)
        {
            if (!string.IsNullOrEmpty(filterParams.FirstName))
            {
                filter = Builders<Recipient>.Filter.And(filter, Builders<Recipient>.Filter.Regex("FirstName", new BsonRegularExpression(filterParams.FirstName, "i")));
            }

            if (!string.IsNullOrEmpty(filterParams.MiddleName))
            {
                filter = Builders<Recipient>.Filter.And(filter, Builders<Recipient>.Filter.Regex("MiddleName", new BsonRegularExpression(filterParams.MiddleName, "i")));
            }

            if (!string.IsNullOrEmpty(filterParams.LastName))
            {
                filter = Builders<Recipient>.Filter.And(filter, Builders<Recipient>.Filter.Regex("LastName", new BsonRegularExpression(filterParams.LastName, "i")));
            }

            if (!string.IsNullOrEmpty(filterParams.Email))
            {
                filter = Builders<Recipient>.Filter.And(filter, Builders<Recipient>.Filter.Regex("Email", new BsonRegularExpression(filterParams.Email, "i")));
            }
        }

        try
        {
            return await _recipientCollection.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting recipients: {ex}");
            return new List<Recipient>();
        }
    }

    /// <inheritdoc/>
    public async Task<Recipient?> GetRecipientByIdAsync(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        try
        {
            var filter = Builders<Recipient>.Filter.Eq(r => r.Id, id);
            var recipient = await _recipientCollection.Find(filter).FirstOrDefaultAsync();
            return recipient;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting recipient by ID: {ex}");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<(string? ErrorMessage, Recipient? UpdatedRecipient)> UpdateRecipientAsync(string id, Recipient recipient)
    {
        try
        {
            var filter = Builders<Recipient>.Filter.Eq(r => r.Id, id);

            recipient.Id = id;

            var result = await _recipientCollection.ReplaceOneAsync(filter, recipient);

            if (result.ModifiedCount > 0)
            {
                var updatedRecipient = await _recipientCollection.Find(filter).FirstOrDefaultAsync();
                return (null, updatedRecipient);
            }
            else
            {
                return ("Recipient not found or not modified.", null);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating recipient: {ex}");
            return ("An error occurred while updating the recipient.", null);
        }
    }
}
