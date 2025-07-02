using System.ComponentModel.DataAnnotations;

namespace SolarX.SERVICE.Services.JwtServices;

public class JwtOptions
{
    [Required]public string Issuer { get; set; }
    [Required]public string Audience { get; set; }
    [Required]public string SecretKey { get; set; }
    [Required]public int ExpireMinutes { get; set; }
}