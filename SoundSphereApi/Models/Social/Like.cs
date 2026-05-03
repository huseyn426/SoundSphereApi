using SoundSphereApi.Models.Identity;
using SoundSphereApi.Models.Music;

namespace SoundSphereApi.Models.Social
{
    public class Like : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int TrackId { get; set; }
        public Track Track { get; set; } = null!;
    }
}
