namespace SoundSphereApi.DTOs.Payment
{
    public class PaymentGetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
