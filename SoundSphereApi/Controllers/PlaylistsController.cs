using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Playlist;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var playlists = await _playlistService.GetAllPlaylistsAsync();
            var myPlaylists = playlists.Where(x => x.UserId == userId.Value);

            return View(myPlaylists);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PlaylistCreateDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _playlistService.CreatePlaylistAsync(userId.Value, dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddTrack(int trackId, int playlistId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _playlistService.AddTrackToPlaylistAsync(userId.Value, new AddTrackToPlaylistDto
            {
                PlaylistId = playlistId,
                TrackId = trackId
            });

            return RedirectToAction("Tracks", "Music");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var playlist = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            // Only owner can see private playlists
            if (!playlist.IsPublic && playlist.UserId != userId.Value)
            {
                return StatusCode(403);
            }

            var tracks = await _playlistService.GetTracksByPlaylistIdAsync(id);
            ViewBag.Playlist = playlist;
            ViewBag.IsOwner = playlist.UserId == userId.Value;

            return View(tracks);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTrack(int playlistId, int trackId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _playlistService.RemoveTrackFromPlaylistAsync(playlistId, trackId, userId.Value);
            return RedirectToAction("Details", new { id = playlistId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _playlistService.DeletePlaylistAsync(id, userId.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
