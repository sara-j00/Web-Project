using Application.Abstraction;

namespace Infrastructure.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(_basePath, uniqueFileName);

        using var file = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(file);

        return $"/images/{uniqueFileName}";
    }

    public Task DeleteAsync(string imageUrl)
    {
        var fileName = imageUrl.Replace("/images/", "");
        var fullPath = Path.Combine(_basePath, fileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}