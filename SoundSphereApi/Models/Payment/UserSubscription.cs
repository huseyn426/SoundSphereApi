using SoundSphereApi.Models.Identity;

namespace SoundSphereApi.Models.Payment
{
    public class UserSubscription : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int SubscriptionPlanId { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
