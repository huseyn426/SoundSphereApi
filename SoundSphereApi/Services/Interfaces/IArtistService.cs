using SoundSphereApi.DTOs.Music;

namespace SoundSphereApi.Services.Interfaces
{
    public interface IArtistService
    {
        Task<IEnumerable<ArtistGetDto>> GetAllArtistsAsync();
        Task<ArtistGetDto?> GetArtistByIdAsync(int id);
        Task CreateArtistAsync(ArtistCreateDto dto);
        Task<string> UpdateArtistAsync(ArtistUpdateDto dto);
        Task<string> DeleteArtistAsync(int id);
    }
}
