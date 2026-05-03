using SoundSphereApi.DTOs.Playlist;
using SoundSphereApi.Models.Music;
using SoundSphereApi.Models.Playlist;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IRepository<PlaylistEntity> _playlistRepository;
        private readonly IRepository<PlaylistTrack> _playlistTrackRepository;
        private readonly IRepository<Track> _trackRepository;
        private readonly IRepository<Artist> _artistRepository;
        private readonly IRepository<Album> _albumRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<SoundSphereApi.Models.Identity.User> _userRepository;

        public PlaylistService(
            IRepository<PlaylistEntity> playlistRepository,
            IRepository<PlaylistTrack> playlistTrackRepository,
            IRepository<Track> trackRepository,
            IRepository<Artist> artistRepository,
            IRepository<Album> albumRepository,
            IRepository<Genre> genreRepository,
            IRepository<SoundSphereApi.Models.Identity.User> userRepository)
        {
            _playlistRepository = playlistRepository;
            _playlistTrackRepository = playlistTrackRepository;
            _trackRepository = trackRepository;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<PlaylistGetDto>> GetAllPlaylistsAsync()
        {
            var playlists = await _playlistRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();

            return playlists.Select(p => new PlaylistGetDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                IsPublic = p.IsPublic,
                CoverImageUrl = p.CoverImageUrl,
                UserId = p.UserId,
                UserName = users.FirstOrDefault(u => u.Id == p.UserId)?.UserName
            });
        }

        public async Task<PlaylistGetDto?> GetPlaylistByIdAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);

            if (playlist == null)
            {
                return null;
            }

            return new PlaylistGetDto
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Description = playlist.Description,
                IsPublic = playlist.IsPublic,
                CoverImageUrl = playlist.CoverImageUrl,
                UserId = playlist.UserId
            };
        }

        public async Task CreatePlaylistAsync(int userId, PlaylistCreateDto playlistDto)
        {
            var playlist = new PlaylistEntity
            {
                Name = playlistDto.Name,
                Description = playlistDto.Description,
                IsPublic = playlistDto.IsPublic,
                CoverImageUrl = playlistDto.CoverImageUrl,
                UserId = userId
            };

            await _playlistRepository.AddAsync(playlist);
            await _playlistRepository.SaveChangesAsync();
        }

        public async Task<string> UpdatePlaylistAsync(int id, int userId, PlaylistUpdateDto dto)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);

            if (playlist == null)
            {
                return "Playlist not found.";
            }

            if (playlist.UserId != userId)
            {
                return "Access denied. You can only edit your own playlists.";
            }

            playlist.Name = dto.Name;
            playlist.Description = dto.Description;
            playlist.IsPublic = dto.IsPublic;
            playlist.CoverImageUrl = dto.CoverImageUrl;
            playlist.UpdatedAt = DateTime.UtcNow;

            _playlistRepository.Update(playlist);
            await _playlistRepository.SaveChangesAsync();

            return "Playlist updated successfully.";
        }

        public async Task<string> DeletePlaylistAsync(int id, int userId)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);

            if (playlist == null)
            {
                return "Playlist not found.";
            }

            if (playlist.UserId != userId)
            {
                return "Access denied. You can only delete your own playlists.";
            }

            _playlistRepository.Delete(playlist);
            await _playlistRepository.SaveChangesAsync();

            return "Playlist deleted successfully.";
        }

        public async Task<string> AddTrackToPlaylistAsync(int userId, AddTrackToPlaylistDto dto)
        {
            var playlist = await _playlistRepository.GetByIdAsync(dto.PlaylistId);
            if (playlist == null)
            {
                return "Playlist not found.";
            }

            if (playlist.UserId != userId)
            {
                return "Access denied. You can only add tracks to your own playlists.";
            }

            var track = await _trackRepository.GetByIdAsync(dto.TrackId);
            if (track == null)
            {
                return "Track not found.";
            }

            var existingPlaylistTrack = await _playlistTrackRepository.FindAsync(pt =>
                pt.PlaylistEntityId == dto.PlaylistId && pt.TrackId == dto.TrackId);

            if (existingPlaylistTrack != null)
            {
                return "Track already exists in playlist.";
            }

            var playlistTrack = new PlaylistTrack
            {
                PlaylistEntityId = dto.PlaylistId,
                TrackId = dto.TrackId
            };

            await _playlistTrackRepository.AddAsync(playlistTrack);
            await _playlistTrackRepository.SaveChangesAsync();

            return "Track added to playlist successfully.";
        }

        public async Task<string> RemoveTrackFromPlaylistAsync(int playlistId, int trackId, int userId)
        {
            var playlist = await _playlistRepository.GetByIdAsync(playlistId);
            if (playlist == null)
            {
                return "Playlist not found.";
            }

            if (playlist.UserId != userId)
            {
                return "Access denied. You can only remove tracks from your own playlists.";
            }

            var playlistTrack = await _playlistTrackRepository.FindAsync(pt =>
                pt.PlaylistEntityId == playlistId && pt.TrackId == trackId);

            if (playlistTrack == null)
            {
                return "Track not found in playlist.";
            }

            _playlistTrackRepository.Delete(playlistTrack);
            await _playlistTrackRepository.SaveChangesAsync();

            return "Track removed from playlist successfully.";
        }

        public async Task<IEnumerable<PlaylistTrackGetDto>> GetTracksByPlaylistIdAsync(int playlistId)
        {
            var playlistTracks = await _playlistTrackRepository.FindAllAsync(pt => pt.PlaylistEntityId == playlistId);
            var tracks = await _trackRepository.GetAllAsync();
            var artists = await _artistRepository.GetAllAsync();
            var albums = await _albumRepository.GetAllAsync();
            var genres = await _genreRepository.GetAllAsync();

            var result = from pt in playlistTracks
                         join t in tracks on pt.TrackId equals t.Id
                         select new PlaylistTrackGetDto
                         {
                             PlaylistTrackId = pt.Id,
                             TrackId = t.Id,
                             Title = t.Title,
                             AudioUrl = t.AudioUrl,
                             CoverImageUrl = t.CoverImageUrl,
                             Duration = t.Duration,
                             PlayCount = t.PlayCount,
                             ArtistId = t.ArtistId,
                             ArtistName = artists.FirstOrDefault(a => a.Id == t.ArtistId)?.Name ?? string.Empty,
                             AlbumId = t.AlbumId,
                             AlbumTitle = albums.FirstOrDefault(a => a.Id == t.AlbumId)?.Title ?? string.Empty,
                             GenreId = t.GenreId,
                             GenreName = genres.FirstOrDefault(g => g.Id == t.GenreId)?.Name ?? string.Empty
                         };

            return result.ToList();
        }
    }
}
