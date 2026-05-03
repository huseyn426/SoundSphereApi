namespace SoundSphereApi.DTOs.Payment
{
    public class UserSubscriptionGetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
