using System.Collections.Generic;
using SoundSphereApi.Models.Identity;

namespace SoundSphereApi.Models.Playlist
{
    public class PlaylistEntity : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;
        public string? CoverImageUrl { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();
    }
}
