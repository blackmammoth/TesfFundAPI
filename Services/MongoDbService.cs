using MongoDB.Driver;
using TesfaFundApp.Models;

namespace TesfaFundApp.Services;

public class MongoDbService
{
    private readonly IMongoDatabase? _database;

    public MongoDbService(IConfiguration configuration)
    {
        var host = configuration.GetSection("MongoDbSettings:Host").Value;
        var port = configuration.GetSection("MongoDbSettings:Port").Value;
        var databaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value;
        var user = configuration.GetSection("MongoDbSettings:User").Value;

        // Password setup for the MongoDb instance in docker compose
        var password = configuration.GetSection("MongoDbSettings:Password").Value;

        string connectionString;
        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
        {

            connectionString = $"mongodb://{user}:{password}@{host}:{port}";
        }
        else
        {
            connectionString = $"mongodb://{host}:{port}";
        }

        try
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to MongoDb: {ex.Message}");
        }
    }

    public IMongoCollection<T>? GetCollection<T>(string collectionName)
    {
        if (_database == null)
        {
            Console.WriteLine("Error: Database connection is not established.");
            return null;
        }
        return _database?.GetCollection<T>(collectionName);
    }
}