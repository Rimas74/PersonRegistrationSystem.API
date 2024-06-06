using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.Common.DTOs
{
    public class PersonGetImageDTO
    {
        public byte[] ProfilePhoto { get; set; }
        public string ProfilePhotoPath { get; set; }
    }
}
