using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Academy.Entity.Management;

//TODO: Currently not in use this can be removed.
public class MenuModule
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string IconGroup { get; set; }
    public string ToolTip { get; set; }
    public IList<ChildItem> ChildItem { get; set; }
    
    public string Description { get; set; }

    #region Meta Fields
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    #endregion

    #region Permission Field
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    #endregion

}

public class ChildItem
{
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]

    //public string Id { get; set; }
    //public string ModuleId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string Href { get; set; }
    public string IconGroup { get; set; }
    public string Icon { get; set; }    
    public string ToolTip { get; set; }

    #region Permission Field
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    #endregion
}