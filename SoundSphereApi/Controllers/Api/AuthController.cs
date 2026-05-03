using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Auth;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.Helpers;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result != "User registered successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(loginDto);

            if (response == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid email or password."));
            }

            return Ok(ApiResponse<AuthResponseDto>.Ok(response));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = ClaimHelper.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

            var profile = await _authService.GetUserProfileAsync(userId.Value);

            if (profile == null)
            {
                return NotFound(ApiResponse.Fail("User not found."));
            }

            return Ok(ApiResponse<UserProfileDto>.Ok(profile));
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateProfileDto dto)
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

            var result = await _authService.UpdateUserProfileAsync(userId.Value, dto);

            if (result != "Profile updated successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }
    }
}
