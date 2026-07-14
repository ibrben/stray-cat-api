using StrayCat.Application.DTOs;

namespace StrayCat.Application.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogDto>> GetAllBlogsAsync();
        Task<BlogDto?> GetBlogByIdAsync(int id);
        Task<BlogDto?> GetBlogBySlugAsync(string slug);
        Task<BlogDto> CreateBlogAsync(CreateBlogDto blog);
        Task<BlogDto?> UpdateBlogAsync(int id, UpdateBlogDto blog);
        Task<bool> DeleteBlogAsync(int id);
    }
}
