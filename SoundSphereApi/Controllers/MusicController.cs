using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Social;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers
{
    public class MusicController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly ISocialService _socialService;
        private readonly IListeningHistoryService _historyService;
        private readonly IArtistService _artistService;
        private readonly IPlaylistService _playlistService;
        private readonly IAlbumService _albumService;
        private readonly IGenreService _genreService;

        public MusicController(
            ITrackService trackService, 
            ISocialService socialService, 
            IListeningHistoryService historyService,
            IArtistService artistService,
            IPlaylistService playlistService,
            IAlbumService albumService,
            IGenreService genreService)
        {
            _trackService = trackService;
            _socialService = socialService;
            _historyService = historyService;
            _artistService = artistService;
            _playlistService = playlistService;
            _albumService = albumService;
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> Tracks(string? search)
        {
            var tracks = await _trackService.GetAllTracksAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var query = search.Trim().ToLower();

                tracks = tracks.Where(x =>
                    x.Title.ToLower().Contains(query) ||
                    x.ArtistName.ToLower().Contains(query) ||
                    x.AlbumTitle.ToLower().Contains(query) ||
                    x.GenreName.ToLower().Contains(query));
            }

            ViewBag.Search = search;
            return View(tracks);
        }

        [HttpGet]
        public async Task<IActionResult> SearchAjax(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Json(new { tracks = new object[]{}, artists = new object[]{}, playlists = new object[]{} });

            query = query.Trim().ToLower();
            
            // Search Tracks
            var allTracks = await _trackService.GetAllTracksAsync();
            var tracks = allTracks
                .Where(t => t.Title.ToLower().Contains(query) || t.ArtistName.ToLower().Contains(query))
                .Take(4)
                .Select(t => new {
                    id = t.Id,
                    title = t.Title,
                    artistName = t.ArtistName,
                    coverImageUrl = t.CoverImageUrl,
                    audioUrl = t.AudioUrl
                });

            // Search Artists
            var allArtists = await _artistService.GetAllArtistsAsync();
            var artists = allArtists
                .Where(a => a.Name.ToLower().Contains(query))
                .Take(3)
                .Select(a => new {
                    id = a.Id,
                    name = a.Name,
                    imageUrl = a.ImageUrl
                });

            // Search Playlists (only public ones or own)
            var userId = HttpContext.Session.GetInt32("UserId");
            var allPlaylists = await _playlistService.GetAllPlaylistsAsync();
            var playlists = allPlaylists
                .Where(p => p.IsPublic || p.UserId == userId)
                .Where(p => p.Name.ToLower().Contains(query))
                .Take(3)
                .Select(p => new {
                    id = p.Id,
                    name = p.Name
                });

            return Json(new { tracks, artists, playlists });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var track = await _trackService.GetTrackByIdAsync(id);
            if (track == null)
            {
                return NotFound();
            }

            var comments = await _socialService.GetCommentsByTrackIdAsync(id);
            
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                // In a real scenario, we might have a CheckLikeAsync, 
                // but since we don't have it directly exposed without fetching track likes,
                // we assume UI handles state via AddLike/RemoveLike. 
                // For now, we will pass a placeholder.
                ViewBag.CurrentUserId = userId.Value;
            }

            ViewBag.Comments = comments;
            return View(track);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int trackId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var dto = new CommentCreateDto { TrackId = trackId, Content = content };
                await _socialService.AddCommentAsync(userId.Value, dto);
            }

            return RedirectToAction("Details", new { id = trackId });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int trackId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Simplified: we just try to add a like.
            var dto = new LikeCreateDto { TrackId = trackId };
            var result = await _socialService.AddLikeAsync(userId.Value, dto);

            if (result != "Track liked successfully.")
            {
                // If it already exists, maybe remove it
                await _socialService.RemoveLikeAsync(userId.Value, trackId);
            }

            return RedirectToAction("Details", new { id = trackId });
        }
        [HttpPost]
        public async Task<IActionResult> LogPlay([FromBody] int trackId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null && trackId > 0)
            {
                var dto = new SoundSphereApi.DTOs.Analytics.ListeningHistoryCreateDto { TrackId = trackId };
                await _historyService.AddListeningHistoryAsync(userId.Value, dto);
                return Ok();
            }
            return Unauthorized();
        }
        [HttpGet]
        public async Task<IActionResult> Artist(int id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id);
            if (artist == null) return NotFound();
            
            var allTracks = await _trackService.GetAllTracksAsync();
            var artistTracks = allTracks.Where(t => t.ArtistName == artist.Name).ToList();
            
            ViewBag.Tracks = artistTracks;
            return View(artist);
        }

        [HttpGet]
        public async Task<IActionResult> Album(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null) return NotFound();
            
            var allTracks = await _trackService.GetAllTracksAsync();
            var albumTracks = allTracks.Where(t => t.AlbumTitle == album.Title).ToList();
            
            ViewBag.Tracks = albumTracks;
            return View(album);
        }

        [HttpGet]
        public async Task<IActionResult> Liked()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var tracks = await _socialService.GetLikedTracksAsync(userId.Value);
            return View(tracks);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var history = await _historyService.GetUserListeningHistoryAsync(userId.Value);
            return View(history);
        }

        [HttpGet]
        public async Task<IActionResult> Explore()
        {
            ViewBag.Genres = await _genreService.GetAllGenresAsync();
            var playlists = await _playlistService.GetAllPlaylistsAsync();
            ViewBag.PublicPlaylists = playlists.Where(p => p.IsPublic).Take(8);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Charts()
        {
            var topTracks = await _trackService.GetPopularTracksAsync(20);
            return View(topTracks);
        }
    }
}
