using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class PlaceOfResidenceCreateDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "City name is too long.")]
        [RegularExpression(@"^[a-zA-ZąčęėįšųūžĄČĘĖĮŠŲŪŽ]+(?: [a-zA-ZąčęėįšųūžĄČĘĖĮŠŲŪŽ]+)*$", ErrorMessage = "City name can only contain letters and a single space between words.")]
        public string City { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Street name is too long.")]
        [RegularExpression(@"^[a-zA-ZąčęėįšųūžĄČĘĖĮŠŲŪŽ]+(?:\s[a-zA-ZąčęėįšųūžĄČĘĖĮŠŲŪŽ]+)+$", ErrorMessage = "Street name must contain at least one space and can only contain letters and spaces.")]
        public string Street { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]+[A-Za-z]?$", ErrorMessage = "House number is not valid.")]
        public string HouseNumber { get; set; }

        [RegularExpression(@"^[0-9]+[A-Za-z]?$", ErrorMessage = "Apartment number is not valid.")]
        public string? ApartmentNumber { get; set; }
    }
}
