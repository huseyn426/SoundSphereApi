using Microsoft.AspNetCore.Mvc;

namespace SoundSphereApi.Controllers
{
    public class LanguageController : Controller
    {
        [HttpGet]
        public IActionResult Set(string culture = "en", string? returnUrl = null)
        {
            if (culture != "en" && culture != "ru")
            {
                culture = "en";
            }

            HttpContext.Session.SetString("lang", culture);

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
