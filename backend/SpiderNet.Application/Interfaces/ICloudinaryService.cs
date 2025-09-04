using Microsoft.AspNetCore.Http;

namespace SpiderNet.Application.Interfaces;

public interface ICloudinaryService
{
    Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string folder = "posts");
    Task<CloudinaryUploadResult> UploadVideoAsync(IFormFile file, string folder = "posts");
    Task<bool> DeleteResourceAsync(string publicId);
}

public class CloudinaryUploadResult
{
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public long Bytes { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}