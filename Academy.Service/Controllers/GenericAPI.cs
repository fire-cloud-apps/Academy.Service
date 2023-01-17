using Academy.DataAccess;
using Academy.DataAccess.GenericHandler;
using Academy.DataAccess.Interface;
using Academy.Service.Utility;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Academy.Service.Controllers;

/// <summary>
/// A Generic API Controller to handle all the Basic operations of CRUD.
/// </summary>
/// <typeparam name="T">Targetting Model Type</typeparam>
public class GenericAPI<T> : IGenericAPI<T> where T : class
{
    #region Global Variables
    private readonly ILogger<object> _logger;
    private readonly AppSettings _appSettings;
    private IActionCommand<T> _actionCommand;
    private IActionQuery<T> _getCommand;
    #endregion
    
    #region Constructor
   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="appSettings"></param>
    public GenericAPI
    (
        ILogger<object> logger,
        IOptions<AppSettings> appSettings
    )
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        
    }
    #endregion
    
    #region CRUD

    /// <summary>
    /// Generic Create Handler
    /// </summary>
    /// <param name="model">Model to insert data into Database</param>
    /// <returns>return's the result</returns>
    public async Task<T> Create(T model)
    {
        _actionCommand = new CreateHandler<T>(
            _appSettings.DBSettings.ClientDB, 
            _appSettings.DBSettings.DataBaseName, 
            _logger);
        var returnModel = await _actionCommand.CommandHandlerAsync(model);
        return ResultHandler(_actionCommand, returnModel);      
    }
    
    /// <summary>
    /// Generic Update Handler
    /// </summary>
    /// <param name="model">Model to insert data into Database</param>
    /// <param name="filter">Filter definition to perform update</param>
    /// <returns>return's the result</returns>
    public async Task<T> Update(T model, FilterDefinition<T> filter)
    {
        _actionCommand = new UpdateHandler<T>(
            _appSettings.DBSettings.ClientDB, 
            _appSettings.DBSettings.DataBaseName, 
            _logger);
        var returnModel = await _actionCommand.CommandHandlerAsync(filter, model);
        return ResultHandler(_actionCommand, returnModel);        
    }
    
    public async Task<T> UpdatePartial(T model,  
        UpdateDefinition<T> updateFilter, 
        FilterDefinition<T> filter)
    {
        _actionCommand = new UpdatePartialHandler<T>(
            _appSettings.DBSettings.ClientDB, 
            _appSettings.DBSettings.DataBaseName, 
            _logger);
        var returnModel = await _actionCommand.CommandHandlerAsync(filter, updateFilter, model);
        return ResultHandler(_actionCommand, returnModel);
    }

    /// <summary>
    /// Generic Delete Handler
    /// </summary>
    /// <param name="model">Model to insert data into Database</param>
    /// <param name="updateFilter">Update filter definition to perform update</param>
    /// <param name="filter">Filter definition to perform update</param>
    /// <param name="soft">if soft is true, it is not deleted hardly</param>
    /// <returns>return's the result</returns>
    public async Task<T> Delete
    (T model, 
        UpdateDefinition<T> updateFilter, 
        FilterDefinition<T> filter, 
        bool soft = true)
    {
        _actionCommand = new DeleteHandler<T>
            (_appSettings.DBSettings.ClientDB, _appSettings.DBSettings.DataBaseName, _logger, soft);
        var returnModel = await _actionCommand.CommandHandlerAsync(filter, updateFilter, model);
        
        return ResultHandler(_actionCommand, returnModel);
    }

    #endregion
    
    #region Get Data with Filter and Pagination
    /// <summary>
    /// Get the data by any filters.
    /// </summary>
    /// <param name="filter">Get filter by Filter Definition</param>
    /// <param name="sort">sort by defining the Sort Definition</param>
    /// <param name="pagination">Pagination data</param>
    /// <returns>returns model data</returns>
    public async Task<IList<T>> GetFilter(FilterDefinition<T> filter, 
        SortDefinition<T> sort = null, Pagination pagination = null)
    {
        _getCommand = new GetHandler<T>
        (
            connString: _appSettings.DBSettings.ClientDB,
            _appSettings.DBSettings.DataBaseName,
            _logger
        );
        
        var model = await _getCommand.GetHandlerAsync
        (
            filter: filter,
            sort: sort,
            pagination: pagination
        );
        return ResultHandler(_getCommand, model);
    }
    #endregion

    #region Get Data with Filter, Projection and Pagination
    /// <summary>
    /// Get the data by any filters.
    /// </summary>
    /// <param name="filter">Get filter by Filter Definition</param>
    /// <param name="projection">Returns only the required field.</param>
    /// <param name="sort">sort by defining the Sort Definition</param>
    /// <param name="pagination">Pagination data</param>
    /// <returns>returns model data</returns>
    public async Task<IList<T>> GetProjectFilter(FilterDefinition<T> filter, ProjectionDefinition<T> projection,
        SortDefinition<T> sort = null, Pagination pagination = null )
    {
        _getCommand = new GetHandler<T>
        (
            connString: _appSettings.DBSettings.ClientDB,
            _appSettings.DBSettings.DataBaseName,
            _logger
        );

        var model = await _getCommand.GetHandlerAsync
        (
            filter: filter,
            sort: sort,
            pagination: pagination,
            project: projection
        );
        return ResultHandler(_getCommand, model);
    }
    #endregion

    private R ResultHandler<R>(IError errorHandler, R result) 
    {
        if (errorHandler.IsError)
        {
            throw new Exception(_getCommand.ErrorMessage);
        }
        return result;
    }

    #region Scalar Methods

    /// <summary>
    /// Get the collection record count for the given table/model
    /// </summary>
    /// <returns>count of records/documents</returns>
    public async Task<long> GetRecordCount(FilterDefinition<T> filter)
    {
        _getCommand = new GetDocumentCount<T>
        (
            connString: _appSettings.DBSettings.ClientDB,
            _appSettings.DBSettings.DataBaseName,
            _logger
        );
        var count = await _getCommand .GetCountAsync(filter);
        return ResultHandler(_getCommand, count);
    }

    #endregion
}



public interface IGenericAPI<T> where T : class
{
    Task<T> Create(T model);
    Task<T> Update(T model, FilterDefinition<T> filter);
    Task<T> UpdatePartial(T model, UpdateDefinition<T> updateFilter, FilterDefinition<T> filter);
    Task<T> Delete(T model, UpdateDefinition<T> updateFilter, FilterDefinition<T> filter, bool soft = true);
    Task<IList<T>> GetFilter(FilterDefinition<T> filter,
        SortDefinition<T> sort = null, Pagination pagination = null);
    Task<IList<T>> GetProjectFilter(FilterDefinition<T> filter, 
        ProjectionDefinition<T> project, SortDefinition<T> sort = null, Pagination pagination = null);
    Task<long> GetRecordCount(FilterDefinition<T> filter);
    //Task<T> Find(FilterDefinition<T> filter);
    //Task<T> FindOne(FilterDefinition<T> filter);
    //Task<T> FindOneAndUpdate(FilterDefinition<T> filter, UpdateDefinition<T> updateFilter, bool isUpsert);
    //Task<T> FindOneAndDelete(FilterDefinition<T> filter);
}