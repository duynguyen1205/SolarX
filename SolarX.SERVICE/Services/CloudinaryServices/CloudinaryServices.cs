using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using SolarX.SERVICE.Abstractions.ICloudinaryService;

namespace SolarX.SERVICE.Services.CloudinaryServices;

public class CloudinaryServices : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryServices(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null.", nameof(file));
        }

        if (!IsValidImage(file))
        {
            throw new ArgumentException("File is not a valid image.", nameof(file));
        }

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folderName,
            UseFilename = true,
            UniqueFilename = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl.ToString();
    }

    public async Task<bool> DeleteFile(string imgUrl)
    {
        if (string.IsNullOrWhiteSpace(imgUrl))
        {
            return false;
        }

        try
        {
            var publicId = ExtractPublicIdFromUrl(imgUrl);
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return false;
            }

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }
        catch
        {
            return false;
        }

    }

    private static string? ExtractPublicIdFromUrl(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex == -1)
            {
                return null;
            }
            
            var startIndex = uploadIndex + 1;
            if (segments[startIndex].StartsWith("v") && long.TryParse(segments[startIndex][1..], out _))
            {
                startIndex++;
            }
            
            var publicIdParts = segments.Skip(startIndex).ToArray();

            if (publicIdParts.Length == 0)
                return null;
            
            var last = publicIdParts[^1];
            var lastDot = last.LastIndexOf('.');
            if (lastDot >= 0)
            {
                publicIdParts[^1] = last[..lastDot];
            }

            return string.Join("/", publicIdParts);
        }
        catch
        {
            return null;
        }
    }
    private static bool IsValidImage(IFormFile file)
    {
        var allowedExtensions = new[] { 
            // Image files
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp",
            // PDF files
            ".pdf",
            // Archive files
            ".zip", ".rar", ".7z", ".tar", ".gz"
        };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(fileExtension);
    }

}