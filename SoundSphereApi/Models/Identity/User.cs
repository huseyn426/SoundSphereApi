using System.Collections.Generic;
using SoundSphereApi.Models.Analytics;
using SoundSphereApi.Models.Playlist;
using SoundSphereApi.Models.Social;

namespace SoundSphereApi.Models.Identity
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<PlaylistEntity> Playlists { get; set; } = new List<PlaylistEntity>();
        public ICollection<SoundSphereApi.Models.Payment.UserSubscription> UserSubscriptions { get; set; } =
            new List<SoundSphereApi.Models.Payment.UserSubscription>();
        public ICollection<SoundSphereApi.Models.Payment.Payment> Payments { get; set; } =
            new List<SoundSphereApi.Models.Payment.Payment>();
        public ICollection<ListeningHistory> ListeningHistories { get; set; } = new List<ListeningHistory>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
    }
}
