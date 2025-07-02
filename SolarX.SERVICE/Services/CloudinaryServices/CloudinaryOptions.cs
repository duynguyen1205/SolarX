using System.ComponentModel.DataAnnotations;

namespace Startup_Project.SERVICE.Services.CloudinaryServices;

public class CloudinaryOptions
{
    [Required] public string CloudName { get; set; }
    [Required] public string ApiKey { get; set; }
    [Required] public string ApiSecret { get; set; }
}