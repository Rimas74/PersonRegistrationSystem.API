﻿using PersonRegistrationSystem.Common.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class PersonUpdateDetailsDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last Name can only contain letters and spaces.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required. Gender must be either 'Male' or 'Female'.")]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required]
        [PersonalCodeValidation(GenderProperty = "Gender", BirthdayProperty = "Birthday")]
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
        public PlaceOfResidenceUpdateDTO PlaceOfResidence { get; set; }
    }

}
