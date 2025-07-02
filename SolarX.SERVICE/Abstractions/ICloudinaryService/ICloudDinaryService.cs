using Microsoft.AspNetCore.Http;

namespace SolarX.SERVICE.Abstractions.ICloudinaryService;

public interface ICloudinaryService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    Task<bool> DeleteFile(string imgUrl);
}