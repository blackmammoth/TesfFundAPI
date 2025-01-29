
namespace TesfaFundApp.Services;

public interface IRecipientService
{
    Task<Recipient> CreateRecipientAsync(Recipient recipient);
    Task<Recipient?> GetRecipientByIdAsync(string id);
    Task<bool> UpdateRecipientAsync(string id, Recipient recipient);
    Task<bool> DeleteRecipientAsync(string id);
    Task<IEnumerable<Recipient>> GetAllRecipientsAsync(RecipientFilterParams filterParams);
}


public class RecipientService : IRecipientService
{
    public async Task<Recipient> CreateRecipientAsync(Recipient recipient)
    {
        throw new NotImplementedException();
    }

    public async Task<Recipient?> GetRecipientByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateRecipientAsync(string id, Recipient recipient)
    {
        var filter = Builders<Recipient>.Filter.Eq(r => r.Id, id); 
        var update = Builders<Recipient>.Update
            .Set(r => r.Name, recipient.Name) 
            .Set(r => r.Email, recipient.Email) 
            .Set(r => r.Address, recipient.Address); 

        var result = await _recipientCollection.UpdateOneAsync(filter, update); 
        return result.ModifiedCount > 0; 
    }

    public async Task<bool> DeleteRecipientAsync(string id)
    {
        var filter = Builders<Recipient>.Filter.Eq(r => r.Id, id); 
        var result = await _recipientCollection.DeleteOneAsync(filter); 
        return result.DeletedCount > 0; 
    }

    public Task<IEnumerable<Recipient>> GetAllRecipientsAsync(RecipientFilterParams filterParams)
    {
        throw new NotImplementedException();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
