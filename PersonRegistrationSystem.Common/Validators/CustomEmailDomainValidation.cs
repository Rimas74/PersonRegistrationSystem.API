using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class CustomEmailDomainValidation : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var email = value as string;
        if (email == null)
        {
            return new ValidationResult("Email is required.");
        }

        var domainPattern = @"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$";
        if (!Regex.IsMatch(email, domainPattern))
        {
            return new ValidationResult("Email format is invalid.");
        }

        return ValidationResult.Success;
    }
}
