using System.Collections.Generic;

namespace SoundSphereApi.Models.Payment
{
    public class SubscriptionPlan : BaseEntity
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public string? Description { get; set; }

        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}
