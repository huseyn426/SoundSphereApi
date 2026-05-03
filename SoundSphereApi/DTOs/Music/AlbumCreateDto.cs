namespace SoundSphereApi.DTOs.Music
{
    public class AlbumCreateDto
    {
        public string Title { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ArtistId { get; set; }
    }
}
