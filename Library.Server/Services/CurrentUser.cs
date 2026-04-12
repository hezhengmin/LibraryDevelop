using System.Security.Claims;

namespace Library.Server.Services
{
    /// <summary>
    /// 當前用戶資訊服務
    /// 用於在整個請求生命週期中訪問當前登入用戶的資訊
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// 用戶 ID
        /// </summary>
        Guid UserId { get; }

        /// <summary>
        /// 用戶帳號
        /// </summary>
        string Account { get; }

        /// <summary>
        /// 用戶名稱
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 用戶 Email
        /// </summary>
        string Email { get; }

        /// <summary>
        /// 角色 ID（主要角色）
        /// </summary>
        string RoleId { get; }

        /// <summary>
        /// 所有角色 ID 列表
        /// </summary>
        IEnumerable<string> RoleIds { get; }

        /// <summary>
        /// 是否已認證
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// 取得所有 Claims
        /// </summary>
        IEnumerable<Claim> Claims { get; }

        /// <summary>
        /// 檢查用戶是否擁有指定角色
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>是否擁有該角色</returns>
        bool HasRole(string roleId);

        /// <summary>
        /// 檢查用戶是否擁有任一指定角色
        /// </summary>
        /// <param name="roleIds">角色 ID 列表</param>
        /// <returns>是否擁有任一角色</returns>
        bool HasAnyRole(params string[] roleIds);

        /// <summary>
        /// 檢查用戶是否擁有所有指定角色
        /// </summary>
        /// <param name="roleIds">角色 ID 列表</param>
        /// <returns>是否擁有所有角色</returns>
        bool HasAllRoles(params string[] roleIds);
    }

    /// <summary>
    /// 當前用戶資訊實作
    /// </summary>
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid UserId
        {
            get
            {
                var subClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User?.FindFirst("sub")?.Value;

                return string.IsNullOrEmpty(subClaim) ? Guid.Empty : Guid.Parse(subClaim);
            }
        }

        public string Account =>
            User?.FindFirst(ClaimTypes.Name)?.Value
            ?? User?.FindFirst("preferred_username")?.Value
            ?? string.Empty;

        public string Name =>
            User?.FindFirst(ClaimTypes.GivenName)?.Value
            ?? User?.FindFirst("name")?.Value
            ?? string.Empty;

        public string Email =>
            User?.FindFirst(ClaimTypes.Email)?.Value
            ?? User?.FindFirst("email")?.Value
            ?? string.Empty;

        public string RoleId
        {
            get
            {
                // 取得第一個角色作為主要角色
                var roleClaim = User?.FindFirst(ClaimTypes.Role)?.Value
                    ?? User?.FindFirst("role")?.Value;

                return roleClaim ?? string.Empty;
            }
        }

        public IEnumerable<string> RoleIds
        {
            get
            {
                if (User == null)
                    return Enumerable.Empty<string>();

                // 取得所有角色 Claims
                var roleClaims = User.FindAll(ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                // 如果沒有使用標準 ClaimTypes.Role，嘗試使用 "role"
                if (!roleClaims.Any())
                {
                    roleClaims = User.FindAll("role")
                        .Select(c => c.Value)
                        .ToList();
                }

                return roleClaims;
            }
        }

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public IEnumerable<Claim> Claims => User?.Claims ?? Enumerable.Empty<Claim>();

        public bool HasRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return false;

            return RoleIds.Any(r => r.Equals(roleId, StringComparison.OrdinalIgnoreCase));
        }

        public bool HasAnyRole(params string[] roleIds)
        {
            if (roleIds == null || roleIds.Length == 0)
                return false;

            var userRoles = RoleIds.ToList();
            return roleIds.Any(roleId => 
                userRoles.Any(r => r.Equals(roleId, StringComparison.OrdinalIgnoreCase)));
        }

        public bool HasAllRoles(params string[] roleIds)
        {
            if (roleIds == null || roleIds.Length == 0)
                return true;

            var userRoles = RoleIds.ToList();
            return roleIds.All(roleId => 
                userRoles.Any(r => r.Equals(roleId, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
