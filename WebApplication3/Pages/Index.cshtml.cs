using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace WebApplication3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public string DecryptedCreditCard { get; private set; }
        public ApplicationUser CurrentUser { get; private set; }

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            IDataProtectionProvider dataProtectionProvider)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser == null)
            {
                return RedirectToPage("/Login");
            }

            var sessionUserId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            var sessionAuthToken = _httpContextAccessor.HttpContext.Session.GetString("AuthToken");
            var cookieAuthToken = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

            if (sessionUserId != CurrentUser.Id || sessionAuthToken != CurrentUser.AuthToken)
            {
                await LogOutUserAsync();
                return RedirectToPage("/Login");
            }

            if (cookieAuthToken != CurrentUser.AuthToken)
            {
                await LogOutUserAsync();
                return RedirectToPage("/Login");
            }

            var protector = _dataProtectionProvider.CreateProtector("MySecretKey");
            DecryptedCreditCard = protector.Unprotect(CurrentUser.CreditCard);

            return Page();
        }

        private async Task LogOutUserAsync()
        {
            CurrentUser.AuthToken = null;
            await _userManager.UpdateAsync(CurrentUser);
            await _signInManager.SignOutAsync();
            _httpContextAccessor.HttpContext.Session.Remove("UserId");
            _httpContextAccessor.HttpContext.Session.Remove("AuthToken");
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("AuthToken");
        }
    }
}
