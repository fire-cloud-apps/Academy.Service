using MongoDB.Bson;

namespace Academy.Entity.Management;

/// <summary>
/// This is embedded document hence it does not have unique Object Id
/// </summary>
public class Principal
{
    public string FullName { get; set; }
    public string StaffId { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string Phone { get; set; }
}