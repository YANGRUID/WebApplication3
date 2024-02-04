using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http; // Required for IFormFile

namespace WebApplication3.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

 
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(12, ErrorMessage = "Password must be at least 12 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%#?&])[A-Za-z\d@$!%*#?&]+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Credit Card No is required.")]
        [RegularExpression(@"^\d{12,19}$", ErrorMessage = "Invalid Credit Card Number.")]
        [DataType(DataType.CreditCard)]
        public string CreditCard { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Mobile No is required.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Invalid Mobile Number. Please enter a 8-digit number.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Delivery Address is required.")]
        [StringLength(255, ErrorMessage = "Delivery Address cannot exceed 255 characters.")]
        public string DeliveryAddress { get; set; }

        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg" }, ErrorMessage = "Only JPG files are allowed.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Maximum file size allowed is 5 MB.")]
        public IFormFile Photo { get; set; }

        public string AboutMe { get; set; }

        // Additional properties and validation as needed
    }
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        public override bool IsValid(object value)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                return _extensions.Contains(extension);
            }
            return false;
        }
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxFileSize;

        public MaxFileSizeAttribute(long maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        public override bool IsValid(object value)
        {
            if (value is IFormFile file)
            {
                return file.Length <= _maxFileSize;
            }
            return false;
        }
    }
}
