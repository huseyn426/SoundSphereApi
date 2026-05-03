using SoundSphereApi.DTOs.Music;
using SoundSphereApi.Models.Music;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class GenreService : IGenreService
    {
        private readonly IRepository<Genre> _genreRepository;

        public GenreService(IRepository<Genre> genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<GenreGetDto>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllAsync();

            return genres.Select(g => new GenreGetDto
            {
                Id = g.Id,
                Name = g.Name
            });
        }

        public async Task<GenreGetDto?> GetGenreByIdAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);

            if (genre == null)
            {
                return null;
            }

            return new GenreGetDto
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }

        public async Task CreateGenreAsync(GenreCreateDto dto)
        {
            var genre = new Genre
            {
                Name = dto.Name
            };

            await _genreRepository.AddAsync(genre);
            await _genreRepository.SaveChangesAsync();
        }

        public async Task<string> UpdateGenreAsync(GenreUpdateDto dto)
        {
            var genre = await _genreRepository.GetByIdAsync(dto.Id);

            if (genre == null)
            {
                return "Genre not found.";
            }

            genre.Name = dto.Name;
            genre.UpdatedAt = DateTime.UtcNow;

            _genreRepository.Update(genre);
            await _genreRepository.SaveChangesAsync();

            return "Genre updated successfully.";
        }

        public async Task<string> DeleteGenreAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);

            if (genre == null)
            {
                return "Genre not found.";
            }

            _genreRepository.Delete(genre);
            await _genreRepository.SaveChangesAsync();

            return "Genre deleted successfully.";
        }
    }
}
