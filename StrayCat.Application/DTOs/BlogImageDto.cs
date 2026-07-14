namespace StrayCat.Application.DTOs
{
    public class BlogImageDto
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateBlogImageDto
    {
        public int BlogId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
