namespace SoundSphereApi.DTOs.Analytics
{
    public class ListeningHistoryGetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int TrackId { get; set; }
        public string TrackTitle { get; set; } = null!;
        public string? AudioUrl { get; set; }
        public string? CoverImageUrl { get; set; }

        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = null!;

        public DateTime PlayedAt { get; set; }
    }
}
