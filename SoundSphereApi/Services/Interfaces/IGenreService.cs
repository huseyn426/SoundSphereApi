using SoundSphereApi.DTOs.Music;

namespace SoundSphereApi.Services.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreGetDto>> GetAllGenresAsync();
        Task<GenreGetDto?> GetGenreByIdAsync(int id);
        Task CreateGenreAsync(GenreCreateDto dto);
        Task<string> UpdateGenreAsync(GenreUpdateDto dto);
        Task<string> DeleteGenreAsync(int id);
    }
}
