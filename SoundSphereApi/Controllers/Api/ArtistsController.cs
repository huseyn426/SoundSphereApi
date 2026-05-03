using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.DTOs.Music;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistService;

        public ArtistsController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllArtists()
        {
            var artists = await _artistService.GetAllArtistsAsync();
            return Ok(ApiResponse<IEnumerable<ArtistGetDto>>.Ok(artists));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetArtistById(int id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id);

            if (artist == null)
            {
                return NotFound(ApiResponse.Fail("Artist not found."));
            }

            return Ok(ApiResponse<ArtistGetDto>.Ok(artist));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateArtist([FromBody] ArtistCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _artistService.CreateArtistAsync(dto);
            return Ok(ApiResponse.Ok("Artist created successfully."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateArtist(int id, [FromBody] ArtistUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse.Fail("Artist ID mismatch."));
            }

            var result = await _artistService.UpdateArtistAsync(dto);

            if (result != "Artist updated successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var result = await _artistService.DeleteArtistAsync(id);

            if (result != "Artist deleted successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }
    }
}
