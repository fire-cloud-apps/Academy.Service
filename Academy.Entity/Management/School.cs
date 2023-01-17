using Academy.Entity.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Academy.Entity.Management;

public class School
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    /// <summary>
    /// This describes the School/College/Institute Name
    /// </summary>
    public string? Name { get; set; }
    public string? RegistrationId { get; set; }
    /// <summary>
    /// When was the institute or school has started functioning.
    /// </summary>
    public DateTime FoundedOn { get; set; }
    public Contact Address { get; set; }
    
    public FileMetaData Logo { get; set; }
    public FileMetaData FavIcon { get; set; }
    
    public IList<FileMetaData> Attachments { get; set; }
    
    public Principal HeadOfSchool { get; set; }
    public  Settings.Academy Academy { get; set; }

    #region Some Background Jobs to be calculated and that has to be updated.
    public int TotalStudents { get; set; }
    public int TotalStaffs { get; set; }
    
    public double TotalFees { get; set; }
    public double TotalDues { get; set; }
    #endregion
    
    
    #region Meta Fields
    public string AccountId { get; set; }
    public string EnteredBy { get; set; }
    public bool IsActive { get; set; }        
    public bool IsDeleted { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
    #endregion
    
}

