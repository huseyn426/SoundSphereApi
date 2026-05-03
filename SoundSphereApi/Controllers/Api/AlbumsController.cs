using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.DTOs.Music;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumService _albumService;

        public AlbumsController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAlbums()
        {
            var albums = await _albumService.GetAllAlbumsAsync();
            return Ok(ApiResponse<IEnumerable<AlbumGetDto>>.Ok(albums));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAlbumById(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);

            if (album == null)
            {
                return NotFound(ApiResponse.Fail("Album not found."));
            }

            return Ok(ApiResponse<AlbumGetDto>.Ok(album));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAlbum([FromBody] AlbumCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _albumService.CreateAlbumAsync(dto);

            if (result != "Album created successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAlbum(int id, [FromBody] AlbumUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse.Fail("Album ID mismatch."));
            }

            var result = await _albumService.UpdateAlbumAsync(dto);

            if (result != "Album updated successfully.")
            {
                return result.Contains("not found")
                    ? NotFound(ApiResponse.Fail(result))
                    : BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            var result = await _albumService.DeleteAlbumAsync(id);

            if (result != "Album deleted successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }
    }
}
