using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Helpers
{
    public static class PersonalCodeValidator
    {
        public static bool Validate(string code)
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

        private static bool IsAllDigits(string code)
        {
            return code.All(char.IsDigit);
        }

        private static bool IsValidDate(string code)
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

        private static int CalculateChecksum(string code, int[] weights)
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
