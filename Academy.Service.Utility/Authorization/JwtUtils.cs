using Microsoft.Extensions.Logging;
using Academy.Entity.Management;

namespace Academy.Service.Utility.Authorization;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

public interface IJwtUtils
{
    public User? ValidateToken(string token);
   
    public User GetCurrentUser(string autheorizationHeader);


}

public class JwtUtils : IJwtUtils
{
    private readonly AppSettings _appSettings;
    private readonly string _secret;
    private readonly ILogger<object> _logger;

    public JwtUtils(ILogger<object> logger, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _secret = _appSettings.JWTSettings.Secret;
        _logger = logger;
    }

    /// <summary>
    /// Generates JWT Token
    /// </summary>
    /// <param name="user">User Details</param>
    /// <param name="account">Account Connection String</param>
    /// <returns></returns>
    public string? GenerateJwtToken(User user, Account account)
    {
        // generate token that is valid for 15 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);
        string tokenJwt = null;
        // string connectionCypherText =
        //     AESString.Encrypt
        //     (account.ClientConnectionString, _appSettings.JWTSettings.EncryptionKey);

        if (user?.MappedAccount.Id != null)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("Id", user.Id.ToString()),
                        new Claim("Email", user.Email),
                        new Claim("GivenName", user.GivenName),
                        new Claim("AccountId", user?.MappedAccount.Id),
                        //new Claim("ConnectionKey", connectionCypherText),
                        new Claim("Picture", user.Picture),
                    }),
                Expires = DateTime.UtcNow.AddDays(_appSettings.JWTSettings.TokenExpiry),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            tokenJwt = tokenHandler.WriteToken(token);

        }
        return tokenJwt;
    }

    public User ValidateToken(string token)
    {
        if (token == null) 
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "Id").Value;

            var user = new User()
            {
                Id = jwtToken.Claims.First(x => x.Type == "Id").Value,
                Email = jwtToken.Claims.First(x => x.Type == "Email").Value,
                GivenName = jwtToken.Claims.First(x => x.Type == "GivenName").Value,
                MappedAccount = new QuickView()
                {
                    Id = jwtToken.Claims.First(x => x.Type == "AccountId").Value,
                },
                Picture = jwtToken.Claims.First(x => x.Type == "Picture").Value,
                
            };
            // return user id from JWT token if validation successful
            return user;
        }
        catch(Exception ex)
        {
            _logger.LogError("Error:", ex);
            // return null if validation fails
            return null;
        }
    }

    public User GetCurrentUser(string autheorizationHeader)
    {
        var tokenData = autheorizationHeader.Split(' ');
        var token = tokenData[1];
        return ValidateToken(token);
    }
}