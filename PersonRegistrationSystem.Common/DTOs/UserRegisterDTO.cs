using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PersonRegistrationSystem.Common.Validators;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class UserRegisterDTO
    {
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 20 characters.")]
        public string Username { get; set; }

        [Required]
        [PasswordComplexity]
        public string Password { get; set; }


    }
}
