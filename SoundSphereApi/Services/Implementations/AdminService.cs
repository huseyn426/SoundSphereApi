using SoundSphereApi.DTOs.Admin;
using SoundSphereApi.DTOs.Payment;
using SoundSphereApi.Models.Identity;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<SoundSphereApi.Models.Payment.Payment> _paymentRepository;
        private readonly IRepository<SoundSphereApi.Models.Payment.UserSubscription> _subscriptionRepository;
        private readonly IRepository<SoundSphereApi.Models.Payment.SubscriptionPlan> _planRepository;

        public AdminService(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<SoundSphereApi.Models.Payment.Payment> paymentRepository,
            IRepository<SoundSphereApi.Models.Payment.UserSubscription> subscriptionRepository,
            IRepository<SoundSphereApi.Models.Payment.SubscriptionPlan> planRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _paymentRepository = paymentRepository;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
        }

        public async Task<IEnumerable<UserGetDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var roles = await _roleRepository.GetAllAsync();

            return users.Select(u => new UserGetDto
            {
                Id = u.Id,
                FullName = u.FullName,
                UserName = u.UserName,
                Email = u.Email,
                IsActive = u.IsActive,
                RoleId = u.RoleId,
                RoleName = roles.FirstOrDefault(r => r.Id == u.RoleId)?.Name ?? string.Empty
            });
        }

        public async Task<string> UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return "User not found.";
            }

            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null)
            {
                return "Role not found.";
            }

            user.RoleId = dto.RoleId;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return "User role updated successfully.";
        }

        public async Task<string> UpdateUserStatusAsync(UpdateUserStatusDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return "User not found.";
            }

            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return "User status updated successfully.";
        }

        public async Task<IEnumerable<PaymentGetDto>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            return payments
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentGetDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    TransactionId = p.TransactionId,
                    PaymentDate = p.PaymentDate
                });
        }

        public async Task<IEnumerable<UserSubscriptionGetDto>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _subscriptionRepository.GetAllAsync();
            var plans = await _planRepository.GetAllAsync();

            return subscriptions
                .OrderByDescending(s => s.StartDate)
                .Select(s =>
                {
                    var plan = plans.FirstOrDefault(p => p.Id == s.SubscriptionPlanId);

                    return new UserSubscriptionGetDto
                    {
                        Id = s.Id,
                        UserId = s.UserId,
                        SubscriptionPlanId = s.SubscriptionPlanId,
                        PlanName = plan?.Name ?? string.Empty,
                        Price = plan?.Price ?? 0,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        IsActive = s.IsActive
                    };
                });
        }
    }
}
