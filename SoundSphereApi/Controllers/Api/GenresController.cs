using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.DTOs.Music;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return Ok(ApiResponse<IEnumerable<GenreGetDto>>.Ok(genres));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);

            if (genre == null)
            {
                return NotFound(ApiResponse.Fail("Genre not found."));
            }

            return Ok(ApiResponse<GenreGetDto>.Ok(genre));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGenre([FromBody] GenreCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _genreService.CreateGenreAsync(dto);
            return Ok(ApiResponse.Ok("Genre created successfully."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse.Fail("Genre ID mismatch."));
            }

            var result = await _genreService.UpdateGenreAsync(dto);

            if (result != "Genre updated successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var result = await _genreService.DeleteGenreAsync(id);

            if (result != "Genre deleted successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }
    }
}
