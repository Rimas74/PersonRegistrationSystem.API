using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonRegistrationSystem.Common.Validators;
using PersonRegistrationSystem.DataAccess.Helpers;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class PersonCreateDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy MMM dd}")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required]
        [PersonalCodeValidation]
        public string PersonalCode { get; set; }

        [Required]
        [RegularExpression(@"^\+370\d{8}$", ErrorMessage = "Phone number format is invalid for Lithuanian numbers.")]
        public string TelephoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        [CustomEmailDomainValidation]
        public string Email { get; set; }

        [Required]
        [MaxFileSize(4 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile ProfilePhoto { get; set; }

        [Required]
        public PlaceOfResidenceCreateDTO PlaceOfResidence { get; set; }
    }
}
