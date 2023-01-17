namespace Academy.Service.Utility;

/// <summary>
/// Used as a response data result to Client side.
/// </summary>
/// <typeparam name="TModel">Model Type</typeparam>
public class BatchResult<TModel>
{
    public IList<TModel> Items { get; set; }
    public long TotalItems { get; set; }
}