using Microsoft.AspNetCore.Mvc;
using StrayCat.Application.DTOs;
using StrayCat.Application.Services;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripImagesController : ControllerBase
    {
        private readonly ITripImageService _tripImageService;
        private readonly IR2StorageService _r2StorageService;
        private readonly IConfiguration _configuration;

        public TripImagesController(ITripImageService tripImageService, IR2StorageService r2StorageService, IConfiguration configuration)
        {
            _tripImageService = tripImageService;
            _r2StorageService = r2StorageService;
            _configuration = configuration;
        }

        // GET: api/tripimages/trip/{tripId}
        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetTripImages(int tripId)
        {
            var images = await _tripImageService.GetTripImagesAsync(tripId);
            return Ok(images);
        }

        // GET: api/tripimages/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripImage(int id)
        {
            var image = await _tripImageService.GetTripImageByIdAsync(id);
            if (image == null)
                return NotFound();

            return Ok(image);
        }

        // POST: api/tripimages/presigned-url
        [HttpPost("presigned-url")]
        public async Task<IActionResult> GetPresignedUrl([FromBody] PresignedUrlRequestDto request)
        {
            try
            {
                var fileName = $"{request.TripId}_{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";
                var presignedUrl = await _r2StorageService.GeneratePresignedUrlAsync(fileName, "trip-images");
                var cdnUrl = $"{_configuration["CloudflareR2:CdnUrl"]}/trip-images/{fileName}";
                
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

        // POST: api/tripimages/confirm-upload
        [HttpPost("confirm-upload")]
        public async Task<IActionResult> ConfirmUpload([FromBody] ConfirmUploadDto confirmDto)
        {
            try
            {
                var cdnUrl = $"{_configuration["CloudflareR2:CdnUrl"]}/trip-images/{confirmDto.FileName}";
                
                var createImageDto = new CreateTripImageDto
                {
                    TripId = confirmDto.TripId,
                    ImageUrl = cdnUrl,
                    DisplayOrder = confirmDto.DisplayOrder
                };

                var tripImage = await _tripImageService.CreateTripImageAsync(createImageDto);
                return Ok(tripImage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to confirm upload.");
            }
        }

        // POST: api/tripimages
        [HttpPost]
        public async Task<IActionResult> CreateTripImage([FromBody] CreateTripImageDto imageDto)
        {
            var image = await _tripImageService.CreateTripImageAsync(imageDto);
            return CreatedAtAction(nameof(GetTripImage), new { id = image.Id }, image);
        }

        // PUT: api/tripimages/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTripImage(int id, [FromBody] TripImageDto imageDto)
        {
            var success = await _tripImageService.UpdateTripImageAsync(id, imageDto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/tripimages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTripImage(int id)
        {
            try
            {
                // Get image info to delete file from R2
                var image = await _tripImageService.GetTripImageByIdAsync(id);
                if (image == null)
                    return NotFound();

                // Delete file from R2
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    await _r2StorageService.DeleteFileAsync(image.ImageUrl);
                }

                // Delete image record
                var success = await _tripImageService.DeleteTripImageAsync(id);
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
