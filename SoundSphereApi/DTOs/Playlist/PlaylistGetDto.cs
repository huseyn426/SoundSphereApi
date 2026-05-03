namespace SoundSphereApi.DTOs.Playlist
{
    public class PlaylistGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public string? CoverImageUrl { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}
