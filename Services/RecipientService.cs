
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
     private readonly List<Recipient> _recipients = new();

        public async Task<Recipient> CreateRecipientAsync(Recipient recipient)
        {
            if (recipient == null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            // Simulate an async operation
            return await Task.Run(() =>
            {
                recipient.Id = _recipients.Count + 1;
                _recipients.Add(recipient);
                return recipient;
            });
        }

        public async Task<List<Recipient>> GetAllRecipientsAsync()
        {
            return await Task.FromResult(_recipients);
        }
    }
  }

    public async Task<Recipient?> GetRecipientByIdAsync(string id)
    {
    	// Check if the id is valid
    	if (string.IsNullOrWhiteSpace(id))
    	{
        	throw new ArgumentException("The provided ID is null, empty, or whitespace.", nameof(id));
    	}

   	 // Return the recipient matching the string ID
    	return await Task.Run(() => _recipients.FirstOrDefault(r => r.Id == id));
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
