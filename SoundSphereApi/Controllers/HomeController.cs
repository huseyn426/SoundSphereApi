using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly IPlaylistService _playlistService;
        private readonly IPaymentService _paymentService;
        private readonly IArtistService _artistService;

        public HomeController(ITrackService trackService, IPlaylistService playlistService, IPaymentService paymentService, IArtistService artistService)
        {
            _trackService = trackService;
            _playlistService = playlistService;
            _paymentService = paymentService;
            _artistService = artistService;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                // Для авторизованного пользователя
                ViewBag.IsLoggedIn = true;
                ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "User";
                ViewBag.UserRole = HttpContext.Session.GetString("UserRole") ?? "User";

                try
                {
                    ViewBag.RecentTracks = await _trackService.GetRecentlyPlayedAsync(userId.Value, 8);
                }
                catch { ViewBag.RecentTracks = Enumerable.Empty<SoundSphereApi.DTOs.Music.TrackGetDto>(); }

                try
                {
                    var playlists = await _playlistService.GetAllPlaylistsAsync();
                    ViewBag.MyPlaylists = playlists.Where(p => p.UserId == userId.Value).Take(4);
                }
                catch { ViewBag.MyPlaylists = Enumerable.Empty<SoundSphereApi.DTOs.Playlist.PlaylistGetDto>(); }

                try
                {
                    ViewBag.PopularTracks = await _trackService.GetPopularTracksAsync(6);
                    ViewBag.PopularArtists = (await _artistService.GetAllArtistsAsync()).Take(8);
                }
                catch { 
                    ViewBag.PopularTracks = Enumerable.Empty<SoundSphereApi.DTOs.Music.TrackGetDto>();
                    ViewBag.PopularArtists = Enumerable.Empty<SoundSphereApi.DTOs.Music.ArtistGetDto>();
                }
            }
            else
            {
                // Для лендинга
                ViewBag.IsLoggedIn = false;
                ViewBag.Plans = await _paymentService.GetAllPlansAsync();
            }

            return View();
        }
    }
}
