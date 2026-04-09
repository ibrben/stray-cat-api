using Microsoft.AspNetCore.Http;

namespace StrayCat.Application.Services
{
    public interface IR2StorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string fileName, string folder = "trip-images");
        Task<bool> DeleteFileAsync(string fileUrl);
        Task<string> GeneratePresignedUrlAsync(string fileName, string folder = "trip-images", TimeSpan? expiry = null);
    }
}
