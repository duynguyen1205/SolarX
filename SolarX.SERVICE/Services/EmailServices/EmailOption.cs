using System.ComponentModel.DataAnnotations;

namespace SolarX.SERVICE.Services.EmailServices;

public class EmailOption
{
    [Required]public string Mail { get; set; }
    [Required]public string Password { get; set; }
    [Required]public string Host { get; set; }
    [Required]public int Port { get; set; }
}