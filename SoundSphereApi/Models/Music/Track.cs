using System.Collections.Generic;
using SoundSphereApi.Models.Analytics;
using SoundSphereApi.Models.Playlist;
using SoundSphereApi.Models.Social;

namespace SoundSphereApi.Models.Music
{
    public class Track : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? AudioUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public int PlayCount { get; set; } = 0;

        public int ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;

        public int AlbumId { get; set; }
        public Album Album { get; set; } = null!;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();
        public ICollection<ListeningHistory> ListeningHistories { get; set; } = new List<ListeningHistory>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
