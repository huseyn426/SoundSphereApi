namespace SoundSphereApi.DTOs.Playlist
{
    public class PlaylistCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;
        public string? CoverImageUrl { get; set; }
    }
}
