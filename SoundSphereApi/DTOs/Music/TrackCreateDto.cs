namespace SoundSphereApi.DTOs.Music
{
    public class TrackCreateDto
    {
        public string Title { get; set; } = null!;
        public string? AudioUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public int ArtistId { get; set; }
        public int AlbumId { get; set; }
        public int GenreId { get; set; }
    }
}
