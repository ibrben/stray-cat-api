using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace StrayCat.Application.Services
{
    public class R2StorageService : IR2StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _baseUrl;

        public R2StorageService(IConfiguration configuration)
        {
            var accessKey = configuration["CloudflareR2:AccessKeyId"];
            var secretKey = configuration["CloudflareR2:SecretAccessKey"];
            var serviceUrl = configuration["CloudflareR2:ServiceUrl"];
            _bucketName = configuration["CloudflareR2:BucketName"];
            _baseUrl = configuration["CloudflareR2:BaseUrl"];

            var config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
                AuthenticationRegion = "auto"
            };

            _s3Client = new AmazonS3Client(accessKey, secretKey, config);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName, string folder = "trip-images")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Invalid file type. Only images are allowed.");

            // Validate file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("File size cannot exceed 10MB.");

            // Generate unique file name
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var key = $"{folder}/{uniqueFileName}";

            using var stream = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };

            var response = await _s3Client.PutObjectAsync(request);
            
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new InvalidOperationException("Failed to upload file to R2.");

            return $"{_baseUrl}/{key}";
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                // Extract key from URL
                var uri = new Uri(fileUrl);
                var key = uri.AbsolutePath.TrimStart('/');
                
                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                var response = await _s3Client.DeleteObjectAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GeneratePresignedUrlAsync(string fileName, string folder = "trip-images", TimeSpan? expiry = null)
        {
            var key = $"{folder}/{fileName}";
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.UtcNow.Add(expiry ?? TimeSpan.FromHours(1))
            };

            return await _s3Client.GetPreSignedURLAsync(request);
        }
    }
}
