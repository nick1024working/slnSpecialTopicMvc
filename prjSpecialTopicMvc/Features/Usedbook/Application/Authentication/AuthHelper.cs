using System.Security.Claims;
using prjSpecialTopicMvc.Features.Usedbook.Enums;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.Authentication
{
    public static class AuthHelper
    {
        public static Guid? GetUserId(ClaimsPrincipal user)
        {
            var userIdRaw = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdRaw, out Guid userId))
                return userId;

            return null;
        }

        public static Role? GetRole(ClaimsPrincipal user)
        {
            var roleRaw = user.FindFirst(ClaimTypes.Role)?.Value;
            if (Enum.TryParse<Role>(roleRaw, out var role))
                return role;
            return null;
        }
    }
}
