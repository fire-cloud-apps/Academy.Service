using Academy.DataAccess;
using Academy.DataAccess.GenericHandler;
using Academy.DataAccess.Interface;
using Academy.Entity.Management;
using Academy.Service.Utility;
using Academy.Service.Utility.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
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
    private readonly DBSettings _dbSettings;
    private IActionCommand<T> _actionCommand;
    private IActionQuery<T> _getCommand;
    #endregion
    
    #region Constructor
   
    /// <summary>
    /// Generic API Constructor, by default it uses 'ServiceDB' from AppSettings
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="appSettings"></param>
    public GenericAPI
    (
        ILogger<object> logger,
        IOptions<AppSettings> appSettings
    )
    {
        _dbSettings = appSettings.Value.ServiceDB;//By default it uses DB Service DB as connection string.
        _logger = logger;
    }
    /// <summary>
    /// Generic API Constructor with Custom DB settings
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dBSettings">to send the custom db settings directly from controller</param>
    public GenericAPI
    (
        ILogger<object> logger,
        DBSettings dBSettings
    )
    {
        _dbSettings = dBSettings; // We can use either 'appSettings.Value.AuthDB' or custom setting, considering future change we have set to use 'dbSettings';
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
            _dbSettings.ClientURL, 
            _dbSettings.DataBaseName, 
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
            _dbSettings.ClientURL, 
            _dbSettings.DataBaseName, 
            _logger);
        var returnModel = await _actionCommand.CommandHandlerAsync(filter, model);
        return ResultHandler(_actionCommand, returnModel);        
    }
    
    public async Task<T> UpdatePartial(T model,  
        UpdateDefinition<T> updateFilter, 
        FilterDefinition<T> filter)
    {
        _actionCommand = new UpdatePartialHandler<T>(
            _dbSettings.ClientURL, 
            _dbSettings.DataBaseName, 
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
            (_dbSettings.ClientURL, _dbSettings.DataBaseName, _logger, soft);
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
            connString: _dbSettings.ClientURL,
            _dbSettings.DataBaseName,
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
            connString: _dbSettings.ClientURL,
            _dbSettings.DataBaseName,
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
            connString: _dbSettings.ClientURL,
            _dbSettings.DataBaseName,
            _logger
        );
        var count = await _getCommand.GetCountAsync(filter);
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
}