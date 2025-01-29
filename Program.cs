using TesfaFundApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<ICampaignService, CampaignService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
