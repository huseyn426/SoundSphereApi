using SoundSphereApi.DTOs.Playlist;

namespace SoundSphereApi.Services.Interfaces
{
    public interface IPlaylistService
    {
        Task<IEnumerable<PlaylistGetDto>> GetAllPlaylistsAsync();
        Task<PlaylistGetDto?> GetPlaylistByIdAsync(int id);
        Task CreatePlaylistAsync(int userId, PlaylistCreateDto playlistDto);
        Task<string> UpdatePlaylistAsync(int id, int userId, PlaylistUpdateDto dto);
        Task<string> DeletePlaylistAsync(int id, int userId);

        Task<string> AddTrackToPlaylistAsync(int userId, AddTrackToPlaylistDto dto);
        Task<string> RemoveTrackFromPlaylistAsync(int playlistId, int trackId, int userId);
        Task<IEnumerable<PlaylistTrackGetDto>> GetTracksByPlaylistIdAsync(int playlistId);
    }
}
