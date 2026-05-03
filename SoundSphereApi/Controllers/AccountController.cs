using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Auth;
using SoundSphereApi.Services.Interfaces;
using SoundSphereApi.ViewModels.Account;

namespace SoundSphereApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IPlaylistService _playlistService;
        private readonly ITrackService _trackService;

        public AccountController(
            IAuthService authService,
            IPlaylistService playlistService,
            ITrackService trackService)
        {
            _authService = authService;
            _playlistService = playlistService;
            _trackService = trackService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId") != null) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registerDto = new RegisterDto
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password
            };

            var result = await _authService.RegisterAsync(registerDto);

            if (result != "User registered successfully.")
            {
                ModelState.AddModelError(string.Empty, result);
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration completed successfully.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginDto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var response = await _authService.LoginAsync(loginDto);

            if (response == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", response.UserId);
            HttpContext.Session.SetString("JWToken", response.Token);
            HttpContext.Session.SetString("UserName", response.UserName);
            HttpContext.Session.SetString("UserEmail", response.Email);
            HttpContext.Session.SetString("UserRole", response.Role);

            TempData["SuccessMessage"] = "Login successful.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>Restores MVC session when a valid JWT exists in localStorage but the session cookie was lost.</summary>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SyncSession([FromBody] SyncSessionDto? dto)
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return Ok(new { ok = true });
            }

            var token = dto?.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { ok = false });
            }

            var response = await _authService.TryRestoreSessionFromTokenAsync(token);
            if (response == null)
            {
                return Unauthorized(new { ok = false });
            }

            HttpContext.Session.SetInt32("UserId", response.UserId);
            HttpContext.Session.SetString("JWToken", response.Token);
            HttpContext.Session.SetString("UserName", response.UserName);
            HttpContext.Session.SetString("UserEmail", response.Email);
            HttpContext.Session.SetString("UserRole", response.Role);

            return Ok(new { ok = true });
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var profile = await _authService.GetUserProfileAsync(userId.Value);

            var playlists = await _playlistService.GetAllPlaylistsAsync();
            var myPlaylists = playlists.Where(p => p.UserId == userId.Value);

            IEnumerable<DTOs.Music.TrackGetDto> recentTracks;
            try
            {
                recentTracks = await _trackService.GetRecentlyPlayedAsync(userId.Value, 8);
            }
            catch
            {
                recentTracks = Enumerable.Empty<DTOs.Music.TrackGetDto>();
            }

            ViewBag.FullName = profile?.FullName;
            ViewBag.ProfileImageUrl = profile?.ProfileImageUrl;
            ViewBag.Playlists = myPlaylists;
            ViewBag.RecentTracks = recentTracks;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string userName, string? profileImageUrl)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var dto = new UpdateProfileDto
            {
                FullName = fullName,
                UserName = userName,
                ProfileImageUrl = profileImageUrl
            };

            var result = await _authService.UpdateUserProfileAsync(userId.Value, dto);

            if (result == "Profile updated successfully.")
            {
                HttpContext.Session.SetString("UserName", userName);
                TempData["ProfileSuccess"] = result;
            }
            else
            {
                TempData["ProfileError"] = result;
            }

            return RedirectToAction("Profile");
        }
    }
}
