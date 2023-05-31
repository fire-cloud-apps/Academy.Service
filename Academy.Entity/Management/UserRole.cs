using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections;

namespace Academy.Entity.Management;

/// <summary>
/// User Role
/// </summary>
public class UserRole
{
    /// <summary>
    /// Role Id
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    #region Permission Configuration

    public IList<ModulePermission> AllowedModules { get; set; }
    public IList<ModulePermission> AllowedAddOns { get; set; }
    public IList<ReportPermission> AllowedReports { get; set; }
    public IList<DashboardPermission> AllowedDashboards { get; set; }

    #endregion
    
    #region Meta Fields
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    #endregion
}

public class ModulePermission
{
    public string UniqueId { get; set; } = string.Empty;
    public string Name { get; set; }
    /// <summary>
    /// Used as a tool tip message
    /// </summary>
    public string Description { get; set; }    
    
    public string LinkGroup { get; set; } = "View";
    public string LinkKey { get; set; } = string.Empty;
    public string IconGroup { get; set; } = "Iconify";
    public string IconKey { get; set; }
    public bool IsSubscribed { get; set; } = false;

    /// <summary>
    /// Will have permission to list view & detailed view
    /// </summary>
    public bool Read { get; set; } = true;
    public bool Create { get; set; } = false;
    public bool Modify { get; set; } = false;
    public bool Delete { get; set; } = false;
    public IList<ModulePermission> SubModules { get; set; } = new List<ModulePermission>();
}

public class ReportPermission
{
    public string UniqueId { get; set; } = string.Empty;
    public string Name { get; set; }
    public bool IsAllowed { get; set; }
    public string Description { get; set; }

    public string IconKey { get; set; } = "HR";
    public string LinkGroup { get; set; } = "View";
    public string LinkKey { get; set; } = string.Empty;
    public string IconGroup { get; set; } = "Iconify";
    public IList<ReportPermission> ChildItems { get; set; } = new List<ReportPermission>();

}

public class DashboardPermission
{
    public string UniqueId { get; set; } = string.Empty;
    public string Name { get; set; }
    public bool IsAllowed { get; set; }
    public string Description { get; set; }

    public string LinkGroup { get; set; } = "View";
    public string LinkKey { get; set; } = string.Empty;
    public string IconGroup { get; set; } = "Iconify";
    public string IconKey { get; set; } = "HR";
    public IList<DashboardPermission> ChildItems { get; set; } = new List<DashboardPermission>();
}

