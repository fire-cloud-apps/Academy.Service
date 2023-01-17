using Academy.DataAccess.Interface;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Academy.DataAccess.GenericHandler;

/// <summary>
/// A Generic Get Handler for all Models Gets the data by 'Id'
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public class GetHandler<T> : IActionQuery<T>
{
    #region Variable
    private ILogger<object> _logger;
    private string _connString;
    private string _dbName;
    #endregion

    #region Constructor
    public GetHandler(string connString, string dbName, ILogger<object> logger)
    {
        _logger = logger;
        _connString = connString;
        _dbName = dbName;
    }

    public GetHandler(string connString, ILogger<object> logger)
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

    #region Get Handlers
    public async Task<IList<T>> GetHandlerAsync
    (
        FilterDefinition<T> filter, 
        SortDefinition<T>? sort,  
        Pagination? pagination
    )
    {
        IList<T> getModels = null;
        try
        {
            var collection = GetCollection<T>(typeof(T).Name);
            pagination ??= new Pagination()
            {
                Page = 1,
                PageSize = 10
            };
            
            var result = collection.Find(filter)
                .Sort(sort)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Limit(pagination.PageSize);
            getModels = await result.ToListAsync();
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = $"{typeof(T).Name} Get Failed. Check Log for more details";
            _logger.LogError(
                "{ModelType}:Get Handler {Error} {Data}", typeof(T).Name, ex, pagination.ToJson());
        }

        return getModels;
    }

    #endregion

    #region Get Handlers which gets only required Column using Project
    public async Task<IList<T>> GetHandlerAsync
    (
        FilterDefinition<T> filter,
        SortDefinition<T>? sort,
        Pagination? pagination,
        ProjectionDefinition<T> project
    )
    {
        IList<T> getModels = null;
        try
        {
            var collection = GetCollection<T>(typeof(T).Name);
            pagination ??= new Pagination()
            {
                Page = 1,
                PageSize = 10
            };

            var result = collection.Find(filter)
                .Sort(sort)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Limit(pagination.PageSize)
                .Project<T>(project)
                ;
            
                //.Project(project) ;
            getModels = await result.ToListAsync();
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = $"{typeof(T).Name} Get Failed. Check Log for more details";
            _logger.LogError(
                "{ModelType}:Get Handler {Error} {Data}", typeof(T).Name, ex, pagination.ToJson());
        }

        return getModels;
    }

    #endregion

    #region IError Handler
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; }
    #endregion
}