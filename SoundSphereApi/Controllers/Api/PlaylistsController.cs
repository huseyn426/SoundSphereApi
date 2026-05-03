using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.DTOs.Playlist;
using SoundSphereApi.Services.Interfaces;
using SoundSphereApi.Helpers;


namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPlaylists()
        {
            var playlists = await _playlistService.GetAllPlaylistsAsync();
            return Ok(ApiResponse<IEnumerable<PlaylistGetDto>>.Ok(playlists));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlaylistById(int id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);

            if (playlist == null)
            {
                return NotFound(ApiResponse.Fail("Playlist not found."));
            }

            return Ok(ApiResponse<PlaylistGetDto>.Ok(playlist));
        }

        [HttpGet("{id}/tracks")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTracksByPlaylistId(int id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);

            if (playlist == null)
            {
                return NotFound(ApiResponse.Fail("Playlist not found."));
            }

            var tracks = await _playlistService.GetTracksByPlaylistIdAsync(id);
            return Ok(ApiResponse<IEnumerable<PlaylistTrackGetDto>>.Ok(tracks));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistCreateDto playlistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            await _playlistService.CreatePlaylistAsync(userId.Value, playlistDto);
            return Ok(ApiResponse.Ok("Playlist created successfully."));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePlaylist(int id, [FromBody] PlaylistUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var result = await _playlistService.UpdatePlaylistAsync(id, userId.Value, dto);

            if (result.Contains("Access denied"))
            {
                return StatusCode(403, ApiResponse.Fail(result));
            }

            if (result != "Playlist updated successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpPost("add-track")]
        [Authorize]
        public async Task<IActionResult> AddTrackToPlaylist([FromBody] AddTrackToPlaylistDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var result = await _playlistService.AddTrackToPlaylistAsync(userId.Value, dto);

            if (result.Contains("Access denied"))
            {
                return StatusCode(403, ApiResponse.Fail(result));
            }

            if (result != "Track added to playlist successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpDelete("{playlistId}/remove-track/{trackId}")]
        [Authorize]
        public async Task<IActionResult> RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var result = await _playlistService.RemoveTrackFromPlaylistAsync(playlistId, trackId, userId.Value);

            if (result.Contains("Access denied"))
            {
                return StatusCode(403, ApiResponse.Fail(result));
            }

            if (result != "Track removed from playlist successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var result = await _playlistService.DeletePlaylistAsync(id, userId.Value);

            if (result.Contains("Access denied"))
            {
                return StatusCode(403, ApiResponse.Fail(result));
            }

            if (result != "Playlist deleted successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }
    }
}
