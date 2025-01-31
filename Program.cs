using System.Reflection;
using Microsoft.OpenApi.Models;
using TesfaFundApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TesfaFund API",
        Version = "v1",
    });

    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<ICampaignService, CampaignService>();
builder.Services.AddSingleton<IDonationService, DonationService>();
builder.Services.AddSingleton<IRecipientService, RecipientService>();

builder.Services.AddSingleton(provider => new Lazy<ICampaignService>(provider.GetRequiredService<ICampaignService>));
builder.Services.AddSingleton(provider => new Lazy<IRecipientService>(provider.GetRequiredService<IRecipientService>));
builder.Services.AddSingleton(provider => new Lazy<IDonationService>(provider.GetRequiredService<IDonationService>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();