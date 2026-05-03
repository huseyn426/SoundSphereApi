using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.DTOs.Payment;
using SoundSphereApi.Helpers;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("plans")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _paymentService.GetAllPlansAsync();
            return Ok(ApiResponse<IEnumerable<SubscriptionPlanGetDto>>.Ok(plans));
        }

        [HttpGet("plans/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _paymentService.GetPlanByIdAsync(id);

            if (plan == null)
            {
                return NotFound(ApiResponse.Fail("Subscription plan not found."));
            }

            return Ok(ApiResponse<SubscriptionPlanGetDto>.Ok(plan));
        }

        [HttpPost("plans")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePlan([FromBody] SubscriptionPlanCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _paymentService.CreatePlanAsync(dto);
            return Ok(ApiResponse.Ok("Subscription plan created successfully."));
        }

        [HttpGet("subscriptions/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserSubscriptions(int userId)
        {
            var subscriptions = await _paymentService.GetUserSubscriptionsAsync(userId);
            return Ok(ApiResponse<IEnumerable<UserSubscriptionGetDto>>.Ok(subscriptions));
        }

        [HttpGet("my-subscriptions")]
        [Authorize]
        public async Task<IActionResult> GetMySubscriptions()
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var subscriptions = await _paymentService.GetUserSubscriptionsAsync(userId.Value);
            return Ok(ApiResponse<IEnumerable<UserSubscriptionGetDto>>.Ok(subscriptions));
        }

        [HttpPost("subscriptions")]
        [Authorize]
        public async Task<IActionResult> CreateUserSubscription([FromBody] UserSubscriptionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var result = await _paymentService.CreateUserSubscriptionAsync(userId.Value, dto);

            if (result != "Subscription created successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpGet("user-payments/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserPayments(int userId)
        {
            var payments = await _paymentService.GetUserPaymentsAsync(userId);
            return Ok(ApiResponse<IEnumerable<PaymentGetDto>>.Ok(payments));
        }

        [HttpGet("my-payments")]
        [Authorize]
        public async Task<IActionResult> GetMyPayments()
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var payments = await _paymentService.GetUserPaymentsAsync(userId.Value);
            return Ok(ApiResponse<IEnumerable<PaymentGetDto>>.Ok(payments));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            await _paymentService.CreatePaymentAsync(userId.Value, dto);
            return Ok(ApiResponse.Ok("Payment created successfully."));
        }
    }
}
