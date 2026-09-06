using StrayCat.Application.DTOs;

namespace StrayCat.Application.Interfaces
{
    public interface IBlogWritingInvitationService
    {
        Task<BlogWritingInvitationDto> CreateInvitationAsync(CreateBlogWritingInvitationDto dto, int userId);
        Task<IEnumerable<BlogWritingInvitationDto>> GetUserInvitationsAsync(int userId);
        Task<bool> RevokeInvitationAsync(string id, int userId);
        Task<GuestBlogWritingContextDto?> GetGuestContextAsync(string token);
        Task<GuestBlogSubmissionResponseDto?> SubmitGuestBlogAsync(string token, GuestBlogSubmissionDto dto);
    }
}