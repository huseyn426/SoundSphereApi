namespace SoundSphereApi.DTOs.Music
{
    public class AlbumUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ArtistId { get; set; }
    }
}
