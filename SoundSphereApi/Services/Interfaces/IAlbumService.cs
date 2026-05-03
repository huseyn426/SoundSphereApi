using SoundSphereApi.DTOs.Music;

namespace SoundSphereApi.Services.Interfaces
{
    public interface IAlbumService
    {
        Task<IEnumerable<AlbumGetDto>> GetAllAlbumsAsync();
        Task<AlbumGetDto?> GetAlbumByIdAsync(int id);
        Task<string> CreateAlbumAsync(AlbumCreateDto dto);
        Task<string> UpdateAlbumAsync(AlbumUpdateDto dto);
        Task<string> DeleteAlbumAsync(int id);
    }
}
