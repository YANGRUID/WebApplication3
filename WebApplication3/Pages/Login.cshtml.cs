using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using System;
using Microsoft.Extensions.Logging;
using WebApplication3.Services;
using System.Web;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly UserManager<ApplicationUser> userManager;

        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly AuditLogService auditLogService;
        public LoginModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, AuditLogService auditLogService)
        {
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.auditLogService = auditLogService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var identityResult = await signInManager.PasswordSignInAsync(HttpUtility.HtmlEncode(LModel.Email), HttpUtility.HtmlEncode(LModel.Password), false, lockoutOnFailure: true);

                if (identityResult.Succeeded)
                {
                    var session = httpContextAccessor.HttpContext.Session;

                    var user = await userManager.FindByEmailAsync(LModel.Email);
                    if (user != null)
                    {
                        string authToken = Guid.NewGuid().ToString();

                        HttpContext.Session.SetString("UserId", LModel.Email);
                        HttpContext.Session.SetString("AuthToken", authToken);
                        Response.Cookies.Append("AuthToken", authToken);

                        user.AuthToken = authToken;
                        await userManager.UpdateAsync(user);

                        auditLogService.LogAudit(user.Id, "Login", "User logged in");

                        return RedirectToPage("Index");
                    }
                }

                else if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account is locked out, please try again in 1 minute");
                    return Page();
                }
                ModelState.AddModelError("", "Username or Password incorrect");
            }

            return Page();
        }
    }
}
