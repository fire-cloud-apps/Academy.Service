
using MongoDB.Driver;

namespace Academy.DataAccess.Interface;

// <summary>
/// An Query execution with Error handler
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IActionQuery<TModel> : IQuery<TModel>,  IError
{
    
    Task<IList<TModel>> GetHandlerAsync
    (
        FilterDefinition<TModel> filter,
        SortDefinition<TModel>? sort,
        Pagination? pagination,
        ProjectionDefinition<TModel> project
    )
    {
        return null;
    }
}

public interface IQuery<TModel>
{
    Task<IList<TModel>> GetHandlerAsync(TModel model)
    {
        return null;
    }
    Task<IList<TModel>> GetHandlerAsync(FilterDefinition<TModel> filter)
    {
        return null;
    }
    
    Task<IList<TModel>> GetHandlerAsync(FilterDefinition<TModel> filter, SortDefinition<TModel>? sort)
    {
        return null;
    }

    Task<IList<TModel>> GetHandlerAsync(FilterDefinition<TModel> filter, SortDefinition<TModel>? sort, 
        Pagination? pagination = null)
    {
        return null;
    }

    async Task<long> GetCountAsync(FilterDefinition<TModel> filter)
    {
        return 0;
    }
}

public interface IError
{
    bool IsError { get; set; }
    string ErrorMessage { get; set; }
}