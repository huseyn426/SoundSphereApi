using SoundSphereApi.DTOs.Social;
using SoundSphereApi.Models.Identity;
using SoundSphereApi.Models.Music;
using SoundSphereApi.Models.Social;
using SoundSphereApi.Repositories.Interfaces;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Services.Implementations
{
    public class SocialService : ISocialService
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Like> _likeRepository;
        private readonly IRepository<Follow> _followRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Track> _trackRepository;
        private readonly IRepository<Artist> _artistRepository;
        private readonly IRepository<Album> _albumRepository;
        private readonly IRepository<Genre> _genreRepository;

        public SocialService(
            IRepository<Comment> commentRepository,
            IRepository<Like> likeRepository,
            IRepository<Follow> followRepository,
            IRepository<User> userRepository,
            IRepository<Track> trackRepository,
            IRepository<Artist> artistRepository,
            IRepository<Album> albumRepository,
            IRepository<Genre> genreRepository)
        {
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
            _followRepository = followRepository;
            _userRepository = userRepository;
            _trackRepository = trackRepository;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<CommentGetDto>> GetCommentsByTrackIdAsync(int trackId)
        {
            var comments = await _commentRepository.FindAllAsync(c => c.TrackId == trackId);
            var users = await _userRepository.GetAllAsync();

            return comments
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentGetDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    UserName = users.FirstOrDefault(u => u.Id == c.UserId)?.UserName ?? string.Empty,
                    TrackId = c.TrackId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                });
        }

        public async Task<string> AddCommentAsync(int userId, CommentCreateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return "User not found.";
            }

            var track = await _trackRepository.GetByIdAsync(dto.TrackId);
            if (track == null)
            {
                return "Track not found.";
            }

            var comment = new Comment
            {
                UserId = userId,
                TrackId = dto.TrackId,
                Content = dto.Content
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            return "Comment added successfully.";
        }

        public async Task<string> AddLikeAsync(int userId, LikeCreateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return "User not found.";
            }

            var track = await _trackRepository.GetByIdAsync(dto.TrackId);
            if (track == null)
            {
                return "Track not found.";
            }

            var existingLike = await _likeRepository.FindAsync(l =>
                l.UserId == userId && l.TrackId == dto.TrackId);

            if (existingLike != null)
            {
                return "Track already liked.";
            }

            var like = new Like
            {
                UserId = userId,
                TrackId = dto.TrackId
            };

            await _likeRepository.AddAsync(like);
            await _likeRepository.SaveChangesAsync();

            return "Track liked successfully.";
        }

        public async Task<string> RemoveLikeAsync(int userId, int trackId)
        {
            var like = await _likeRepository.FindAsync(l =>
                l.UserId == userId && l.TrackId == trackId);

            if (like == null)
            {
                return "Like not found.";
            }

            _likeRepository.Delete(like);
            await _likeRepository.SaveChangesAsync();

            return "Like removed successfully.";
        }

        public async Task<string> FollowUserAsync(int followerId, FollowCreateDto dto)
        {
            if (followerId == dto.FollowingId)
            {
                return "You cannot follow yourself.";
            }

            var follower = await _userRepository.GetByIdAsync(followerId);
            if (follower == null)
            {
                return "Follower user not found.";
            }

            var following = await _userRepository.GetByIdAsync(dto.FollowingId);
            if (following == null)
            {
                return "User to follow not found.";
            }

            var existingFollow = await _followRepository.FindAsync(f =>
                f.FollowerId == followerId && f.FollowingId == dto.FollowingId);

            if (existingFollow != null)
            {
                return "Already following this user.";
            }

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = dto.FollowingId
            };

            await _followRepository.AddAsync(follow);
            await _followRepository.SaveChangesAsync();

            return "User followed successfully.";
        }

        public async Task<string> UnfollowUserAsync(int followerId, int followingId)
        {
            var follow = await _followRepository.FindAsync(f =>
                f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null)
            {
                return "Follow relationship not found.";
            }

            _followRepository.Delete(follow);
            await _followRepository.SaveChangesAsync();

            return "User unfollowed successfully.";
        }

        public async Task<IEnumerable<FollowGetDto>> GetFollowersAsync(int userId)
        {
            var follows = await _followRepository.FindAllAsync(f => f.FollowingId == userId);
            var users = await _userRepository.GetAllAsync();

            return follows.Select(f => new FollowGetDto
            {
                Id = f.Id,
                FollowerId = f.FollowerId,
                FollowerUserName = users.FirstOrDefault(u => u.Id == f.FollowerId)?.UserName ?? string.Empty,
                FollowingId = f.FollowingId,
                FollowingUserName = users.FirstOrDefault(u => u.Id == f.FollowingId)?.UserName ?? string.Empty
            });
        }

        public async Task<IEnumerable<FollowGetDto>> GetFollowingAsync(int userId)
        {
            var follows = await _followRepository.FindAllAsync(f => f.FollowerId == userId);
            var users = await _userRepository.GetAllAsync();

            return follows.Select(f => new FollowGetDto
            {
                Id = f.Id,
                FollowerId = f.FollowerId,
                FollowerUserName = users.FirstOrDefault(u => u.Id == f.FollowerId)?.UserName ?? string.Empty,
                FollowingId = f.FollowingId,
                FollowingUserName = users.FirstOrDefault(u => u.Id == f.FollowingId)?.UserName ?? string.Empty
            });
        }
        public async Task<IEnumerable<SoundSphereApi.DTOs.Music.TrackGetDto>> GetLikedTracksAsync(int userId)
        {
            var likes = await _likeRepository.FindAllAsync(l => l.UserId == userId);
            var tracks = await _trackRepository.GetAllAsync();
            var artists = await _artistRepository.GetAllAsync();
            var albums = await _albumRepository.GetAllAsync();
            var genres = await _genreRepository.GetAllAsync();

            var likedTracks = from l in likes
                               join t in tracks on l.TrackId equals t.Id
                               select new SoundSphereApi.DTOs.Music.TrackGetDto
                               {
                                   Id = t.Id,
                                   Title = t.Title,
                                   AudioUrl = t.AudioUrl,
                                   CoverImageUrl = t.CoverImageUrl,
                                   Duration = t.Duration,
                                   PlayCount = t.PlayCount,
                                   ArtistId = t.ArtistId,
                                   ArtistName = artists.FirstOrDefault(a => a.Id == t.ArtistId)?.Name ?? string.Empty,
                                   AlbumId = t.AlbumId,
                                   AlbumTitle = albums.FirstOrDefault(a => a.Id == t.AlbumId)?.Title ?? string.Empty,
                                   GenreId = t.GenreId,
                                   GenreName = genres.FirstOrDefault(g => g.Id == t.GenreId)?.Name ?? string.Empty
                               };

            return likedTracks.ToList();
        }
    }
}
