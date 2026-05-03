namespace SoundSphereApi.DTOs.Auth
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
    }
}
