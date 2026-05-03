namespace SoundSphereApi.DTOs.Playlist
{
    public class PlaylistUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
