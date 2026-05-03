namespace SoundSphereApi.DTOs.Music
{
    public class ArtistGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
    }
}
