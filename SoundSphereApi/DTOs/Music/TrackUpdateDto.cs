namespace SoundSphereApi.DTOs.Music
{
    public class TrackUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? AudioUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public int ArtistId { get; set; }
        public int AlbumId { get; set; }
        public int GenreId { get; set; }
        public int PlayCount { get; set; }
    }
}
