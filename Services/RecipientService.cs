
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
