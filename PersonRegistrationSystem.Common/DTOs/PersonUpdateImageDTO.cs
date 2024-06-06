using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class PersonUpdateImageDTO
    {
        [Required]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile ProfilePhoto { get; set; }
    }
}
