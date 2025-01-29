namespace TesfaFundApp;
public class Campaign
{
    public String? Id { get; set; }
    public String? Title { get; set; }
    public String? Description { get; set; }
    public String? FundraisingGoal { get; set; }
    public Donation[]? Donations { get; set; }
}