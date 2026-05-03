using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Analytics;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.Services.Interfaces;
using SoundSphereApi.Helpers;


namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ListeningHistoryController : ControllerBase
    {
        private readonly IListeningHistoryService _listeningHistoryService;

        public ListeningHistoryController(IListeningHistoryService listeningHistoryService)
        {
            _listeningHistoryService = listeningHistoryService;
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserListeningHistory(int userId)
        {
            var result = await _listeningHistoryService.GetUserListeningHistoryAsync(userId);
            return Ok(ApiResponse<IEnumerable<ListeningHistoryGetDto>>.Ok(result));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyListeningHistory()
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var result = await _listeningHistoryService.GetUserListeningHistoryAsync(userId.Value);
            return Ok(ApiResponse<IEnumerable<ListeningHistoryGetDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> AddListeningHistory([FromBody] ListeningHistoryCreateDto dto)
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

            var result = await _listeningHistoryService.AddListeningHistoryAsync(userId.Value, dto);

            if (result != "Listening history added successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

    }
}
