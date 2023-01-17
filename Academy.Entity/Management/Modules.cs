using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Academy.Entity.Management;

public class Module
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }  
    public string Name { get; set; }
    public string Description { get; set; }
    
    #region Meta Fields
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    #endregion
}