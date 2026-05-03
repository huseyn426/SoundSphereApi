using SoundSphereApi.DTOs.Music;

namespace SoundSphereApi.Services.Interfaces
{
    public interface ITrackService
    {
        Task<IEnumerable<TrackGetDto>> GetAllTracksAsync();
        Task<TrackGetDto?> GetTrackByIdAsync(int id);
        Task CreateTrackAsync(TrackCreateDto trackDto);
        Task UpdateTrackAsync(TrackUpdateDto trackDto);
        Task DeleteTrackAsync(int id);
        Task<IEnumerable<TrackGetDto>> GetPopularTracksAsync(int count);
        Task<IEnumerable<TrackGetDto>> GetRecentlyPlayedAsync(int userId, int count);
    }
}
