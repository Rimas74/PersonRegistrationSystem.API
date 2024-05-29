using PersonRegistrationSystem.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic.Interfaces
{
    public interface IPlaceOfResidenceService
    {
        Task<PlaceOfResidenceDTO> UpdatePlaceOfResidenceAsync(int personId, PlaceOfResidenceUpdateDTO placeOfResidenceUpdateDTO);
    }
}
