using SoundSphereApi.Models.Music;

namespace SoundSphereApi.Models.Playlist
{
    public class PlaylistTrack : BaseEntity
    {
        public int PlaylistEntityId { get; set; }
        public PlaylistEntity PlaylistEntity { get; set; } = null!;

        public int TrackId { get; set; }
        public Track Track { get; set; } = null!;
    }
}
