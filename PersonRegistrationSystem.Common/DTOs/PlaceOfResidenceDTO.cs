using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class PlaceOfResidenceDTO
    {
        public int Id { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public string? ApartmentNumber { get; set; }
    }
}
