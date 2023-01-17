using Academy.DataAccess.Interface;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Academy.DataAccess.GenericHandler;

/// <summary>
/// A Generic Get Handler for all Models Gets the data by 'Id'
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public class GetDocumentCount<T> : IActionQuery<T>
{
    #region Variable
    private ILogger<object> _logger;
    private string _connString;
    private string _dbName;
    #endregion

    #region Constructor
    public GetDocumentCount(string connString, string dbName, ILogger<object> logger)
    {
        _logger = logger;
        _connString = connString;
        _dbName = dbName;
    }

    public GetDocumentCount(string connString, ILogger<object> logger)
    {
        _logger = logger;
        _connString = connString;
        _dbName = "Academy";
    }

    private IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        #region Sample Connection String
        //#Local MongoDB: "mongodb://localhost:27017"
        //#Cloud MongoDB: "mongodb+srv://fc_client_admin:fc.Serverless.mongo@cluster0.acxm4.mongodb.net/ClientDB?authSource=admin&replicaSet=atlas-g9u9yl-shard-0&w=majority&readPreference=primary&retryWrites=true&ssl=true"
        //Object dispose or connection will be handled automatically by mongo C# Drive
        //https://stackoverflow.com/questions/32703051/properly-shutting-down-mongodb-database-connection-from-c-sharp-2-1-driver
        #endregion

        var client = new MongoClient(_connString);
        var database = client.GetDatabase(_dbName);
        var collection = database.GetCollection<T>(collectionName);
        return collection;
    }

    #endregion

    #region Get Handers
    public async Task<long> GetCountAsync(FilterDefinition<T> filter)
    {
        long resultCount = 0;
        try
        {
            var collection = GetCollection<T>(typeof(T).Name);
            //resultCount = await collection.CountDocumentsAsync(new BsonDocument());
            resultCount = await collection.CountDocumentsAsync(filter);
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = $"{typeof(T).Name} Get Count Failed. Check Log for more details";
            //_logger.LogError($"{typeof(T).Name}::Get Command Handler", ex);
            _logger.LogError(
                "{ModelType}:GetCount Handler {Error}", typeof(T).Name, ex);
        }
        return resultCount;
    }


    #endregion

    #region IError Handler
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; }
    #endregion
}