using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 20 characters.")]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Salt { get; set; }

        [Required]
        public string Role { get; set; } // 'Admin' or 'User'

        public ICollection<Person> Persons { get; set; }
    }
}
