using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundSphereApi.DTOs.Admin;
using SoundSphereApi.DTOs.Common;
using SoundSphereApi.DTOs.Payment;
using SoundSphereApi.Services.Interfaces;

namespace SoundSphereApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserGetDto>>.Ok(users));
        }

        [HttpPut("users/role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminService.UpdateUserRoleAsync(dto);

            if (result != "User role updated successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpPut("users/status")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminService.UpdateUserStatusAsync(dto);

            if (result != "User status updated successfully.")
            {
                return BadRequest(ApiResponse.Fail(result));
            }

            return Ok(ApiResponse.Ok(result));
        }

        [HttpPut("users/{id}/block")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var result = await _adminService.UpdateUserStatusAsync(new UpdateUserStatusDto { UserId = id, IsActive = false });
            return result == "User status updated successfully." 
                ? Ok(ApiResponse.Ok("User blocked")) 
                : BadRequest(ApiResponse.Fail(result));
        }

        [HttpPut("users/{id}/unblock")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var result = await _adminService.UpdateUserStatusAsync(new UpdateUserStatusDto { UserId = id, IsActive = true });
            return result == "User status updated successfully." 
                ? Ok(ApiResponse.Ok("User unblocked")) 
                : BadRequest(ApiResponse.Fail(result));
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _adminService.GetAllPaymentsAsync();
            return Ok(ApiResponse<IEnumerable<PaymentGetDto>>.Ok(payments));
        }

        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var subscriptions = await _adminService.GetAllSubscriptionsAsync();
            return Ok(ApiResponse<IEnumerable<UserSubscriptionGetDto>>.Ok(subscriptions));
        }
    }
}
