namespace SoundSphereApi.DTOs.Payment
{
    public class PaymentCreateDto
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? TransactionId { get; set; }
    }
}
