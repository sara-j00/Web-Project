namespace Application.Abstraction;

public interface IImageStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName);
    Task DeleteAsync(string imageUrl);
}