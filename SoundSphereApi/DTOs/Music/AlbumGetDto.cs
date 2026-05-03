namespace SoundSphereApi.DTOs.Music
{
    public class AlbumGetDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = null!;
    }
}
