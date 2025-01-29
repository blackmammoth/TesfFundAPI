using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TesfaFundApp;
public class Campaign
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public String? Id { get; set; }
    public String? Title { get; set; }
    public String? Description { get; set; }
    public int? FundraisingGoal { get; set; }
    public String? UserId { get; set; }
}

public class CampaignFilterParams
{
    public string? Title { get; set; }
    public string? UserId { get; set; }
    public int? MinFundraisingGoal { get; set; }
    public int? MaxFundraisingGoal { get; set; }
}