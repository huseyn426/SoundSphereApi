using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.DTOs.Social;
using SoundSphereApi.Services.Interfaces;
using SoundSphereApi.Helpers;


namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SocialController : ControllerBase
    {
        private readonly ISocialService _socialService;

        public SocialController(ISocialService socialService)
        {
            _socialService = socialService;
        }

        [HttpGet("comments/{trackId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsByTrackId(int trackId)
        {
            var comments = await _socialService.GetCommentsByTrackIdAsync(trackId);
            return Ok(ApiResponse<IEnumerable<CommentGetDto>>.Ok(comments));
        }

        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateDto dto)
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

            var result = await _socialService.AddCommentAsync(userId.Value, dto);

            if (result != "Comment added successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpPost("likes")]
        public async Task<IActionResult> AddLike([FromBody] LikeCreateDto dto)
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

            var result = await _socialService.AddLikeAsync(userId.Value, dto);

            if (result != "Track liked successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpDelete("likes")]
        public async Task<IActionResult> RemoveLike([FromQuery] int trackId)
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var result = await _socialService.RemoveLikeAsync(userId.Value, trackId);

            if (result != "Like removed successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpPost("follow")]
        public async Task<IActionResult> FollowUser([FromBody] FollowCreateDto dto)
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

            var result = await _socialService.FollowUserAsync(userId.Value, dto);

            if (result != "User followed successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpDelete("unfollow")]
        public async Task<IActionResult> UnfollowUser([FromQuery] int followingId)
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var result = await _socialService.UnfollowUserAsync(userId.Value, followingId);

            if (result != "User unfollowed successfully.")
            {
                return NotFound(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpGet("followers/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFollowers(int userId)
        {
            var result = await _socialService.GetFollowersAsync(userId);
            return Ok(ApiResponse<IEnumerable<FollowGetDto>>.Ok(result));
        }

        [HttpGet("following/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFollowing(int userId)
        {
            var result = await _socialService.GetFollowingAsync(userId);
            return Ok(ApiResponse<IEnumerable<FollowGetDto>>.Ok(result));
        }
    }
}
