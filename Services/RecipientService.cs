
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
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteRecipientAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Recipient>> GetAllRecipientsAsync(RecipientFilterParams filterParams)
    {
        throw new NotImplementedException();
    }
}
