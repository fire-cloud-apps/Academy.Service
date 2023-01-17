using System.ComponentModel.DataAnnotations;

namespace Academy.Entity.Management;

public class AuthenticateRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}