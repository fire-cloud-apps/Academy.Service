//using LiteDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Academy.Entity.Settings;

public class Academy
{
    [BsonId]
    public ObjectId Id { get; set; }    
    public string Name { get; set; }
    public DateTime StartsOn { get; set; }
    public DateTime EndsOn { get; set; }
    
    public string Description { get; set; }
    
    #region Meta Fields
    public ObjectId AccountId { get; set; }
    public ObjectId EnteredBy { get; set; }
    public bool IsActive { get; set; }        
    public bool IsDeleted { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
    #endregion
}