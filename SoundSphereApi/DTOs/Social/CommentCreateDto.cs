namespace SoundSphereApi.DTOs.Social
{
    public class CommentCreateDto
    {
        public int TrackId { get; set; }
        public string Content { get; set; } = null!;
    }
}
