using SoundSphereApi.Models.Identity;
using System.Collections.Generic;

namespace SoundSphereApi.Models.Identity
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
