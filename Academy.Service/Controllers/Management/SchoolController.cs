using Academy.DataAccess.GenericHandler;
using Academy.DataAccess.Interface;
using Academy.DataAccess;
using Academy.Entity.DataBogus.Management;
using Academy.Entity.Management;
using Academy.Service.Utility;
using Academy.Service.Utility.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Academy.Service.Controllers.Management;

/// <summary>
/// School Controller API
/// </summary>
[Authorize]
[ApiController]
[Route("Management/[controller]")]
public class SchoolController : ControllerBase
{
    #region Global Variables    
    private readonly ILogger<GenericAPI<School>> _logger;
    private readonly AppSettings _appSettings;
    private IActionCommand<School> _actionCommand;
    private readonly GenericAPI<School> _genericApi;
    #endregion

    #region Constructor

    /// <summary>
    /// UserRole Constructor
    /// </summary>
    /// <param name="logger">Generic Logger</param>
    /// <param name="appSettings">Application Settings</param>
    public SchoolController(
        ILogger<GenericAPI<School>> logger,
        IOptions<AppSettings> appSettings
    )
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _genericApi = new GenericAPI<School>(_logger, appSettings);
    }

    #endregion

    #region DML Actions

    /// <summary>
    /// Registers the Schools to the institute, with the School details as such
    /// </summary>
    /// <param name="model">School model data to register</param>
    /// <returns>returns the success or failed message</returns>
    ///[AllowAnonymous]
    [HttpPost("Create")]
    public async Task<IActionResult> Create(School model)
    {
        _actionCommand = new CreateHandler<School>(_appSettings.ServiceDB.ClientURL, _appSettings.ServiceDB.DataBaseName, _logger);
        await _actionCommand.CommandHandlerAsync(model);

        return Ok(new { message = "School Created successfully" });
    }

    /// <summary>
    /// Update the "School", with any of the model date, this is just replaces all data inside the model
    /// </summary>    
    /// <param name="updateSchool">the new data to be updated</param>
    /// <returns>returns updated "School". </returns>
    /// <exception cref="Exception">Possible exceptions to be thrown</exception>
    //[AllowAnonymous]
    [HttpPut]
    public async Task<IActionResult> Update(School updateSchool)
    {
        #region Update Filter
        //1. Filter to update the field
        var builder = Builders<School>.Filter;
        var filter = builder.Eq(u => u.Id, updateSchool.Id);
        #endregion

        updateSchool = await _genericApi.Update(updateSchool, filter);

        return Ok(updateSchool);
    }

    /// <summary>
    /// Delete the "School", by the given Id
    /// </summary>
    /// <param name="id">School Id</param>
    /// <param name="soft">true-soft or false-hard delete </param>
    /// <returns>returns deleted Id</returns>
    //[AllowAnonymous]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, bool soft = true)
    {
        #region Delete Filter
        //1. Fields to be Updated.
        School deleteSchool = new School();
        var builderUpdate = Builders<School>.Update;
        var updateFields = builderUpdate
                .Set(u => u.IsDeleted, soft);
        //.Set(u => u.IsActive, false);

        //2. Apply the Filter by Id
        var builder = Builders<School>.Filter;
        var filter = builder.Eq(u => u.Id, id);
        #endregion

        await _genericApi.Delete(deleteSchool, updateFields, filter, soft);

        return Ok(id);
    }


    /// <summary>
    /// Activates or De-Activates the "School"
    /// </summary>
    /// <param name="id">Activate or DeActive by School Id</param>
    /// <param name="activate">bool value 'true'-Activate, 'false'-DeActivate</param>
    /// <returns>returns the School Id with status message</returns>
    [HttpPut("Activation/{id}")]
    //[AllowAnonymous]
    public async Task<IActionResult> Activation(string id, bool activate = true)
    {
        #region Set Update Filter
        //1. Fields to be Updated.
        School acivationSchool = new School();
        var builderUpdate = Builders<School>.Update;
        var updateFields = builderUpdate
                .Set(u => u.IsActive, activate);
        //2. Apply the Update Filter by ID.
        var builder = Builders<School>.Filter;
        var filter = builder.Eq(u => u.Id, id);
        #endregion

        await _genericApi.UpdatePartial(acivationSchool, updateFields, filter);

        ResultData<string> result = new ResultData<string>()
        {
            Information = activate ? "School Activated" : "School De-Activated",
            Response = id
        };

        return Ok(result);
    }

    /// <summary>
    /// Revoke an School, which is deleted
    /// </summary>
    /// <param name="id">School Id</param>
    /// <param name="delete">by default false. false-revoke. true- will delete</param>
    /// <returns>success message or failure message</returns>
    [HttpPut("Revoke/{id}")]
    public async Task<IActionResult> Revoke(string id, bool delete = false)
    {
        #region Set Update Filter
        //1. Fields to be Updated.
        School deleteSchool = new School();
        var builderUpdate = Builders<School>.Update;
        var updateFields = builderUpdate
            .Set(u => u.IsDeleted, delete);
        //2. Apply the Update Filter by ID.
        var builder = Builders<School>.Filter;
        var filter = builder.Eq(u => u.Id, id);
        #endregion

        await _genericApi.UpdatePartial(deleteSchool, updateFields, filter);

        ResultData<string> result = new ResultData<string>()
        {
            Information = delete ? "School Deleted" : "School Revoked",
            Response = id
        };

        return Ok(result);
    }


    #endregion

    #region Read Data


    /// <summary>
    /// Get School by unique id
    /// </summary>
    /// <param name="id">Document Id</param>
    /// <returns>returns document with full details</returns>
    //[AllowAnonymous]
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        #region Filter by ID
        var builder = Builders<School>.Filter;
        // Filter by field
        var idFilter = builder.Eq(u => u.Id, id);
        #endregion
        var model = await _genericApi.GetFilter(idFilter);
        return Ok(model.FirstOrDefault());
    }

    /// <summary>
    /// Get School Data without any filter
    /// </summary>
    /// <returns>Get all the document without any filter</returns>
    //[AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        #region No Filter
        var builder = Builders<School>.Filter;
        // Filter by field
        var noFilter = builder.Empty;

        #endregion

        var result = await _genericApi.GetFilter(noFilter);
        return Ok(result.ToList());
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
        var builder = Builders<School>.Filter;
        // Filter by field
        var filters = BuildFilter(builder, metaData);
        //Sort
        var sort = Builders<School>.Sort.Ascending(u => u.Name);
        sort = BuildSort(sort, metaData);
        //Pagination
        var pagination = new Pagination()
        {
            Page = metaData.Page,
            PageSize = metaData.PageSize
        };
        #endregion

        var result = await _genericApi.GetFilter(filters, sort: sort, pagination: pagination);

        BatchResult<School> batchResult = new BatchResult<School>()
        {
            Items = result,
            TotalItems = await _genericApi.GetRecordCount(filters)
        };
        return Ok(batchResult);
    }

    /// <summary>
    /// Gets the total document count
    /// </summary>
    /// <returns>returns total number of documents</returns>
    [Route("Count")]
    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        var builder = Builders<School>.Filter;
        var isDeletedFilters = builder.Eq(u => u.IsDeleted, false);
        var result = await _genericApi.GetRecordCount(isDeletedFilters);
        return Ok(result);
    }

    #region Helper Methods

    /// <summary>
    /// Build Filter Definition for the given model
    /// </summary>
    /// <param name="builder">filter builder</param>
    /// <param name="metaData">meta data to perform filter</param>
    /// <returns>returns filter query</returns>
    private static FilterDefinition<School> BuildFilter(FilterDefinitionBuilder<School> builder, PageMetaData metaData)
    {
        var field = metaData.SearchField;
        var filterBuilder = builder.Empty;
        var ignoreIsDeleted = false;
        IList<FilterDefinition<School>> filterLists = new List<FilterDefinition<School>>();

        #region Primary Condition Filter

        switch (field)
        {
            case "Name":
                var pattern = new BsonRegularExpression(metaData.SearchText, "i"); //"i" Indicates case insensitive.
                filterBuilder = builder.Regex(u => u.Name, pattern);
                break;
            case "IsActive":
                filterBuilder = builder.Eq(u => u.IsActive, metaData.SearchText == "true");
                break;
            case "IsDeleted":
                filterBuilder = builder.Eq(u => u.IsDeleted, metaData.SearchText == "true");
                ignoreIsDeleted = true;
                break;
            default:
                filterBuilder = builder.Empty;
                break;
        }

        #endregion

        filterLists.Add(filterBuilder);

        #region Additional Field value filters
        IList<FieldValue> additionalParams = metaData.FilterParams;
        if (additionalParams.Count > 0)
        {
            foreach (FieldValue fieldValue in additionalParams)
            {
                switch (fieldValue.Field)
                {
                    case "Name":
                        var SchoolFilter = builder.Eq(u => u.Name, fieldValue.Value);
                        filterLists.Add(SchoolFilter);
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

        if (ignoreIsDeleted is not true)
        {
            //Default filter to avoid returning 'Deleted Items'.
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
    private static SortDefinition<School> BuildSort(SortDefinition<School> sorting, PageMetaData metaData)
    {
        SortDefinition<School> sort;
        var field = metaData.SortLabel;
        sort = field switch
        {
            "Name" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.Name)
                : sorting.Descending(u => u.Name),
            "IsActive" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.IsActive)
                : sorting.Descending(u => u.IsActive),
            "IsDeleted" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.IsDeleted)
                : sorting.Descending(u => u.IsDeleted),
            _ => sorting.Ascending(u => u.Name)
        };
        return sort;
    }

    #endregion

    #endregion

    #region Fake Data
    [AllowAnonymous]
    [HttpGet]
    [Route("FakeData")]
    public async Task<ActionResult<School>> FakeData()
    {
        SchoolFaker faker = new SchoolFaker();
        var fake = faker.GenerateData();
        return Ok(fake.Generate(1));
    }
    #endregion
}

