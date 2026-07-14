using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IStorageService _r2StorageService;
        private readonly IConfiguration _configuration;

        public BlogsController(IBlogService blogService, IStorageService r2StorageService, IConfiguration configuration)
        {
            _blogService = blogService;
            _r2StorageService = r2StorageService;
            _configuration = configuration;
        }

        // GET: api/blogs
        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            var blogs = await _blogService.GetAllBlogsAsync();
            return Ok(blogs);
        }

        // GET: api/blogs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlog(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null)
                return NotFound();
            return Ok(blog);
        }

        // GET: api/blogs/slug/{slug}
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBlogBySlug(string slug)
        {
            var blog = await _blogService.GetBlogBySlugAsync(slug);
            if (blog == null)
                return NotFound();
            return Ok(blog);
        }

        // POST: api/blogs
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogDto blog)
        {
            var createdBlog = await _blogService.CreateBlogAsync(blog);
            return CreatedAtAction(nameof(GetBlog), new { id = createdBlog.Id }, createdBlog);
        }

        // PUT: api/blogs/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] UpdateBlogDto blog)
        {
            var result = await _blogService.UpdateBlogAsync(id, blog);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // DELETE: api/blogs/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var result = await _blogService.DeleteBlogAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        // POST: api/blogs/presigned-url
        [HttpPost("presigned-url")]
        public async Task<IActionResult> GetPresignedUrl([FromBody] BlogPresignedUrlRequestDto request)
        {
            try
            {
                var fileName = $"{request.BlogId}-{request.FileName}";
                var presignedUrl = await _r2StorageService.GeneratePresignedUrlAsync(fileName, "blog-images/tmp");
                var cdnUrl = $"{_configuration["CloudflareR2:CdnUrl"]}/blog-images/tmp/{fileName}";
                
                
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
    }
}
