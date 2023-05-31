namespace Academy.DataAccess;

/// <summary>
/// Pagination data
/// </summary>
public class PageMetaData : Pagination
{
    /// <summary>
    /// Server Side Search for the fixed fields.
    /// </summary>
    public string SearchText { get; set; } = string.Empty;
    
    /// <summary>
    /// Field Name to sort
    /// </summary>
    public string SortLabel { get; set; }

    /// <summary>
    /// If not provided it will use the default search field.
    /// </summary>
    public string SearchField { get; set; }

    /// <summary>
    /// A - Ascending, D - Descending
    /// </summary>
    public string SortDirection { get; set; } = "A";
    /// <summary>
    /// Additional Parameters
    /// </summary>
    public IList<FieldValue> FilterParams { get; set; }

    /// <summary>
    /// Get the Record count after filter & Search
    /// </summary>
    public long RecordCount { get; set; } = 0;

}


public class FieldValue
{
    public string Value { get; set; }
    public string Field { get; set; }
}