using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.DTOs.Music;
using SoundSphereApi.Helpers;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TracksController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TracksController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTracks()
        {
            var tracks = await _trackService.GetAllTracksAsync();
            return Ok(ApiResponse<IEnumerable<TrackGetDto>>.Ok(tracks));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrackById(int id)
        {
            var track = await _trackService.GetTrackByIdAsync(id);

            if (track == null)
            {
                return NotFound(ApiResponse.Fail("Track not found."));
            }

            return Ok(ApiResponse<TrackGetDto>.Ok(track));
        }

        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopularTracks([FromQuery] int count = 10)
        {
            var tracks = await _trackService.GetPopularTracksAsync(count);
            return Ok(ApiResponse<IEnumerable<TrackGetDto>>.Ok(tracks));
        }

        [HttpGet("recent")]
        [Authorize]
        public async Task<IActionResult> GetRecentlyPlayed([FromQuery] int count = 10)
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var tracks = await _trackService.GetRecentlyPlayedAsync(userId.Value, count);
            return Ok(ApiResponse<IEnumerable<TrackGetDto>>.Ok(tracks));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTrack([FromBody] TrackCreateDto trackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _trackService.CreateTrackAsync(trackDto);
            return Ok(ApiResponse.Ok("Track created successfully."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTrack(int id, [FromBody] TrackUpdateDto trackDto)
        {
            if (id != trackDto.Id)
            {
                return BadRequest(ApiResponse.Fail("Track ID mismatch."));
            }

            var existingTrack = await _trackService.GetTrackByIdAsync(id);
            if (existingTrack == null)
            {
                return NotFound(ApiResponse.Fail("Track not found."));
            }

            await _trackService.UpdateTrackAsync(trackDto);
            return Ok(ApiResponse.Ok("Track updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrack(int id)
        {
            var existingTrack = await _trackService.GetTrackByIdAsync(id);
            if (existingTrack == null)
            {
                return NotFound(ApiResponse.Fail("Track not found."));
            }

            await _trackService.DeleteTrackAsync(id);
            return Ok(ApiResponse.Ok("Track deleted successfully."));
        }
    }
}
