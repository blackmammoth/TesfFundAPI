using System.Numerics;

namespace TesfaFundApp;

public class Donation
{
    public String? Id { get; set; }
    public int? Amount { get; set; }
    public DateTime? TimeStamp { get; set; }
    public String? CampaignId { get; set; }
}

public class DonationProgress
{
    public string CampaignId { get; set; }
    public int TotalDonations { get; set; }
    public int FundraisingGoal { get; set; }
    public double ProgressPercentage { get; set; }
}

public class DonationFilterParams
{
    public string? CampaignId { get; set; }
    public int? MinAmount { get; set; }
    public int? MaxAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

