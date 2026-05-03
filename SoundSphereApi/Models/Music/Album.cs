using SoundSphereApi.Models.Music;
using System.Collections.Generic;

namespace SoundSphereApi.Models.Music
{
    public class Album : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public DateTime ReleaseDate { get; set; }

        public int ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;

        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
