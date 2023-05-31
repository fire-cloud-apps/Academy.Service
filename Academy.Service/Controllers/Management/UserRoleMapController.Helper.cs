using Academy.Entity.Management;
using MongoDB.Driver;

namespace Academy.Service.Controllers.Management;

public partial class UserRoleMapController
{
    private async Task<UserRoleMapping> IsUserExists(string userId)
    {
        #region Filter by ID
        var builder = Builders<UserRoleMapping>.Filter;
        // Filter by field
        var idFilter = builder.Eq(u => u.UserId.Id, userId);
        #endregion
        var model = await _genericApi.GetFilter(idFilter);
        return model.FirstOrDefault();
    }
    
    private async Task<UserRole?> GetUserRoleByUserId(string userId)
    {
        UserRole? userRole = null;
        #region Filter by User Id
        var builder = Builders<UserRoleMapping>.Filter;
        // Filter by field
        var idFilter = builder.Eq(u => u.UserId.Id, userId);
        #endregion

        var roleMap = await _genericApi.GetFilter(idFilter);
        var roleMapModel = roleMap.FirstOrDefault();
        if(roleMapModel is not null)
        {
            //Get the User Role assocaited.
            var roleBuilder = Builders<UserRole>.Filter;
            var roldIdFilter = roleBuilder.Eq(u => u.Id, roleMapModel.RoleId.Id);
            var userRoles = await _genericUserRoleApi.GetFilter(roldIdFilter);
            userRole = userRoles?.FirstOrDefault();
        }
        return userRole;
    }

    private static ModulePermission FindModule(
        string moduleId, 
        IList<ModulePermission> allowedModules)
    {
        ModulePermission result = null;
        foreach (var module in allowedModules)
        {
            if (module.UniqueId == moduleId)
            {
                result = module;
                break;
            }
        }

        return result;
    }

}

