using System.Security.Claims;

namespace SoundSphereApi.Helpers
{
    public static class ClaimHelper
    {
        public static int? GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
