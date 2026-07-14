using Microsoft.EntityFrameworkCore;
using StrayCat.Application.DTOs;
using StrayCat.Domain.Entities;
using StrayCat.Infrastructure.Data;

namespace StrayCat.Application.Services
{
    public interface IBlogImageService
    {
        Task<IEnumerable<BlogImageDto>> GetBlogImagesAsync(int blogId);
        Task<BlogImageDto?> GetBlogImageByIdAsync(int id);
        Task<BlogImageDto> CreateBlogImageAsync(CreateBlogImageDto imageDto);
        Task<bool> UpdateBlogImageAsync(int id, BlogImageDto imageDto);
        Task<bool> DeleteBlogImageAsync(int id);
        Task<bool> UpdateFeaturedImageAsync(int blogId, string cdnUrl);
        Task<List<BlogImageDto>> AddMultipleImagesAsync(MultipleBlogImagesRequestDto request);
    }

    public class BlogImageService : IBlogImageService
    {
        private readonly StrayCatDbContext _context;

        public BlogImageService(StrayCatDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogImageDto>> GetBlogImagesAsync(int blogId)
        {
            var images = await _context.BlogImages
                .Where(bi => bi.BlogId == blogId)
                .OrderBy(bi => bi.DisplayOrder)
                .ToListAsync();

            return images.Select(MapToBlogImageDto);
        }

        public async Task<BlogImageDto?> GetBlogImageByIdAsync(int id)
        {
            var image = await _context.BlogImages.FindAsync(id);
            return image != null ? MapToBlogImageDto(image) : null;
        }

        public async Task<BlogImageDto> CreateBlogImageAsync(CreateBlogImageDto imageDto)
        {
            var blogImage = new BlogImage
            {
                BlogId = imageDto.BlogId,
                ImageUrl = imageDto.ImageUrl,
                DisplayOrder = imageDto.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.BlogImages.Add(blogImage);
            await _context.SaveChangesAsync();

            return await GetBlogImageByIdAsync(blogImage.Id);
        }

        public async Task<bool> UpdateBlogImageAsync(int id, BlogImageDto imageDto)
        {
            var existingImage = await _context.BlogImages.FindAsync(id);
            if (existingImage == null)
                return false;

            existingImage.ImageUrl = imageDto.ImageUrl;
            existingImage.DisplayOrder = imageDto.DisplayOrder;
            existingImage.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBlogImageAsync(int id)
        {
            var image = await _context.BlogImages.FindAsync(id);
            if (image == null)
                return false;

            _context.BlogImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateFeaturedImageAsync(int blogId, string cdnUrl)
        {
            try
            {
                // Find the blog to update
                var blog = await _context.Blogs.FindAsync(blogId);
                if (blog == null)
                    return false;

                // Update the blog's featured image URL
                blog.FeaturedImageUrl = cdnUrl;
                blog.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<BlogImageDto>> AddMultipleImagesAsync(MultipleBlogImagesRequestDto request)
        {
            var addedImages = new List<BlogImageDto>();

            try
            {
                foreach (var imageUrlDto in request.ImageUrls)
                {
                    var blogImage = new BlogImage
                    {
                        BlogId = request.BlogId,
                        ImageUrl = imageUrlDto.Url,
                        DisplayOrder = imageUrlDto.DisplayOrder,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.BlogImages.Add(blogImage);
                    await _context.SaveChangesAsync();

                    addedImages.Add(MapToBlogImageDto(blogImage));
                }

                return addedImages;
            }
            catch (Exception)
            {
                // In case of error, return what was successfully added
                return addedImages;
            }
        }

        private static BlogImageDto MapToBlogImageDto(BlogImage blogImage)
        {
            return new BlogImageDto
            {
                Id = blogImage.Id,
                BlogId = blogImage.BlogId,
                ImageUrl = blogImage.ImageUrl,
                DisplayOrder = blogImage.DisplayOrder,
                CreatedAt = blogImage.CreatedAt,
                UpdatedAt = blogImage.UpdatedAt
            };
        }
    }
}
