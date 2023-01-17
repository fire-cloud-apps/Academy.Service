using Academy.DataAccess.Interface;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;
namespace Academy.DataAccess.GenericHandler;



/// <summary>
/// A Generic Delete Handler for all Models
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public class DeleteHandler<T> : IActionCommand<T>
{
    #region Variable
    private ILogger<object> _logger;
    private string _connString;
    private string _dbName;
    private bool _isSoftDelete;
    #endregion

    #region Constructor
    public DeleteHandler(string connString, string dbName, ILogger<object> logger, bool isSoftDelete =false)
    {
        _logger = logger;
        _connString = connString;
        _dbName = dbName;
        _isSoftDelete = isSoftDelete;
    }

    public DeleteHandler(string connString, ILogger<object> logger, bool isSoftDelete =false)
    {
        _logger = logger;
        _connString = connString;
        _dbName = "Academy";
        _isSoftDelete = isSoftDelete;
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
    public async Task<T> CommandHandlerAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, T model)
    {
        T actionModel = model;
        try
        {
            var collection = GetCollection<T>(model.GetType().Name);
            if (_isSoftDelete)
            {
               await  collection.UpdateOneAsync(filter, update);
            }
            else
            {
                await collection.DeleteOneAsync(filter);
            }
            
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = $"{model.GetType().Name} Storage Failed. Check Log for more details";
            //_logger.LogError($"{model.GetType().Name}::Update Command Handler", ex);
            _logger.LogError(
                "{ModelType}:Delete Command Handler {Error} {Data}", model.GetType().Name, ex, model.ToJson());
        }
        return actionModel;
    }


    #endregion

    #region IError Handler
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; }
    #endregion
}

