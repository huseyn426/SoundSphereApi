using SoundSphereApi.DTOs.Social;

namespace SoundSphereApi.Services.Interfaces
{
    public interface ISocialService
    {
        Task<IEnumerable<CommentGetDto>> GetCommentsByTrackIdAsync(int trackId);
        Task<string> AddCommentAsync(int userId, CommentCreateDto dto);
        Task<string> AddLikeAsync(int userId, LikeCreateDto dto);
        Task<string> RemoveLikeAsync(int userId, int trackId);
        Task<string> FollowUserAsync(int followerId, FollowCreateDto dto);
        Task<string> UnfollowUserAsync(int followerId, int followingId);
        Task<IEnumerable<FollowGetDto>> GetFollowersAsync(int userId);
        Task<IEnumerable<FollowGetDto>> GetFollowingAsync(int userId);
        Task<IEnumerable<SoundSphereApi.DTOs.Music.TrackGetDto>> GetLikedTracksAsync(int userId);
    }
}
