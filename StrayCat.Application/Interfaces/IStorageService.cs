using Microsoft.AspNetCore.Http;

namespace StrayCat.Application.Interfaces
{
    public interface IStorageService
    {
        Task<bool> DeleteFileAsync(string fileUrl);
        Task<string> GeneratePresignedUrlAsync(string fileName, string folder = "trip-images", TimeSpan? expiry = null);
    }
}
