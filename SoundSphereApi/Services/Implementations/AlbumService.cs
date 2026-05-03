using SoundSphereApi.DTOs.Music;
using SoundSphereApi.Models.Music;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class AlbumService : IAlbumService
    {
        private readonly IRepository<Album> _albumRepository;
        private readonly IRepository<Artist> _artistRepository;

        public AlbumService(
            IRepository<Album> albumRepository,
            IRepository<Artist> artistRepository)
        {
            _albumRepository = albumRepository;
            _artistRepository = artistRepository;
        }

        public async Task<IEnumerable<AlbumGetDto>> GetAllAlbumsAsync()
        {
            var albums = await _albumRepository.GetAllAsync();
            var artists = await _artistRepository.GetAllAsync();

            return albums.Select(a => new AlbumGetDto
            {
                Id = a.Id,
                Title = a.Title,
                CoverImageUrl = a.CoverImageUrl,
                ReleaseDate = a.ReleaseDate,
                ArtistId = a.ArtistId,
                ArtistName = artists.FirstOrDefault(ar => ar.Id == a.ArtistId)?.Name ?? string.Empty
            });
        }

        public async Task<AlbumGetDto?> GetAlbumByIdAsync(int id)
        {
            var album = await _albumRepository.GetByIdAsync(id);

            if (album == null)
            {
                return null;
            }

            var artist = await _artistRepository.GetByIdAsync(album.ArtistId);

            return new AlbumGetDto
            {
                Id = album.Id,
                Title = album.Title,
                CoverImageUrl = album.CoverImageUrl,
                ReleaseDate = album.ReleaseDate,
                ArtistId = album.ArtistId,
                ArtistName = artist?.Name ?? string.Empty
            };
        }

        public async Task<string> CreateAlbumAsync(AlbumCreateDto dto)
        {
            var artist = await _artistRepository.GetByIdAsync(dto.ArtistId);

            if (artist == null)
            {
                return "Artist not found.";
            }

            var album = new Album
            {
                Title = dto.Title,
                CoverImageUrl = dto.CoverImageUrl,
                ReleaseDate = dto.ReleaseDate,
                ArtistId = dto.ArtistId
            };

            await _albumRepository.AddAsync(album);
            await _albumRepository.SaveChangesAsync();

            return "Album created successfully.";
        }

        public async Task<string> UpdateAlbumAsync(AlbumUpdateDto dto)
        {
            var album = await _albumRepository.GetByIdAsync(dto.Id);

            if (album == null)
            {
                return "Album not found.";
            }

            var artist = await _artistRepository.GetByIdAsync(dto.ArtistId);
            if (artist == null)
            {
                return "Artist not found.";
            }

            album.Title = dto.Title;
            album.CoverImageUrl = dto.CoverImageUrl;
            album.ReleaseDate = dto.ReleaseDate;
            album.ArtistId = dto.ArtistId;
            album.UpdatedAt = DateTime.UtcNow;

            _albumRepository.Update(album);
            await _albumRepository.SaveChangesAsync();

            return "Album updated successfully.";
        }

        public async Task<string> DeleteAlbumAsync(int id)
        {
            var album = await _albumRepository.GetByIdAsync(id);

            if (album == null)
            {
                return "Album not found.";
            }

            _albumRepository.Delete(album);
            await _albumRepository.SaveChangesAsync();

            return "Album deleted successfully.";
        }
    }
}
