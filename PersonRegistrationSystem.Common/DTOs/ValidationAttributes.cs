using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.Common.DTOs
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
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize) => _maxFileSize = maxFileSize;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null && file.Length > _maxFileSize)
                return new ValidationResult($"Maximum allowed file size is {_maxFileSize} bytes.");

            return ValidationResult.Success;
        }
    }
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_extensions.Contains(extension))
                    return new ValidationResult($"Invalid file extension. Allowed extensions are: {string.Join(", ", _extensions)}");
            }

            return ValidationResult.Success;
        }
    }
}
