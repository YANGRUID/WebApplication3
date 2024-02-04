using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApplication3.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [EmailAddress]
        public override string Email { get; set; }
        public string FullName { get; set; }
        public string CreditCard { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string DeliveryAddress { get; set; }
        public string AboutMe { get; set; }
        public string? Photo { get; set; }
        public string? AuthToken { get; set; }
        public string PasswordHistory { get; set; }
    }
}
