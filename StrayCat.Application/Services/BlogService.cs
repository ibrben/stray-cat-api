using Microsoft.EntityFrameworkCore;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;
using StrayCat.Domain.Entities;
using StrayCat.Domain.Enums;
using StrayCat.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace StrayCat.Application.Services
{
    public class BlogService : IBlogService
    {
        private readonly StrayCatDbContext _context;

        public BlogService(StrayCatDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogDto>> GetAllBlogsAsync()
        {
            var blogs = await _context.Blogs
                .Include(b => b.Trip)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return blogs.Select(MapToBlogDto);
        }

        public async Task<BlogDto?> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.Id == id);

            return blog != null ? MapToBlogDto(blog) : null;
        }

        public async Task<BlogDto?> GetBlogBySlugAsync(string slug)
        {
            var blog = await _context.Blogs
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.Slug == slug);

            return blog != null ? MapToBlogDto(blog) : null;
        }

        public async Task<BlogDto> CreateBlogAsync(CreateBlogDto blogDto)
        {
            var blog = new Blog
            {
                Title = blogDto.Title,
                Content = blogDto.Content,
                Author = blogDto.Author,
                PublisherName = blogDto.PublisherName,
                Excerpt = blogDto.Excerpt,
                Category = blogDto.Category,
                FeaturedImageUrl = blogDto.FeaturedImageUrl,
                Slug = blogDto.Slug,
                IsPublished = blogDto.IsPublished,
                PublishedAt = blogDto.IsPublished ? DateTime.UtcNow : null,
                TripId = blogDto.TripId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            if (string.IsNullOrEmpty(blogDto.FeaturedImageUrl))
            {
                var match = Regex.Match(blogDto.Content, @"!\[.*?\]\((.*?)\)", RegexOptions.Singleline);
                if (match.Success)
                {
                    blog.FeaturedImageUrl = match.Groups[1].Value;
                }
            }

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return await GetBlogByIdAsync(blog.Id);
        }

        public async Task<BlogDto?> UpdateBlogAsync(int id, UpdateBlogDto blogDto)
        {
            var existingBlog = await _context.Blogs
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBlog == null)
                return null;

            existingBlog.Title = blogDto.Title;
            existingBlog.Content = blogDto.Content;
            existingBlog.Author = blogDto.Author;
            existingBlog.PublisherName = blogDto.PublisherName;
            existingBlog.Excerpt = blogDto.Excerpt;
            existingBlog.Category = blogDto.Category;
            existingBlog.FeaturedImageUrl = blogDto.FeaturedImageUrl;
            existingBlog.Slug = blogDto.Slug;
            existingBlog.TripId = blogDto.TripId;
            existingBlog.UpdatedAt = DateTime.UtcNow;

            if (string.IsNullOrEmpty(existingBlog.FeaturedImageUrl))
            {
                var match = Regex.Match(blogDto.Content, @"!\[.*?\]\((.*?)\)", RegexOptions.Singleline);
                if (match.Success)
                {
                    existingBlog.FeaturedImageUrl = match.Groups[1].Value;
                }
            }

            // Update published status and date
            if (blogDto.IsPublished && !existingBlog.IsPublished)
            {
                existingBlog.IsPublished = true;
                existingBlog.PublishedAt = DateTime.UtcNow;
                existingBlog.PublicationStatus = PublicationStatus.Published;
            }
            else if (!blogDto.IsPublished)
            {
                existingBlog.IsPublished = false;
                existingBlog.PublishedAt = null;
                existingBlog.PublicationStatus = PublicationStatus.Draft;
            }

            await _context.SaveChangesAsync();

            return MapToBlogDto(existingBlog);
        }

        public async Task<bool> DeleteBlogAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
                return false;

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BlogDto?> CreateGuestBlogAsync(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return await GetBlogByIdAsync(blog.Id);
        }

        private static BlogDto MapToBlogDto(Blog blog)
        {
            return new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                Author = blog.Author,
                FeaturedImageUrl = blog.FeaturedImageUrl,
                Slug = blog.Slug,
                IsPublished = blog.IsPublished,
                PublishedAt = blog.PublishedAt,
                TripId = blog.TripId,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt,
                Trip = blog.Trip != null ? new TripSummaryDto
                {
                    TripId = blog.Trip.Id,
                    Title = blog.Trip.Title
                } : null,
                PublisherName = blog.PublisherName,
                Excerpt = blog.Excerpt,
                Category = blog.Category,
                PublicationStatus = blog.PublicationStatus.ToString().ToLowerInvariant(),
                SourceInviteId = blog.SourceInviteId
            };
        }
    }
}
