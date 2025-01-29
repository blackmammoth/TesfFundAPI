using MongoDB.Driver;

namespace TesfaFundApp;

public class MongoDbService
{
    private readonly IMongoDatabase? _database;

    public MongoDbService(IConfiguration configuration)
    {
        var host = configuration.GetSection("MongoDbSettings:Host").Value;
        var port = configuration.GetSection("MongoDbSettings:Port").Value;
        var databaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value;
        var user = configuration.GetSection("DatabaseSettings:User").Value;

        // Password setup for the MongoDb instance in docker compose
        var password = configuration.GetSection("DatabaseSettings:Password").Value;

        string connectionString = "";
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

    public IMongoCollection<T>? GetCollection<T>(string collectionName) {
        return _database?.GetCollection<T>(collectionName);
    }
}