using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PersonRegistrationSystem.DataAccess.Helpers
{
    public class PersonalCodeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var code = value as string;
            if (string.IsNullOrEmpty(code) || !Validate(code))
            {
                return new ValidationResult("Invalid Personal Identification Code.");
            }
            return ValidationResult.Success;
        }

        private bool Validate(string code)
        {
            if (code.Length != 11 || !IsAllDigits(code))
                return false;

            if (!IsValidDate(code))
                return false;

            int checksum = CalculateChecksum(code, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 });
            if (checksum == 10)
            {
                checksum = CalculateChecksum(code, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2, 3 });
                if (checksum == 10)
                    checksum = 0;
            }
            return checksum == int.Parse(code[10].ToString());
        }

        private bool IsAllDigits(string code)
        {
            return code.All(char.IsDigit);
        }

        private bool IsValidDate(string code)
        {
            int g = int.Parse(code[0].ToString());
            int year;

            if (g % 2 == 0)
            {
                year = (g / 2 - 1) * 100 + 1800 + int.Parse(code.Substring(1, 2));
            }
            else
            {
                year = (g / 2) * 100 + 1800 + int.Parse(code.Substring(1, 2));
            }

            string datePart = $"{year}-{code.Substring(3, 2)}-{code.Substring(5, 2)}";
            return DateTime.TryParse(datePart, out _);
        }

        private int CalculateChecksum(string code, int[] weights)
        {
            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(code[i].ToString()) * weights[i];
            }
            return sum % 11;
        }
    }
}
