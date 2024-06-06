using System.ComponentModel.DataAnnotations;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class PlaceOfResidenceUpdateDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "City name is too long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City name can only contain letters and spaces.")]
        public string City { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Street name is too long.")]
        [RegularExpression(@"^[a-zA-Z]+(?:\s[a-zA-Z]+)+$", ErrorMessage = "Street name must contain at least one space and can only contain letters and spaces.")]
        public string Street { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]+[A-Za-z]?$", ErrorMessage = "House number is not valid.")]
        public string HouseNumber { get; set; }

        [RegularExpression(@"^[0-9]+[A-Za-z]?$", ErrorMessage = "Apartment number is not valid.")]
        public string? ApartmentNumber { get; set; }
    }

}