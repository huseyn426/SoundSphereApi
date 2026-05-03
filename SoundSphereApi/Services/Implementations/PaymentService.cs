using SoundSphereApi.DTOs.Payment;
using SoundSphereApi.Models.Identity;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<SoundSphereApi.Models.Payment.SubscriptionPlan> _planRepository;
        private readonly IRepository<SoundSphereApi.Models.Payment.UserSubscription> _userSubscriptionRepository;
        private readonly IRepository<SoundSphereApi.Models.Payment.Payment> _paymentRepository;
        private readonly IRepository<User> _userRepository;

        public PaymentService(
            IRepository<SoundSphereApi.Models.Payment.SubscriptionPlan> planRepository,
            IRepository<SoundSphereApi.Models.Payment.UserSubscription> userSubscriptionRepository,
            IRepository<SoundSphereApi.Models.Payment.Payment> paymentRepository,
            IRepository<User> userRepository)
        {
            _planRepository = planRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<SubscriptionPlanGetDto>> GetAllPlansAsync()
        {
            var plans = await _planRepository.GetAllAsync();

            return plans.Select(p => new SubscriptionPlanGetDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationInDays = p.DurationInDays,
                Description = p.Description
            });
        }

        public async Task<SubscriptionPlanGetDto?> GetPlanByIdAsync(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);

            if (plan == null)
            {
                return null;
            }

            return new SubscriptionPlanGetDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                Description = plan.Description
            };
        }

        public async Task CreatePlanAsync(SubscriptionPlanCreateDto dto)
        {
            var plan = new SoundSphereApi.Models.Payment.SubscriptionPlan
            {
                Name = dto.Name,
                Price = dto.Price,
                DurationInDays = dto.DurationInDays,
                Description = dto.Description
            };

            await _planRepository.AddAsync(plan);
            await _planRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserSubscriptionGetDto>> GetUserSubscriptionsAsync(int userId)
        {
            var subscriptions = await _userSubscriptionRepository.FindAllAsync(us => us.UserId == userId);
            var plans = await _planRepository.GetAllAsync();

            return subscriptions.Select(us =>
            {
                var plan = plans.FirstOrDefault(p => p.Id == us.SubscriptionPlanId);

                return new UserSubscriptionGetDto
                {
                    Id = us.Id,
                    UserId = us.UserId,
                    SubscriptionPlanId = us.SubscriptionPlanId,
                    PlanName = plan?.Name ?? string.Empty,
                    Price = plan?.Price ?? 0,
                    StartDate = us.StartDate,
                    EndDate = us.EndDate,
                    IsActive = us.IsActive
                };
            });
        }

        public async Task<string> CreateUserSubscriptionAsync(int userId, UserSubscriptionCreateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return "User not found.";
            }

            var plan = await _planRepository.GetByIdAsync(dto.SubscriptionPlanId);
            if (plan == null)
            {
                return "Subscription plan not found.";
            }

            var activeSubscriptions = await _userSubscriptionRepository.FindAllAsync(us =>
                us.UserId == userId && us.IsActive);

            foreach (var item in activeSubscriptions)
            {
                item.IsActive = false;
                item.UpdatedAt = DateTime.UtcNow;
                _userSubscriptionRepository.Update(item);
            }

            var subscription = new SoundSphereApi.Models.Payment.UserSubscription
            {
                UserId = userId,
                SubscriptionPlanId = dto.SubscriptionPlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationInDays),
                IsActive = true
            };

            await _userSubscriptionRepository.AddAsync(subscription);
            await _userSubscriptionRepository.SaveChangesAsync();

            return "Subscription created successfully.";
        }

        public async Task<IEnumerable<PaymentGetDto>> GetUserPaymentsAsync(int userId)
        {
            var payments = await _paymentRepository.FindAllAsync(p => p.UserId == userId);

            return payments.Select(p => new PaymentGetDto
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

        public async Task CreatePaymentAsync(int userId, PaymentCreateDto dto)
        {
            var payment = new SoundSphereApi.Models.Payment.Payment
            {
                UserId = userId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status,
                TransactionId = dto.TransactionId,
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();
        }
    }
}
