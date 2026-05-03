namespace SoundSphereApi.DTOs.Payment
{
    public class SubscriptionPlanGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public string? Description { get; set; }
    }
}
