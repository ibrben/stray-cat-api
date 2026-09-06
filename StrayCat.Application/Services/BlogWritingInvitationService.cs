using Microsoft.EntityFrameworkCore;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;
using StrayCat.Application.Settings;
using StrayCat.Domain.Entities;
using StrayCat.Domain.Enums;
using StrayCat.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace StrayCat.Application.Services
{
    public class BlogWritingInvitationService : IBlogWritingInvitationService
    {
        private readonly StrayCatDbContext _context;
        private readonly FrontendSettings _frontendSettings;

        public BlogWritingInvitationService(
            StrayCatDbContext context,
            FrontendSettings frontendSettings)
        {
            _context = context;
            _frontendSettings = frontendSettings;
        }

        public async Task<BlogWritingInvitationDto> CreateInvitationAsync(CreateBlogWritingInvitationDto dto, int userId)
        {
            // Validate expiration date
            if (dto.ExpiresAt <= DateTime.UtcNow)
            {
                throw new ArgumentException("Expiration date must be in the future.");
            }

            // Validate trip ownership if provided
            if (dto.TripId.HasValue)
            {
                var trip = await _context.Trips.FindAsync(dto.TripId.Value);
                if (trip == null || trip.OrganizerId != userId)
                {
                    throw new ArgumentException("Invalid trip or you don't have permission to use this trip.");
                }
            }

            // Generate secure token
            var rawToken = GenerateSecureToken();
            var tokenHash = HashToken(rawToken);

            // Generate invitation ID
            var invitationId = GenerateInvitationId();

            var invitation = new BlogWritingInvitation
            {
                Id = invitationId,
                OwnerUserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = dto.ExpiresAt,
                MaxSubmissions = dto.MaxSubmissions,
                SubmissionCount = 0,
                TripId = dto.TripId,
                CreatedAt = DateTime.UtcNow
            };

            _context.BlogWritingInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            // Build writing URL
            var writingUrl = $"{_frontendSettings.Url}/write/guest/{rawToken}";

            return new BlogWritingInvitationDto
            {
                Id = invitation.Id,
                WritingUrl = writingUrl,
                ExpiresAt = invitation.ExpiresAt,
                MaxSubmissions = invitation.MaxSubmissions,
                SubmissionCount = invitation.SubmissionCount,
                Status = "active",
                CreatedAt = invitation.CreatedAt,
                TripId = invitation.TripId
            };
        }

        public async Task<IEnumerable<BlogWritingInvitationDto>> GetUserInvitationsAsync(int userId)
        {
            var invitations = await _context.BlogWritingInvitations
                .Include(bwi => bwi.Trip)
                .Where(bwi => bwi.OwnerUserId == userId)
                .OrderByDescending(bwi => bwi.CreatedAt)
                .ToListAsync();

            return invitations.Select(invitation => new BlogWritingInvitationDto
            {
                Id = invitation.Id,
                WritingUrl = null, // Never return the raw token after creation
                ExpiresAt = invitation.ExpiresAt,
                MaxSubmissions = invitation.MaxSubmissions,
                SubmissionCount = invitation.SubmissionCount,
                Status = DeriveStatus(invitation),
                CreatedAt = invitation.CreatedAt,
                TripId = invitation.TripId
            });
        }

        public async Task<bool> RevokeInvitationAsync(string id, int userId)
        {
            var invitation = await _context.BlogWritingInvitations
                .FirstOrDefaultAsync(bwi => bwi.Id == id && bwi.OwnerUserId == userId);

            if (invitation == null)
            {
                return false;
            }

            if (invitation.RevokedAt != null)
            {
                // Already revoked - treat as success for idempotency
                return true;
            }

            invitation.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<GuestBlogWritingContextDto?> GetGuestContextAsync(string token)
        {
            var tokenHash = HashToken(token);
            var invitation = await _context.BlogWritingInvitations
                .Include(bwi => bwi.Owner)
                .Include(bwi => bwi.Trip)
                .FirstOrDefaultAsync(bwi => bwi.TokenHash == tokenHash);

            if (invitation == null)
            {
                return null;
            }

            // Check if invitation is usable
            var status = DeriveStatus(invitation);
            if (status != "active")
            {
                return null; // Return null for expired, revoked, or consumed invitations
            }

            var submissionsRemaining = invitation.MaxSubmissions - invitation.SubmissionCount;

            return new GuestBlogWritingContextDto
            {
                InviterDisplayName = invitation.Owner?.Name,
                ExpiresAt = invitation.ExpiresAt,
                SubmissionsRemaining = submissionsRemaining,
                Trip = invitation.Trip != null ? new GuestBlogTripContextDto
                {
                    Id = invitation.Trip.Id,
                    Title = invitation.Trip.Title
                } : null
            };
        }

        public async Task<GuestBlogSubmissionResponseDto?> SubmitGuestBlogAsync(string token, GuestBlogSubmissionDto dto)
        {
            var tokenHash = HashToken(token);
            var invitation = await _context.BlogWritingInvitations
                .Include(bwi => bwi.Owner)
                .Include(bwi => bwi.Trip)
                .FirstOrDefaultAsync(bwi => bwi.TokenHash == tokenHash);

            if (invitation == null)
            {
                return null;
            }

            // Check if invitation is usable
            var status = DeriveStatus(invitation);
            if (status != "active")
            {
                return null; // Invitation is expired, revoked, or consumed
            }

            // Check submission limit
            if (invitation.SubmissionCount >= invitation.MaxSubmissions)
            {
                return null;
            }

            // Validate featured image URL if provided
            if (!string.IsNullOrEmpty(dto.FeaturedImageUrl))
            {
                if (!Uri.TryCreate(dto.FeaturedImageUrl, UriKind.Absolute, out var uri))
                {
                    throw new ArgumentException("Invalid featured image URL.");
                }
            }

            // Create blog in a transaction with submission count increment
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var blog = new Blog
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    Author = dto.PublisherName, // For compatibility with older readers
                    PublisherName = dto.PublisherName,
                    Excerpt = dto.Excerpt,
                    FeaturedImageUrl = dto.FeaturedImageUrl,
                    Slug = GenerateSlug(dto.Title),
                    IsPublished = false,
                    PublicationStatus = PublicationStatus.PendingReview,
                    PublishedAt = null,
                    TripId = invitation.TripId, // Use trip from invitation, not from request
                    SourceInviteId = invitation.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Blogs.Add(blog);

                // Increment submission count
                invitation.SubmissionCount++;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GuestBlogSubmissionResponseDto
                {
                    Id = blog.Id,
                    Status = "pendingReview",
                    PublisherName = blog.PublisherName ?? string.Empty
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private string DeriveStatus(BlogWritingInvitation invitation)
        {
            if (invitation.RevokedAt != null)
            {
                return "revoked";
            }

            if (invitation.ExpiresAt <= DateTime.UtcNow)
            {
                return "expired";
            }

            if (invitation.SubmissionCount >= invitation.MaxSubmissions)
            {
                return "consumed";
            }

            return "active";
        }

        private string GenerateSecureToken()
        {
            // Generate at least 128 bits of cryptographically secure random token
            var bytes = new byte[32]; // 256 bits
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-").TrimEnd('=');
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string GenerateInvitationId()
        {
            // Generate a short ID like "bwi_123"
            return $"bwi_{Random.Shared.Next(100000, 999999)}";
        }

        private string GenerateSlug(string title)
        {
            // Simple slug generation - could be enhanced
            var slug = title.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("!", "")
                .Replace("?", "")
                .Replace("'", "")
                .Replace("\"", "");

            // Ensure uniqueness by appending random suffix if needed
            var baseSlug = slug.Length > 200 ? slug.Substring(0, 200) : slug;
            var finalSlug = baseSlug;
            var counter = 1;

            while (_context.Blogs.Any(b => b.Slug == finalSlug))
            {
                finalSlug = $"{baseSlug}-{counter}";
                counter++;
            }

            return finalSlug;
        }
    }
}