using SoundSphereApi.Models.Music;
using System.Collections.Generic;

namespace SoundSphereApi.Models.Music
{
    public class Genre : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
