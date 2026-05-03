using SoundSphereApi.Models.Music;
using System.Collections.Generic;

namespace SoundSphereApi.Models.Music
{
    public class Artist : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<Album> Albums { get; set; } = new List<Album>();
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
