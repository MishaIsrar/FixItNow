using FixItNow.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Services
{
    public sealed record CurrentUserInfo(int Id, string Type);

    public class CurrentUserService
    {
        private const string CookieName = "currentUser";
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _protector;

        public CurrentUserService(
            ApplicationDbContext db,
            IHttpContextAccessor httpContextAccessor,
            IDataProtectionProvider dataProtectionProvider)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _protector = dataProtectionProvider.CreateProtector("FixItNow.CurrentUser.v1");
        }

        public void SignIn(int id, string type)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            var payload = _protector.Protect($"{id}|{type}|{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
            httpContext.Response.Cookies.Append(CookieName, payload, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Secure = httpContext.Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }

        public void SignOut()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(CookieName);
        }

        public async Task<CurrentUserInfo?> GetCurrentUserAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null ||
                !httpContext.Request.Cookies.TryGetValue(CookieName, out var protectedPayload) ||
                string.IsNullOrWhiteSpace(protectedPayload))
            {
                return null;
            }

            try
            {
                var payload = _protector.Unprotect(protectedPayload);
                var parts = payload.Split('|');
                if (parts.Length < 2 || !int.TryParse(parts[0], out var id))
                {
                    SignOut();
                    return null;
                }

                var type = parts[1].Trim().ToLowerInvariant();
                var exists = type switch
                {
                    "admin" => id == 0,
                    "user" => await _db.Userss.AnyAsync(u => u.id == id),
                    "provider" => await _db.Providers.AnyAsync(p => p.Id == id),
                    _ => false
                };

                if (!exists)
                {
                    SignOut();
                    return null;
                }

                return new CurrentUserInfo(id, type);
            }
            catch
            {
                SignOut();
                return null;
            }
        }
    }
}
