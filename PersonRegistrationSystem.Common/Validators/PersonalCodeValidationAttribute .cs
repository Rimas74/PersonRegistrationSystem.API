using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PersonRegistrationSystem.Common.Validators
{
    public class PersonalCodeValidationAttribute : ValidationAttribute
    {
        public string GenderProperty { get; set; }
        public string BirthdayProperty { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var loggerFactory = validationContext.GetService<IServiceProvider>().GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<PersonalCodeValidationAttribute>();

            var code = value as string;
            var gender = validationContext.ObjectType.GetProperty(GenderProperty)?.GetValue(validationContext.ObjectInstance, null) as string;
            var birthday = validationContext.ObjectType.GetProperty(BirthdayProperty)?.GetValue(validationContext.ObjectInstance, null) as DateTime?;

            if (string.IsNullOrEmpty(code) || !Validate(code, gender, birthday, logger))
            {
                logger.LogWarning("Invalid Personal Identification Code.");
                return new ValidationResult("Invalid Personal Identification Code.");
            }
            return ValidationResult.Success;
        }

        private bool Validate(string code, string gender, DateTime? birthday, ILogger logger)
        {
            if (code.Length != 11 || !IsAllDigits(code))
            {
                logger.LogWarning("Personal code must be 11 digits.");
                return false;
            }

            if (!IsValidDate(code, birthday, logger))
            {
                logger.LogWarning("Personal code date does not match the entered birthday.");
                return false;
            }

            if (!IsValidGender(code, gender, logger))
            {
                logger.LogWarning("Personal code gender does not match the entered gender.");
                return false;
            }

            int checksum = CalculateChecksum(code, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 });
            if (checksum == 10)
            {
                checksum = CalculateChecksum(code, new[] { 3, 4, 5, 6, 7, 8, 9, 1, 2, 3 });
                if (checksum == 10)
                {
                    checksum = 0;
                }
            }

            bool isValidChecksum = checksum == int.Parse(code[10].ToString());
            if (!isValidChecksum)
            {
                logger.LogWarning("Invalid checksum for personal code.");
            }

            return isValidChecksum;
        }

        private bool IsAllDigits(string code)
        {
            return code.All(char.IsDigit);
        }

        private bool IsValidDate(string code, DateTime? birthday, ILogger logger)
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

            int month = int.Parse(code.Substring(3, 2));
            int day = int.Parse(code.Substring(5, 2));

            if (birthday.HasValue)
            {
                if (birthday.Value.Year != year || birthday.Value.Month != month || birthday.Value.Day != day)
                {
                    logger.LogWarning($"Birthday {birthday.Value} does not match personal code date {year}-{month}-{day}.");
                    return false;
                }
            }

            string datePart = $"{year}-{month}-{day}";
            return DateTime.TryParse(datePart, out _);
        }

        private bool IsValidGender(string code, string gender, ILogger logger)
        {
            int firstDigit = int.Parse(code[0].ToString());
            bool isValidGender = (gender == "Male" && (firstDigit == 3 || firstDigit == 5)) || (gender == "Female" && (firstDigit == 2 || firstDigit == 4));
            if (!isValidGender)
            {
                logger.LogWarning($"Gender {gender} does not match personal code first digit {firstDigit}.");
            }
            return isValidGender;
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
