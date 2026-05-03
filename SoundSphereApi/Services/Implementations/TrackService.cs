using SoundSphereApi.DTOs.Music;
using SoundSphereApi.Models.Analytics;
using SoundSphereApi.Models.Music;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class TrackService : ITrackService
    {
        private readonly IRepository<Track> _trackRepository;
        private readonly IRepository<Artist> _artistRepository;
        private readonly IRepository<Album> _albumRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<ListeningHistory> _historyRepository;

        public TrackService(
            IRepository<Track> trackRepository,
            IRepository<Artist> artistRepository,
            IRepository<Album> albumRepository,
            IRepository<Genre> genreRepository,
            IRepository<ListeningHistory> historyRepository)
        {
            _trackRepository = trackRepository;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
            _historyRepository = historyRepository;
        }

        public async Task<IEnumerable<TrackGetDto>> GetAllTracksAsync()
        {
            var tracks = await _trackRepository.GetAllAsync();
            var artists = await _artistRepository.GetAllAsync();
            var albums = await _albumRepository.GetAllAsync();
            var genres = await _genreRepository.GetAllAsync();

            var result = tracks.Select(track => new TrackGetDto
            {
                Id = track.Id,
                Title = track.Title,
                AudioUrl = track.AudioUrl,
                CoverImageUrl = track.CoverImageUrl,
                Duration = track.Duration,
                PlayCount = track.PlayCount,
                ArtistId = track.ArtistId,
                ArtistName = artists.FirstOrDefault(a => a.Id == track.ArtistId)?.Name ?? string.Empty,
                AlbumId = track.AlbumId,
                AlbumTitle = albums.FirstOrDefault(a => a.Id == track.AlbumId)?.Title ?? string.Empty,
                GenreId = track.GenreId,
                GenreName = genres.FirstOrDefault(g => g.Id == track.GenreId)?.Name ?? string.Empty
            });

            return result;
        }

        public async Task<TrackGetDto?> GetTrackByIdAsync(int id)
        {
            var track = await _trackRepository.GetByIdAsync(id);
            if (track == null)
            {
                return null;
            }

            var artist = await _artistRepository.GetByIdAsync(track.ArtistId);
            var album = await _albumRepository.GetByIdAsync(track.AlbumId);
            var genre = await _genreRepository.GetByIdAsync(track.GenreId);

            return new TrackGetDto
            {
                Id = track.Id,
                Title = track.Title,
                AudioUrl = track.AudioUrl,
                CoverImageUrl = track.CoverImageUrl,
                Duration = track.Duration,
                PlayCount = track.PlayCount,
                ArtistId = track.ArtistId,
                ArtistName = artist?.Name ?? string.Empty,
                AlbumId = track.AlbumId,
                AlbumTitle = album?.Title ?? string.Empty,
                GenreId = track.GenreId,
                GenreName = genre?.Name ?? string.Empty
            };
        }

        public async Task CreateTrackAsync(TrackCreateDto trackDto)
        {
            var track = new Track
            {
                Title = trackDto.Title,
                AudioUrl = trackDto.AudioUrl,
                CoverImageUrl = trackDto.CoverImageUrl,
                Duration = trackDto.Duration,
                ArtistId = trackDto.ArtistId,
                AlbumId = trackDto.AlbumId,
                GenreId = trackDto.GenreId
            };

            await _trackRepository.AddAsync(track);
            await _trackRepository.SaveChangesAsync();
        }

        public async Task UpdateTrackAsync(TrackUpdateDto trackDto)
        {
            var existingTrack = await _trackRepository.GetByIdAsync(trackDto.Id);

            if (existingTrack == null)
            {
                return;
            }

            existingTrack.Title = trackDto.Title;
            existingTrack.AudioUrl = trackDto.AudioUrl;
            existingTrack.CoverImageUrl = trackDto.CoverImageUrl;
            existingTrack.Duration = trackDto.Duration;
            existingTrack.ArtistId = trackDto.ArtistId;
            existingTrack.AlbumId = trackDto.AlbumId;
            existingTrack.GenreId = trackDto.GenreId;
            existingTrack.PlayCount = trackDto.PlayCount;
            existingTrack.UpdatedAt = DateTime.UtcNow;

            _trackRepository.Update(existingTrack);
            await _trackRepository.SaveChangesAsync();
        }

        public async Task DeleteTrackAsync(int id)
        {
            var track = await _trackRepository.GetByIdAsync(id);

            if (track != null)
            {
                _trackRepository.Delete(track);
                await _trackRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TrackGetDto>> GetPopularTracksAsync(int count)
        {
            var allTracks = await GetAllTracksAsync();
            return allTracks.OrderByDescending(t => t.PlayCount).Take(count);
        }

        public async Task<IEnumerable<TrackGetDto>> GetRecentlyPlayedAsync(int userId, int count)
        {
            var histories = await _historyRepository.FindAllAsync(h => h.UserId == userId);
            var recentTrackIds = histories
                .OrderByDescending(h => h.PlayedAt)
                .Select(h => h.TrackId)
                .Distinct()
                .Take(count)
                .ToList();

            var allTracks = await GetAllTracksAsync();
            var trackDict = allTracks.ToDictionary(t => t.Id);

            return recentTrackIds
                .Where(id => trackDict.ContainsKey(id))
                .Select(id => trackDict[id]);
        }
    }
}
