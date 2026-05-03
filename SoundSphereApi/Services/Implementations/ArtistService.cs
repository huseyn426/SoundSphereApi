using SoundSphereApi.DTOs.Music;
using SoundSphereApi.Models.Music;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class ArtistService : IArtistService
    {
        private readonly IRepository<Artist> _artistRepository;

        public ArtistService(IRepository<Artist> artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<IEnumerable<ArtistGetDto>> GetAllArtistsAsync()
        {
            var artists = await _artistRepository.GetAllAsync();

            return artists.Select(a => new ArtistGetDto
            {
                Id = a.Id,
                Name = a.Name,
                Bio = a.Bio,
                ImageUrl = a.ImageUrl
            });
        }

        public async Task<ArtistGetDto?> GetArtistByIdAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);

            if (artist == null)
            {
                return null;
            }

            return new ArtistGetDto
            {
                Id = artist.Id,
                Name = artist.Name,
                Bio = artist.Bio,
                ImageUrl = artist.ImageUrl
            };
        }

        public async Task CreateArtistAsync(ArtistCreateDto dto)
        {
            var artist = new Artist
            {
                Name = dto.Name,
                Bio = dto.Bio,
                ImageUrl = dto.ImageUrl
            };

            await _artistRepository.AddAsync(artist);
            await _artistRepository.SaveChangesAsync();
        }

        public async Task<string> UpdateArtistAsync(ArtistUpdateDto dto)
        {
            var artist = await _artistRepository.GetByIdAsync(dto.Id);

            if (artist == null)
            {
                return "Artist not found.";
            }

            artist.Name = dto.Name;
            artist.Bio = dto.Bio;
            artist.ImageUrl = dto.ImageUrl;
            artist.UpdatedAt = DateTime.UtcNow;

            _artistRepository.Update(artist);
            await _artistRepository.SaveChangesAsync();

            return "Artist updated successfully.";
        }

        public async Task<string> DeleteArtistAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);

            if (artist == null)
            {
                return "Artist not found.";
            }

            _artistRepository.Delete(artist);
            await _artistRepository.SaveChangesAsync();

            return "Artist deleted successfully.";
        }
    }
}
