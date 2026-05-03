using SoundSphereApi.DTOs.Admin;
using SoundSphereApi.DTOs.Payment;

namespace SoundSphereApi.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserGetDto>> GetAllUsersAsync();
        Task<string> UpdateUserRoleAsync(UpdateUserRoleDto dto);
        Task<string> UpdateUserStatusAsync(UpdateUserStatusDto dto);
        Task<IEnumerable<PaymentGetDto>> GetAllPaymentsAsync();
        Task<IEnumerable<UserSubscriptionGetDto>> GetAllSubscriptionsAsync();
    }
}
