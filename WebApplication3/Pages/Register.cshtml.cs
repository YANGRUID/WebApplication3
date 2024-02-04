using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using System.Text.RegularExpressions;
using System.Web;
using WebApplication3.Model;


namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IWebHostEnvironment _hostingEnv;

        [BindProperty]
        public Register RegisterData { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             IWebHostEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _hostingEnv = hostingEnvironment;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var existingUser = await userManager.FindByEmailAsync(RegisterData.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email already in use");
                    return Page();
                }

                string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$";
                if (!Regex.IsMatch(RegisterData.Password, passwordPattern))
                {
                    ModelState.AddModelError("RegisterData.Password", "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
                    return Page();
                }

                var user = new ApplicationUser()
                {
                    UserName = HttpUtility.HtmlEncode(RegisterData.Email),
                    FullName = HttpUtility.HtmlEncode(RegisterData.FullName),
                    Email = HttpUtility.HtmlEncode(RegisterData.Email),
                    CreditCard = protector.Protect(HttpUtility.HtmlEncode(RegisterData.CreditCard)),
                    Gender = HttpUtility.HtmlEncode(RegisterData.Gender),
                    MobileNo = HttpUtility.HtmlEncode(RegisterData.MobileNo),
                    DeliveryAddress = HttpUtility.HtmlEncode(RegisterData.DeliveryAddress),
                    AboutMe = HttpUtility.HtmlEncode(RegisterData.AboutMe),
                };

                var result = await userManager.CreateAsync(user, RegisterData.Password);

                if (result.Succeeded)
                {

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return Page();
        }

        // Method to handle photo upload (if needed, based on your ViewModel)
        private async Task HandlePhotoUpload(IFormFile photo, string email)
        {
            if (photo != null && photo.Length > 0)
            {
                // Define the directory where photos will be stored
                var uploadsDirectory = Path.Combine(_hostingEnv.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                // Create a unique name for the file
                var fileName = $"{email}_{DateTime.UtcNow:yyyyMMddHHmmss}_{photo.FileName}";
                var filePath = Path.Combine(uploadsDirectory, fileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }

                // Update the RegisterData record with the file path
                var RegisterData = await userManager.FindByEmailAsync(email);
                if (RegisterData != null)
                {
                    RegisterData.Photo = fileName;
                    await userManager.UpdateAsync(RegisterData);
                }
            }

        }
    }
}
