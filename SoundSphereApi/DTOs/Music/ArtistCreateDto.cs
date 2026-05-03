namespace SoundSphereApi.DTOs.Music
{
    public class ArtistCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
    }
}
