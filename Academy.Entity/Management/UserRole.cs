

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Academy.Entity.Management;

/// <summary>
/// User Role
/// </summary>
public class UserRole
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IList<Permissions> AllowedModules { get; set; }
    
    #region Meta Fields
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    #endregion
}

public class Permissions
{
    public Module AllowedModule
    {
        get;
        set;
    }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
}