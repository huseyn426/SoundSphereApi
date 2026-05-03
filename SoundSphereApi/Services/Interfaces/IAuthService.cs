using SoundSphereApi.DTOs.Auth;

namespace SoundSphereApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> TryRestoreSessionFromTokenAsync(string token);
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
        Task<string> UpdateUserProfileAsync(int userId, UpdateProfileDto dto);
    }
}
