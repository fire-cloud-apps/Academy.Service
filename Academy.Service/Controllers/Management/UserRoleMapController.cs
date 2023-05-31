using Academy.DataAccess;
using Academy.DataAccess.GenericHandler;
using Academy.DataAccess.Interface;
using Academy.Entity.DataBogus.Management;
using Academy.Entity.Management;
using Academy.Service.Utility;
using Academy.Service.Utility.Authorization;
using Bogus.DataSets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Academy.Service.Controllers.Management;

    

/// <summary>
/// User Role Mapper Controller
/// </summary>
[Authorize]
[ApiController]
[Route("Management/[controller]")]
public partial class UserRoleMapController : ControllerBase
{
    #region Global Variables

    private readonly ILogger<GenericAPI<UserRoleMapping>> _logger;
    private readonly AppSettings _appSettings;
    private IActionCommand<UserRoleMapping> _actionCommand;
    private readonly GenericAPI<UserRoleMapping> _genericApi;
    private readonly GenericAPI<UserRole> _genericUserRoleApi;
    #endregion

    #region Constructor

    /// <summary>
    /// UserRoleMapping Constructor
    /// </summary>
    /// <param name="logger">Generic Logger</param>
    /// <param name="appSettings">Application Settings</param>
    public UserRoleMapController(
        ILogger<GenericAPI<UserRoleMapping>> logger,
        IOptions<AppSettings> appSettings
    )
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _genericApi = new GenericAPI<UserRoleMapping>(_logger, appSettings);
        _genericUserRoleApi = new GenericAPI<UserRole>(_logger, appSettings);
    }

    #endregion

    #region DML Actions

    /// <summary>
    /// Registers the UserRoleMappings to the institute, with the UserRoleMapping details as such
    /// </summary>
    /// <param name="model">UserRoleMapping model data to register</param>
    /// <returns>returns the success or failed message</returns>
    ///[AllowAnonymous]
    [HttpPost("Create")]
    public async Task<IActionResult> Create(UserRoleMapping model)
    {
        IActionResult actionResult = null;
        //1. Find if the user is already configured with any 'Role'.
        var userExists =  await IsUserExists(model.UserId.Id);
        //2. If user already exists, else throw some error code as 'ValidationProblem/Conflict'
        if(userExists is null)
        {
            //3. User does not exists, 'Excute insertion'.
            _actionCommand = new CreateHandler<UserRoleMapping>(_appSettings.ServiceDB.ClientURL, _appSettings.ServiceDB.DataBaseName, _logger);
            await _actionCommand.CommandHandlerAsync(model);
            actionResult = Ok(new { message = "UserRoleMapping Created successfully" });
        }
        else
        {
            actionResult = ValidationProblem($"User '{userExists.UserId.Name}', is already mapped with the Role '{userExists.RoleId.Name}'.");
        }
        
        return actionResult;

        
    }

   

    /// <summary>
    /// Update the "UserRoleMapping", with any of the model date, this is just replaces all data inside the model
    /// </summary>    
    /// <param name="updateUserRoleMapping">the new data to be updated</param>
    /// <returns>returns updated "UserRoleMapping". </returns>
    /// <exception cref="Exception">Possible exceptions to be thrown</exception>
    //[AllowAnonymous]
    [HttpPut]
    public async Task<IActionResult> Update(UserRoleMapping updateUserRoleMapping)
    {
        #region Update Filter
        //1. Filter to update the field
        var builder = Builders<UserRoleMapping>.Filter;
        var filter = builder.Eq(u => u.Id, updateUserRoleMapping.Id);
        #endregion

        updateUserRoleMapping = await _genericApi.Update(updateUserRoleMapping, filter);

        return Ok(updateUserRoleMapping);
    }

    /// <summary>
    /// Delete the "UserRoleMapping", by the given Id
    /// </summary>
    /// <param name="id">UserRoleMapping Id</param>
    /// <param name="soft">true-soft or false-hard delete </param>
    /// <returns>returns deleted Id</returns>
    //[AllowAnonymous]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, bool soft = true)
    {
        #region Delete Filter
        //1. Fields to be Updated.
        UserRoleMapping deleteUserRoleMapping = new UserRoleMapping();
        var builderUpdate = Builders<UserRoleMapping>.Update;
        var updateFields = builderUpdate
                .Set(u => u.IsDeleted, soft);
        //.Set(u => u.IsActive, false);

        //2. Apply the Filter by Id
        var builder = Builders<UserRoleMapping>.Filter;
        var filter = builder.Eq(u => u.Id, id);
        #endregion

        await _genericApi.Delete(deleteUserRoleMapping, updateFields, filter, soft);

        return Ok(id);
    }


    /// <summary>
    /// Activates or De-Activates the "UserRoleMapping"
    /// </summary>
    /// <param name="id">Activate or DeActive by UserRoleMapping Id</param>
    /// <param name="activate">bool value 'true'-Activate, 'false'-DeActivate</param>
    /// <returns>returns the UserRoleMapping Id with status message</returns>
    [HttpPut("Activation/{id}")]
    //[AllowAnonymous]
    public async Task<IActionResult> Activation(string id, bool activate = true)
    {
        #region Set Update Filter
        //1. Fields to be Updated.
        UserRoleMapping acivationUserRoleMapping = new UserRoleMapping();
        var builderUpdate = Builders<UserRoleMapping>.Update;
        var updateFields = builderUpdate
                .Set(u => u.IsActive, activate);
        //2. Apply the Update Filter by ID.
        var builder = Builders<UserRoleMapping>.Filter;
        var filter = builder.Eq(u => u.Id, id);
        #endregion

        await _genericApi.UpdatePartial(acivationUserRoleMapping, updateFields, filter);

        ResultData<string> result = new ResultData<string>()
        {
            Information = activate ? "UserRoleMapping Activated" : "UserRoleMapping De-Activated",
            Response = id
        };

        return Ok(result);
    }

    /// <summary>
    /// Revoke an UserRoleMapping, which is deleted
    /// </summary>
    /// <param name="id">UserRoleMapping Id</param>
    /// <param name="delete">by default false. false-revoke. true- will delete</param>
    /// <returns>success message or failure message</returns>
    [HttpPut("Revoke/{id}")]
    public async Task<IActionResult> Revoke(string id, bool delete = false)
    {
        #region Set Update Filter
        //1. Fields to be Updated.
        UserRoleMapping deleteUserRoleMapping = new UserRoleMapping();
        var builderUpdate = Builders<UserRoleMapping>.Update;
        var updateFields = builderUpdate
            .Set(u => u.IsDeleted, delete);
        //2. Apply the Update Filter by ID.
        var builder = Builders<UserRoleMapping>.Filter;
        var filter = builder.Eq(u => u.Id, id);
        #endregion

        await _genericApi.UpdatePartial(deleteUserRoleMapping, updateFields, filter);

        ResultData<string> result = new ResultData<string>()
        {
            Information = delete ? "UserRoleMapping Deleted" : "UserRoleMapping Revoked",
            Response = id
        };

        return Ok(result);
    }


    #endregion

    #region Read Data


    /// <summary>
    /// Get UserRoleMapping by unique id
    /// </summary>
    /// <param name="id">Document Id</param>
    /// <returns>returns document with full details</returns>
    //[AllowAnonymous]
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        #region Filter by ID
        var builder = Builders<UserRoleMapping>.Filter;
        // Filter by field
        var idFilter = builder.Eq(u => u.Id, id);
        #endregion
        var model = await _genericApi.GetFilter(idFilter);
        return Ok(model.FirstOrDefault());
    }

    /// <summary>
    /// Get UserRoleMapping Data without any filter
    /// </summary>
    /// <returns>Get all the document without any filter</returns>
    //[AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        #region No Filter
        var builder = Builders<UserRoleMapping>.Filter;
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
        var builder = Builders<UserRoleMapping>.Filter;
        // Filter by field
        var filters = BuildFilter(builder, metaData);
        //Sort
        var sort = Builders<UserRoleMapping>.Sort.Ascending(u => u.UserId.Name);
        sort = BuildSort(sort, metaData);
        //Pagination
        var pagination = new Pagination()
        {
            Page = metaData.Page,
            PageSize = metaData.PageSize
        };
        #endregion

        var result = await _genericApi.GetFilter(filters, sort: sort, pagination: pagination);

        BatchResult<UserRoleMapping> batchResult = new BatchResult<UserRoleMapping>()
        {
            Items = result,
            TotalItems = await _genericApi.GetRecordCount(filters)
        };
        return Ok(batchResult);
    }

    /// <summary>
    /// Get UserRoleMapping by unique UserId
    /// </summary>
    /// <param name="userId">Document userId</param>
    /// <returns>returns document with full details</returns>
    //[AllowAnonymous]
    [HttpGet("UserRole/{userId}")]
    public async Task<IActionResult> GetRoleByUserId(string userId)
    {
        IActionResult actionResult = null;

        var userRole = await GetUserRoleByUserId(userId);
        if (userRole is not null)
        {
            actionResult = Ok(userRole);
        }
        else
        {
            actionResult = ValidationProblem($"User is not assigned with any Role.");
        }
        return actionResult;
    }

    
    
    /// <summary>
    /// Get UserRoleMapping by unique UserId
    /// </summary>
    /// <param name="userId">Document userId</param>
    /// <param name="moduleId">Document moduleId</param>
    /// <returns>returns document with full details</returns>
    //[AllowAnonymous]
    [HttpGet("UserRole/{userId}/Module/{moduleId}")]
    public async Task<IActionResult> GetModuleByUserId(string userId, string moduleId)
    {
        IActionResult actionResult = null;
        var userRole = await GetUserRoleByUserId(userId);
        if (userRole is not null)
        {
            var module = FindModule(moduleId, userRole.AllowedModules);
            actionResult = Ok(module);
        }
        else
        {
            actionResult = ValidationProblem($"User/Module is not assigned with any Role.");
        }

        return actionResult;
    }
    
    /// <summary>
    /// Get AddOns by unique UserId
    /// </summary>
    /// <param name="userId">Document userId</param>
    /// <param name="addOnId">Document addOnId</param>
    /// <returns>returns document with full details</returns>
    //[AllowAnonymous]
    [HttpGet("UserRole/{userId}/AddOn/{addOnId}")]
    public async Task<IActionResult> GetAddOnsByUserId(string userId, string addOnId)
    {
        IActionResult actionResult = null;
        var userRole = await GetUserRoleByUserId(userId);
        if (userRole is not null)
        {
            var module = FindModule(addOnId, userRole.AllowedAddOns);
            actionResult = Ok(module);
        }
        else
        {
            actionResult = ValidationProblem($"User/AddOns is not assigned with any Role.");
        }

        return actionResult;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Build Filter Definition for the given model
    /// </summary>
    /// <param name="builder">filter builder</param>
    /// <param name="metaData">meta data to perform filter</param>
    /// <returns>returns filter query</returns>
    private static FilterDefinition<UserRoleMapping> BuildFilter(FilterDefinitionBuilder<UserRoleMapping> builder, PageMetaData metaData)
    {
        var field = metaData.SearchField;
        var filterBuilder = builder.Empty;
        var ignoreIsDeleted = false;
        IList<FilterDefinition<UserRoleMapping>> filterLists = new List<FilterDefinition<UserRoleMapping>>();

        #region Primary Condition Filter

        switch (field)
        {
            case "Name":
                var pattern = new BsonRegularExpression(metaData.SearchText, "i"); //"i" Indicates case insensitive.
                filterBuilder = builder.Regex(u => u.UserId.Name, pattern);
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
                    case "User":
                        var UserRoleMappingFilter = builder.Eq(u => u.UserId.Id, fieldValue.Value);
                        filterLists.Add(UserRoleMappingFilter);
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
    private static SortDefinition<UserRoleMapping> BuildSort(SortDefinition<UserRoleMapping> sorting, PageMetaData metaData)
    {
        SortDefinition<UserRoleMapping> sort;
        var field = metaData.SortLabel;
        sort = field switch
        {
            "Name" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.UserId.Name)
                : sorting.Descending(u => u.UserId.Name),
            "IsActive" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.IsActive)
                : sorting.Descending(u => u.IsActive),
            "IsDeleted" => metaData.SortDirection == "A"
                ? sorting.Ascending(u => u.IsDeleted)
                : sorting.Descending(u => u.IsDeleted),
            _ => sorting.Ascending(u => u.UserId.Name)
        };
        return sort;
    }

    #endregion

    #region Scaler Methods
    /// <summary>
    /// Gets the total document count (No of AcademyYear (which is not deleted))
    /// </summary>
    /// <returns>returns total number of documents</returns>
    [Route("Count")]
    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        var builder = Builders<UserRoleMapping>.Filter;
        var isDeletedFilters = builder.Eq(u => u.IsDeleted, false);
        var result = await _genericApi.GetRecordCount(isDeletedFilters);
        return Ok(result);
    }
    
    #endregion

    #region Fake Data
    [AllowAnonymous]
    [HttpGet]
    [Route("FakeData")]
    public async Task<ActionResult<UserRoleMapping>> FakeData()
    {
        //UserRoleMappingFaker faker = new UserRoleMappingFaker();
        //var fake = faker.GenerateData();
        //return Ok(fake.Generate(1));
        return Ok();
    }
    #endregion
}

