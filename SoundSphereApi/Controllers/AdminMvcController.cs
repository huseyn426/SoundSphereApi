using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.Data.Context;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers
{
    [Route("AdminPanel/[action]")]
    public class AdminPanelController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly AppDbContext _context;
        private readonly ITrackService _trackService;
        private readonly IArtistService _artistService;
        private readonly IAlbumService _albumService;
        private readonly IGenreService _genreService;

        public AdminPanelController(
            IAdminService adminService, 
            AppDbContext context,
            ITrackService trackService,
            IArtistService artistService,
            IAlbumService albumService,
            IGenreService genreService)
        {
            _adminService = adminService;
            _context = context;
            _trackService = trackService;
            _artistService = artistService;
            _albumService = albumService;
            _genreService = genreService;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var users = await _adminService.GetAllUsersAsync();

            ViewBag.UserCount = _context.Users.Count();
            ViewBag.TrackCount = _context.Tracks.Count();
            ViewBag.PlaylistCount = _context.Playlists.Count();
            ViewBag.SubCount = _context.UserSubscriptions.Count();
            ViewBag.Users = users;
            ViewBag.Roles = _context.Roles.ToList();

            return View("~/Views/Admin/Dashboard.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var users = await _adminService.GetAllUsersAsync();
            return View("~/Views/Admin/Users.cshtml", users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int userId, bool isActive)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _adminService.UpdateUserStatusAsync(
                new DTOs.Admin.UpdateUserStatusDto { UserId = userId, IsActive = isActive });

            TempData["AdminSuccess"] = result;
            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatusAjax([FromBody] DTOs.Admin.UpdateUserStatusDto request)
        {
            if (!IsAdmin())
            {
                return Unauthorized(new { success = false, message = "Access denied" });
            }

            var result = await _adminService.UpdateUserStatusAsync(request);
            
            if (result == "User status updated successfully.")
            {
                return Ok(new { success = true, message = result, isActive = request.IsActive });
            }
            
            return BadRequest(new { success = false, message = result });
        }
        [HttpGet]
        public async Task<IActionResult> Tracks(string? search)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var tracks = await _trackService.GetAllTracksAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var query = search.Trim().ToLower();
                tracks = tracks.Where(t => t.Title.ToLower().Contains(query) || t.ArtistName.ToLower().Contains(query));
            }

            ViewBag.Search = search;
            ViewBag.Artists = await _artistService.GetAllArtistsAsync();
            ViewBag.Genres = await _genreService.GetAllGenresAsync();
            ViewBag.Albums = await _albumService.GetAllAlbumsAsync();

            return View("~/Views/Admin/Tracks.cshtml", tracks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrack([FromBody] DTOs.Music.TrackCreateDto dto)
        {
            if (!IsAdmin()) return Unauthorized(new { success = false, message = "Access denied" });
            
            await _trackService.CreateTrackAsync(dto);
            return Ok(new { success = true, message = "Track created successfully." });
        }

        [HttpPut]
        public async Task<IActionResult> EditTrack(int id, [FromBody] DTOs.Music.TrackUpdateDto dto)
        {
            if (!IsAdmin()) return Unauthorized(new { success = false, message = "Access denied" });

            dto.Id = id;
            await _trackService.UpdateTrackAsync(dto);
            return Ok(new { success = true, message = "Track updated successfully." });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTrack(int id)
        {
            if (!IsAdmin()) return Unauthorized(new { success = false, message = "Access denied" });

            await _trackService.DeleteTrackAsync(id);
            return Ok(new { success = true, message = "Track deleted successfully." });
        }

        // ================= CATALOG MANAGEMENT =================

        [HttpGet]
        public async Task<IActionResult> Catalog()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.Genres = await _genreService.GetAllGenresAsync();
            ViewBag.Artists = await _artistService.GetAllArtistsAsync();
            ViewBag.Albums = await _albumService.GetAllAlbumsAsync();

            return View("~/Views/Admin/Catalog.cshtml");
        }

        // --- GENRES ---
        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] DTOs.Music.GenreCreateDto dto)
        {
            if (!IsAdmin()) return Unauthorized();
            await _genreService.CreateGenreAsync(dto);
            return Ok(new { success = true });
        }

        [HttpPut]
        public async Task<IActionResult> EditGenre(int id, [FromBody] DTOs.Music.GenreUpdateDto dto)
        {
            if (!IsAdmin()) return Unauthorized();
            dto.Id = id;
            var res = await _genreService.UpdateGenreAsync(dto);
            return res.Contains("success") ? Ok(new { success = true }) : BadRequest(new { message = res });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            var res = await _genreService.DeleteGenreAsync(id);
            return res.Contains("success") ? Ok(new { success = true }) : BadRequest(new { message = res });
        }

        // --- ARTISTS ---
        [HttpPost]
        public async Task<IActionResult> CreateArtist([FromBody] DTOs.Music.ArtistCreateDto dto)
        {
            if (!IsAdmin()) return Unauthorized();
            await _artistService.CreateArtistAsync(dto);
            return Ok(new { success = true });
        }

        [HttpPut]
        public async Task<IActionResult> EditArtist(int id, [FromBody] DTOs.Music.ArtistUpdateDto dto)
        {
            if (!IsAdmin()) return Unauthorized();
            dto.Id = id;
            var res = await _artistService.UpdateArtistAsync(dto);
            return res.Contains("success") ? Ok(new { success = true }) : BadRequest(new { message = res });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            var res = await _artistService.DeleteArtistAsync(id);
            return res.Contains("success") ? Ok(new { success = true }) : BadRequest(new { message = res });
        }

        // --- ALBUMS ---
        [HttpPost]
        public async Task<IActionResult> CreateAlbum([FromBody] DTOs.Music.AlbumCreateDto dto)
        {
            if (!IsAdmin()) return Unauthorized();
            var res = await _albumService.CreateAlbumAsync(dto);
            return res.Contains("success") ? Ok(new { success = true }) : BadRequest(new { message = res });
        }

        [HttpPut]
        public async Task<IActionResult> EditAlbum(int id, [FromBody] DTOs.Music.AlbumUpdateDto dto)
        {
            if (!IsAdmin()) return Unauthorized();
            dto.Id = id;
            var res = await _albumService.UpdateAlbumAsync(dto);
            return res.Contains("success") ? Ok(new { success = true }) : BadRequest(new { message = res });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            var res = await _albumService.DeleteAlbumAsync(id);
            return res.Contains("success") ? Ok(new { success = true }) : BadRequest(new { message = res });
        }
    }
}
