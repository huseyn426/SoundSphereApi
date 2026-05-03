using SoundSphereApi.DTOs.Analytics;

namespace SoundSphereApi.Services.Interfaces
{
    public interface IListeningHistoryService
    {
        Task<IEnumerable<ListeningHistoryGetDto>> GetUserListeningHistoryAsync(int userId);
        Task<string> AddListeningHistoryAsync(int userId, ListeningHistoryCreateDto dto);
    }
}
