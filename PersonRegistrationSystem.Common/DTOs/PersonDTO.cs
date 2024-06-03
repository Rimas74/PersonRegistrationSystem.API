using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class PersonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string PersonalCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string Email { get; set; }
        public string ProfilePhotoPath { get; set; }
        public PlaceOfResidenceDTO PlaceOfResidence { get; set; }
    }
}
