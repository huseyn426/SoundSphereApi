using SoundSphereApi.DTOs.Payment;

namespace SoundSphereApi.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<SubscriptionPlanGetDto>> GetAllPlansAsync();
        Task<SubscriptionPlanGetDto?> GetPlanByIdAsync(int id);
        Task CreatePlanAsync(SubscriptionPlanCreateDto dto);

        Task<IEnumerable<UserSubscriptionGetDto>> GetUserSubscriptionsAsync(int userId);
        Task<string> CreateUserSubscriptionAsync(int userId, UserSubscriptionCreateDto dto);

        Task<IEnumerable<PaymentGetDto>> GetUserPaymentsAsync(int userId);
        Task CreatePaymentAsync(int userId, PaymentCreateDto dto);
    }
}
