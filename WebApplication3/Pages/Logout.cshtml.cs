using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using Microsoft.AspNetCore.Http;
using WebApplication3.Services;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditLogService auditLogService;
        public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, AuditLogService auditLogService)
        {
            this.signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            this.auditLogService = auditLogService;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("AuthToken");
            Response.Cookies.Delete("AuthToken");
            var user = await _userManager.GetUserAsync(User);
            user.AuthToken = null;
            await _userManager.UpdateAsync(user);
            auditLogService.LogAudit(user.Id, "Logout", "User logged out");

            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
