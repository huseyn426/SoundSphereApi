using SoundSphereApi.Models.Identity;
using SoundSphereApi.Models.Music;

namespace SoundSphereApi.Models.Analytics
{
    public class ListeningHistory : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int TrackId { get; set; }
        public Track Track { get; set; } = null!;

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}
