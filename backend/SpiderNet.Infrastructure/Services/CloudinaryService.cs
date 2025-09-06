using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SpiderNet.Application.Interfaces;
using SpiderNet.Infrastructure.Settings;

namespace SpiderNet.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    public CloudinaryService(IOptions<CloudinarySetting> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }
    
    public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string folder = "posts")
    {
        if (file.Length <= 0) 
            throw new ArgumentException("File is empty");

        // Validate image type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Only JPEG, PNG, GIF, and WebP images are allowed");

        // Validate size (max 10MB)
        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("Image size cannot exceed 10MB");

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
                .Width(1200)
                .Height(1200)
                .Crop("limit"),
            UseFilename = false,
            UniqueFilename = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception($"Upload failed: {uploadResult.Error.Message}");

        return new CloudinaryUploadResult
        {
            Url = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId,
            Format = uploadResult.Format,
            Bytes = uploadResult.Bytes,
            Width = uploadResult.Width,
            Height = uploadResult.Height
        };
    }
    
    public async Task<CloudinaryUploadResult> UploadVideoAsync(IFormFile file, string folder = "posts")
    {
        if (file.Length <= 0) 
            throw new ArgumentException("File is empty");

        // Validate video type
        var allowedTypes = new[] { "video/mp4", "video/webm", "video/ogg", "video/avi", "video/mov" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Only MP4, WebM, OGG, AVI, and MOV videos are allowed");

        // Validate size (max 100MB)
        if (file.Length > 100 * 1024 * 1024)
            throw new ArgumentException("Video size cannot exceed 100MB");

        using var stream = file.OpenReadStream();

        var uploadParams = new VideoUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            Transformation = new Transformation()
                .Quality("auto")
                .Width(1280)
                .Height(720)
                .Crop("limit"),
            UseFilename = false,
            UniqueFilename = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception($"Upload failed: {uploadResult.Error.Message}");

        return new CloudinaryUploadResult
        {
            Url = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId,
            Format = uploadResult.Format,
            Bytes = uploadResult.Bytes,
            Width = uploadResult.Width,
            Height = uploadResult.Height
        };
    }

    public async Task<CloudinaryUploadResult> UploadGifAsync(IFormFile file, string folder = "comments")
    {
        if (file.Length <= 0)
        {
            throw new ArgumentException("File is empty");
        }
        var allowedTypes = new[] { "image/gif" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
        {
            throw new ArgumentException("Only GIF images are allowed");
        }

        if (file.Length > 20 * 1024 * 1024)
        {
            throw new ArgumentException("Image size cannot exceed 20MB");
        }
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
                .Width(400)
                .Height(400)
                .Crop("limit"),
            UseFilename = false,
            UniqueFilename = true,
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        if (uploadResult.Error is not null)
        {
            throw new Exception($"Upload failed: {uploadResult.Error.Message}");
        }
        return new CloudinaryUploadResult
        {
            Url = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId,
            Format = uploadResult.Format,
            Bytes = uploadResult.Bytes,
            Width = uploadResult.Width,
            Height = uploadResult.Height
        };
    }
    public async Task<bool> DeleteResourceAsync(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return false;

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        
        return result.Result == "ok";
    }
}