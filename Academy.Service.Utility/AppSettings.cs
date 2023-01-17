using System.Runtime;

namespace Academy.Service.Utility;

public class AppSettings
{
    public string Secret { get; set; }    
    public DBSettings DBSettings { get; set; }
    public JWTSettings JWTSettings { get; set; }
}

public class API
{
    public string Version { get; set; }  
    public string Name { get; set; }  
    public string Description { get; set; }
    public string Company { get; set; }
    public string Email { get; set; }
}
public class JWTSettings
{
    public string Secret { get; set; }

    public int RefreshTokenTTL { get; set; }
    /// <summary>
    /// JWT Token Expiry in Days
    /// </summary>
    public int TokenExpiry { get; set; }

    public string LogMongo { get; set; }
    /// <summary>
    /// Used to encrypt the connection string.
    /// Key can be generated from
    /// Bit 256
    /// https://www.allkeysgenerator.com/Random/Security-Encryption-Key-Generator.aspx
    /// </summary>
    public string EncryptionKey { get; set; }
}
public class DBSettings
{
    public string ClientDB { get; set; }
    public string DataBaseName { get; set; }
    public string CollectionName { get; set; }
}