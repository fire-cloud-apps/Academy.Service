using Academy.DataAccess;
using Academy.DataAccess.GenericHandler;
using Academy.DataAccess.Interface;
using Academy.Entity.Management;
using Academy.Service.Utility;
using Academy.Service.Utility.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using BCrypt.Net;

namespace Academy.Service.Controllers.Management;

/// <summary>
/// User Controller API to perform API Action
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController: ControllerBase
{
    #region Global Variables
    
    private readonly IJwtUtils _jwtUtils;
    private readonly ILogger<GenericAPI<User>> _logger;    
    private readonly AppSettings _appSettings;
    private IActionCommand<User> _actionCommand;
    private readonly IOptions<AppSettings> _optionAppSettings;
    private readonly GenericAPI<User> _genericApi;
    
    #endregion
    
    #region Constructor
    
    /// <summary>
    /// User Constructor
    /// </summary>
    /// <param name="logger">Generic Logger</param>
    /// <param name="jwtUtils">Token helper</param>
    /// <param name="appSettings">Application Settings</param>
    public UsersController(
        ILogger<GenericAPI<User>> logger,        
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings
    )
    {
        _jwtUtils = jwtUtils;
        _optionAppSettings = appSettings;
        _appSettings = appSettings.Value;
        _logger = logger;
        _genericApi = new GenericAPI<User>(_logger, _appSettings.AuthDB);
    }

    #endregion

    #region DML Actions

    //All DLM Actions performed in Fire cloud.Authentication.Service API
   
    #endregion

    #region Read Data

    /// <summary>
    /// Get User by unique id
    /// </summary>
    /// <param name="id">Document Id</param>
    /// <returns>returns document with full details</returns>
    //[AllowAnonymous]
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        #region Filter by ID
        var builder = Builders<User>.Filter;
        // Filter by field
        var idFilter = builder.Eq(u => u.Id, id);
        #endregion
        var model = await _genericApi.GetFilter(idFilter);
        return Ok(model.FirstOrDefault());
    }

    /// <summary>
    /// Get the data by batch, with the given size and page
    /// </summary>
    /// <param name="metaData">Pagination meta data to return data</param>
    /// <returns>returns the batch of documents</returns>
    //[AllowAnonymous]
    [Route("ByBatch")]
    [HttpPost]
    public async Task<IActionResult> GetBatch(PageMetaData metaData)
    {
        #region Filter & Sort
        var builder = Builders<User>.Filter;
        // Filter by field
        var filters = BuildFilter(builder, metaData);
        //Sort
        var sort = Builders<User>.Sort.Ascending(u => u.FirstName);
        sort = BuildSort(sort, metaData);
        //Pagination
        var pagination = new Pagination()
        {
            Page = metaData.Page,
            PageSize = metaData.PageSize
        };
        #endregion

        var result = await _genericApi.GetFilter(filters, sort, pagination);        
        BatchResult<User> batchResult = new BatchResult<User>()
        {
            Items = result,
            TotalItems = await _genericApi.GetRecordCount(filters)
        };
        return Ok(batchResult);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Build Filter Definition for the given model
    /// </summary>
    /// <param name="builder">filter builder</param>
    /// <param name="metaData">meta data to perform filter</param>
    /// <returns>returns filter query</returns>
    private static FilterDefinition<User> BuildFilter(FilterDefinitionBuilder<User> builder, PageMetaData metaData)
    {
        var field = metaData.SearchField;
        var filterBuilder = builder.Empty;
        var ignoreIsDeleted = false;
        var pattern = new BsonRegularExpression(metaData.SearchText, "i"); //"i" Indicates case insensitive.
        IList<FilterDefinition<User>> filterLists = new List<FilterDefinition<User>>();

        #region Default Search & Filter
        switch (field)
        {
            case "GivenName":
                filterBuilder = builder.Regex(u => u.GivenName, pattern);
                break;
            case "Mobile":
                filterBuilder = builder.Regex(u => u.Mobile, pattern);
                break;
            case "Email":
                filterBuilder = builder.Regex(u => u.Email, pattern);
                break;
            case "IsActive":
                filterBuilder = builder.Eq(u => u.IsActive, metaData.SearchText == "true");
                break;
            default:
                filterBuilder = builder.Empty;
                break;
        }

        filterLists.Add(filterBuilder);
        #endregion

        #region Additional Field value filters
        IList<FieldValue> additionalParams = metaData.FilterParams;
        if (additionalParams.Count > 0)
        {            
            foreach(FieldValue fieldValue in additionalParams)
            {
                switch (fieldValue.Field)
                {
                    case "AccountId":
                      var accountFilter = builder.Eq(u => u.MappedAccount.Id, fieldValue.Value);
                        filterLists.Add(accountFilter);
                      Console.WriteLine($"Account ID {fieldValue.Value}");
                        break;
                    case "IsDeleted":
                        var deleteFilter = builder.Eq(u => u.IsDeleted, fieldValue.Value == "true");
                        filterLists.Add(deleteFilter);
                        ignoreIsDeleted = true;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        
        //Default filter to avoid returning 'Deleted Items'.
        if (ignoreIsDeleted is not true)
        {
            var isDeletedFilters = builder.Eq(u => u.IsDeleted, false);
            filterLists.Add(isDeletedFilters);
        }

        //Append all the filters and return back.
        var filters = builder.And(filterLists);

        return filters;
    }

    /// <summary>
    /// Builds Sorting Definition for the given model
    /// </summary>
    /// <param name="sorting">performs sort definition</param>
    /// <param name="metaData">meta data to perform sorting</param>
    /// <returns>returns sort query</returns>
    private static SortDefinition<User> BuildSort(SortDefinition<User> sorting, PageMetaData metaData)
    {
        SortDefinition<User> sort;
        var field = metaData.SortLabel;
        sort = field switch
        {
            "FirstName" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.FirstName)
                : sorting.Descending(u => u.FirstName),
            "IsActive" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.IsActive)
                : sorting.Descending(u => u.IsActive),
            "IsDeleted" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.IsDeleted)
                : sorting.Descending(u => u.IsDeleted),
            _ => sorting.Ascending(u => u.FirstName)
        };
        return sort;
    }

    #endregion
}

/// <summary>
/// Password Rest Property
/// </summary>
public class ResetUserPassword
{
    /// <summary>
    /// User Id to Reset the Password
    /// </summary>
    public string? Id { get; set; }
    /// <summary>
    /// Password to Reset/New Password
    /// </summary>
    public string? Password { get; set; }
        
}