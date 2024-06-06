using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PersonRegistrationSystem.Common.Validators
{
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Password is required.");
            }

            var password = value.ToString();

            if (password.Length < 12)
            {
                return new ValidationResult("Password must be at least 12 characters long.");
            }

            if (!Regex.IsMatch(password, @"[A-Z].*[A-Z]")) // At least two uppercase letters
            {
                return new ValidationResult("Password must contain at least two uppercase letters.");
            }

            if (!Regex.IsMatch(password, @"[a-z].*[a-z]")) // At least two lowercase letters
            {
                return new ValidationResult("Password must contain at least two lowercase letters.");
            }

            if (!Regex.IsMatch(password, @"[0-9].*[0-9]")) // At least two digits
            {
                return new ValidationResult("Password must contain at least two digits.");
            }

            if (!Regex.IsMatch(password, @"[\W_].*[\W_]")) // At least two special characters
            {
                return new ValidationResult("Password must contain at least two special characters.");
            }

            return ValidationResult.Success;
        }
    }
}
