using SoundSphereApi.Models.Identity;

namespace SoundSphereApi.Models.Payment
{
    public class Payment : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? TransactionId { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
