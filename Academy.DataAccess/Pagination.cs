namespace Academy.DataAccess;

/// <summary>
/// Basic pagination
/// </summary>
public class Pagination
{
    /// <summary>
    /// What is the page 'index'/'Skip' how many records.
    /// Usually should start from 0, 10, 20, 30 etc.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Total records to be displayed per page or Page 'Limit'
    /// </summary>
    public int PageSize { get; set; }
}