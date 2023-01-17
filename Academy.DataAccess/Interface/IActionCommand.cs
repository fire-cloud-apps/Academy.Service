
using MongoDB.Driver;

namespace Academy.DataAccess.Interface;

/// <summary>
/// Combines both IError handler with ICommand to handle errors easily.
/// </summary>
public interface IActionCommand<TModel> : ICommand<TModel>, IError
{

}

public interface ICommand<TModel>
{
    //TModel CommandHandler(TModel model);
    Task<TModel> CommandHandlerAsync(TModel model)
    {
        //Empty implementation.
        return null;
    }

    public Task<TModel> CommandHandlerAsync(FilterDefinition<TModel> filter, TModel model)
    {
        //Empty implementation.
        //This will be implemented when conditional update/delete or any action.
        return null;
    }
    
    public Task<TModel> CommandHandlerAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, TModel model)
    {
        //Empty implementation.
        //This will be implemented when multiple conditional update/delete or any action.
        return null;
    }
}
