using Microsoft.AspNetCore.Mvc;
using StrayCat.Application.DTOs;
using StrayCat.Application.Services;
using StrayCat.Application.Interfaces;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogImagesController : ControllerBase
    {
        private readonly IBlogImageService _blogImageService;
        private readonly IStorageService _r2StorageService;
        private readonly IConfiguration _configuration;

        public BlogImagesController(IBlogImageService blogImageService, IStorageService r2StorageService, IConfiguration configuration)
        {
            _blogImageService = blogImageService;
            _r2StorageService = r2StorageService;
            _configuration = configuration;
        }

        // GET: api/blogimages/blog/{blogId}
        [HttpGet("blog/{blogId}")]
        public async Task<IActionResult> GetBlogImages(int blogId)
        {
            var images = await _blogImageService.GetBlogImagesAsync(blogId);
            return Ok(images);
        }

        // GET: api/blogimages/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogImage(int id)
        {
            var image = await _blogImageService.GetBlogImageByIdAsync(id);
            if (image == null)
                return NotFound();

            return Ok(image);
        }

        // POST: api/blogimages/presigned-url
        [HttpPost("presigned-url")]
        public async Task<IActionResult> GetPresignedUrl([FromBody] BlogPresignedUrlRequestDto request)
        {
            try
            {
                var fileName = $"{request.BlogId}_{request.FileName}";
                var presignedUrl = await _r2StorageService.GeneratePresignedUrlAsync(fileName, "blog-images");
                var cdnUrl = $"{_configuration["CloudflareR2:CdnUrl"]}/blog-images/{fileName}";
                
                // If this is a featured image, update the blog's FeaturedImageUrl
                if (request.IsFeaturedImage)
                {
                    var featuredImageCdnUrl = cdnUrl;
                    await _blogImageService.UpdateFeaturedImageAsync(request.BlogId, featuredImageCdnUrl);
                }
                
                return Ok(new PresignedUrlResponseDto
                {
                    FileName = fileName,
                    PresignedUrl = presignedUrl,
                    ImageUrl = cdnUrl,
                    ExpiresIn = 3600 // 1 hour
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to generate presigned URL.");
            }
        }

        // POST: api/blogimages/multiple
        [HttpPost("multiple")]
        public async Task<IActionResult> AddMultipleImages([FromBody] MultipleBlogImagesRequestDto request)
        {
            try
            {
                var addedImages = await _blogImageService.AddMultipleImagesAsync(request);
                return Ok(addedImages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to add multiple images.");
            }
        }

        // POST: api/blogimages/confirm-upload
        [HttpPost("confirm-upload")]
        public async Task<IActionResult> ConfirmUpload([FromBody] BlogConfirmUploadDto confirmDto)
        {
            try
            {
                var cdnUrl = $"{_configuration["CloudflareR2:CdnUrl"]}/blog-images/{confirmDto.FileName}";
                
                var createImageDto = new CreateBlogImageDto
                {
                    BlogId = confirmDto.BlogId,
                    ImageUrl = cdnUrl,
                    DisplayOrder = confirmDto.DisplayOrder
                };

                var blogImage = await _blogImageService.CreateBlogImageAsync(createImageDto);
                return Ok(blogImage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to confirm upload.");
            }
        }

        // POST: api/blogimages
        [HttpPost]
        public async Task<IActionResult> CreateBlogImage([FromBody] CreateBlogImageDto imageDto)
        {
            var image = await _blogImageService.CreateBlogImageAsync(imageDto);
            return CreatedAtAction(nameof(GetBlogImage), new { id = image.Id }, image);
        }

        // PUT: api/blogimages/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogImage(int id, [FromBody] BlogImageDto imageDto)
        {
            var success = await _blogImageService.UpdateBlogImageAsync(id, imageDto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/blogimages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogImage(int id)
        {
            try
            {
                // Get image info to delete file from R2
                var image = await _blogImageService.GetBlogImageByIdAsync(id);
                if (image == null)
                    return NotFound();

                // Delete file from R2
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    await _r2StorageService.DeleteFileAsync(image.ImageUrl);
                }

                // Delete image record
                var success = await _blogImageService.DeleteBlogImageAsync(id);
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting image.");
            }
        }
    }
}
