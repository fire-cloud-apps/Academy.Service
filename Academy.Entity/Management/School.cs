using Academy.Entity.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Net;

namespace Academy.Entity.Management;

public class School
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    #region General
    public string Name { get; set; }
    public string About { get; set; }
    public string RegistrationNo { get; set; }
    public DateTime? FoundedOn { get; set; }
    public FileMetaData SchoolLogo { get; set; }
    public QuickView Principal { get; set; } // Autocomplete search box selection.
    #endregion

    #region Contacts
    public Address AddressDetails { get; set; }
    public string PrimaryMobile { get; set; }
    public string SecondaryMobile { get; set; }

    public string Email { get; set; }
    public string WebURL { get; set; }
    #endregion

    #region School Current Year Settings
    public QuickView AcademyYear { get; set; }
    public bool AdminissionStatus { get; set; }
    #endregion


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

