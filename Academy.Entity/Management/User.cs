

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Academy.Entity.Management;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string NameIdentifier { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string Description { get; set; }
    public string Password { get; set; }
    public string EmailVerfied { get; set; }

    public string Picture { get; set; }
    public string GivenName { get; set; }
    public string SurName { get; set; }
    public string Locale { get; set; }

    public string ClientDomain { get; set; }

    /// <summary>
    /// Authentication JWT Token for the Logged in user.
    /// </summary>
    public string? Jwt { get; set; }


    /// <summary>
    /// User Mapped to which account
    /// </summary>
    //public string? AccountId { get; set; }
    //public string? AccountName { get; set; }

    public QuickView MappedAccount { get; set; }

    /// <summary>
    /// This tells us where this user is allowed to login eg. School, Store, Institute etc.
    /// </summary>
    public IList<string> AllowedIn { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public string FirstName { get; set; }
    public string LastName { get; set; }    

    public IdentityProvider IDPType { get; set; } = IdentityProvider.Custom;

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;


}

public enum IdentityProvider
{
    Google,
    Microsoft,
    Twitter,
    Facebook,
    LinkedIn,
    Custom
}