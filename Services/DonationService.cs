namespace TesfaFundApp.Services;

public interface IDonationService
{
    Task<Donation?> GetDonationByIdAsync(string id);
    Task<Donation?> MakeDonationAsync(Donation donation);
    Task<IEnumerable<Donation>> GetFilteredDonationsAsync(DonationFilterParams filters);
}

public class DonationService : IDonationService
{
    public Task<Donation?> GetDonationByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Donation?> MakeDonationAsync(Donation donation)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Donation>> GetFilteredDonationsAsync(DonationFilterParams filters)
    {
        throw new NotImplementedException();
    }
}
