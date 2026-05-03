using SoundSphereApi.DTOs.Analytics;
using SoundSphereApi.Models.Analytics;
using SoundSphereApi.Models.Identity;
using SoundSphereApi.Models.Music;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class ListeningHistoryService : IListeningHistoryService
    {
        private readonly IRepository<ListeningHistory> _historyRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Track> _trackRepository;
        private readonly IRepository<Artist> _artistRepository;

        public ListeningHistoryService(
            IRepository<ListeningHistory> historyRepository,
            IRepository<User> userRepository,
            IRepository<Track> trackRepository,
            IRepository<Artist> artistRepository)
        {
            _historyRepository = historyRepository;
            _userRepository = userRepository;
            _trackRepository = trackRepository;
            _artistRepository = artistRepository;
        }

        public async Task<IEnumerable<ListeningHistoryGetDto>> GetUserListeningHistoryAsync(int userId)
        {
            var histories = await _historyRepository.FindAllAsync(h => h.UserId == userId);
            var tracks = await _trackRepository.GetAllAsync();
            var artists = await _artistRepository.GetAllAsync();

            var result = from history in histories
                         join track in tracks on history.TrackId equals track.Id
                         select new ListeningHistoryGetDto
                         {
                             Id = history.Id,
                             UserId = history.UserId,
                             TrackId = track.Id,
                             TrackTitle = track.Title,
                             AudioUrl = track.AudioUrl,
                             CoverImageUrl = track.CoverImageUrl,
                             ArtistId = track.ArtistId,
                             ArtistName = artists.FirstOrDefault(a => a.Id == track.ArtistId)?.Name ?? string.Empty,
                             PlayedAt = history.PlayedAt
                         };

            return result.OrderByDescending(x => x.PlayedAt).ToList();
        }

        public async Task<string> AddListeningHistoryAsync(int userId, ListeningHistoryCreateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return "User not found.";
            }

            var track = await _trackRepository.GetByIdAsync(dto.TrackId);
            if (track == null)
            {
                return "Track not found.";
            }

            var history = new ListeningHistory
            {
                UserId = userId,
                TrackId = dto.TrackId,
                PlayedAt = DateTime.UtcNow
            };

            track.PlayCount += 1;
            track.UpdatedAt = DateTime.UtcNow;

            await _historyRepository.AddAsync(history);
            _trackRepository.Update(track);

            await _historyRepository.SaveChangesAsync();

            return "Listening history added successfully.";
        }

    }
}
