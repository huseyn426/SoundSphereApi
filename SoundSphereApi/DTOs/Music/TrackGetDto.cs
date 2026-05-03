namespace SoundSphereApi.DTOs.Music
{
    public class TrackGetDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? AudioUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public int PlayCount { get; set; }

        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = null!;

        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; } = null!;

        public int GenreId { get; set; }
        public string GenreName { get; set; } = null!;
    }
}
