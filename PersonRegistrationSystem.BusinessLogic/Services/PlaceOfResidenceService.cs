using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Services
{
    public class PlaceOfResidenceService : IPlaceOfResidenceService
    {
        public Task<PlaceOfResidenceDTO> UpdatePlaceOfResidenceAsync(int personId, PlaceOfResidenceUpdateDTO placeOfResidenceUpdateDTO)
        {
            throw new NotImplementedException();
        }
    }
}
