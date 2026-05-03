using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly IPaymentService _paymentService;

        public SubscriptionController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> Plans()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var plans = await _paymentService.GetAllPlansAsync();

            // Get current plan
            var subs = await _paymentService.GetUserSubscriptionsAsync(userId.Value);
            var activeSub = subs.OrderByDescending(s => s.EndDate).FirstOrDefault();
            ViewBag.CurrentPlan = activeSub?.PlanName;

            return View(plans);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int planId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _paymentService.CreateUserSubscriptionAsync(userId.Value,
                new DTOs.Payment.UserSubscriptionCreateDto { SubscriptionPlanId = planId });

            if (result == "Subscription created successfully.")
            {
                TempData["SubSuccess"] = "Subscription activated! Enjoy premium features.";
            }
            else
            {
                TempData["SubError"] = result;
            }

            return RedirectToAction("Plans");
        }
    }
}
