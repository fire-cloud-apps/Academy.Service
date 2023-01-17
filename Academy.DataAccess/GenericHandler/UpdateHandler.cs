using Academy.DataAccess.Interface;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Academy.DataAccess.GenericHandler;


/// <summary>
/// A Generic Update Handler for all Models
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public class UpdateHandler<T> : IActionCommand<T>
{
    #region Variable
    private ILogger<object> _logger;
    private string _connString;
    private string _dbName;
    #endregion

    #region Constructor
    public UpdateHandler(string connString, string dbName, ILogger<object> logger)
    {
        _logger = logger;
        _connString = connString;
        _dbName = dbName;
    }

    public UpdateHandler(string connString, ILogger<object> logger)
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

    #region Command Handers
    public async Task<T> CommandHandlerAsync( FilterDefinition<T> filter, T model)
    {
        T updateModel = model;
        try
        {
            var collection = GetCollection<T>(model.GetType().Name);
            await collection.ReplaceOneAsync(filter, updateModel);
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = $"{model.GetType().Name} Storage Failed. Check Log for more details";
            _logger.LogError(
                "{ModelType}:Update Command Handler {Error} {Data}", model.GetType().Name, ex, model.ToJson());
        }
        return updateModel;
    }


    #endregion

    #region IError Handler
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; }
    #endregion
}