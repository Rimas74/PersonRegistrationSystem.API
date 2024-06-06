using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.Common.DTOs;
using System.Security.Claims;

namespace PersonRegistrationSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IPersonService personService, ILogger<PersonController> logger)
        {
            _personService = personService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPerson()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }
            try
            {
                _logger.LogInformation($"Retrieving persons for user ID: {userId.Value}");
                var persons = await _personService.GetAllPersonsByUserIdAsync(userId.Value);

                if (persons == null || !persons.Any())
                {
                    _logger.LogInformation($"No persons found for user ID: {userId.Value}");
                    return Ok("No persons found.");
                }
                return Ok(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving persons for user ID: {userId.Value}");
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpGet("{id}")]

        public async Task<IActionResult> GetPersonById(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var person = await _personService.GetPersonByIdAsync(userId.Value, id);
                return Ok(person);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the person.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}/photo")]
        public async Task<IActionResult> GetPersonPicture(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }

            _logger.LogInformation($"Request to get picture for person ID: {id} by user ID: {userId}");

            try
            {
                var personImageDTO = await _personService.GetPersonImageAsync(userId.Value, id);

                if (personImageDTO == null || personImageDTO.ProfilePhoto == null)
                {
                    _logger.LogWarning($"Picture for person ID: {id} by user ID: {userId} not found.");
                    return NotFound("Person or picture not found.");
                }

                var fileName = Path.GetFileName(personImageDTO.ProfilePhotoPath);

                _logger.LogInformation($"Picture {fileName} for person ID: {id} by user ID: {userId} retrieved successfully.");

                return File(personImageDTO.ProfilePhoto, "image/jpeg", fileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Forbid(); // Call Forbid without a scheme
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the person picture.");
                return StatusCode(500, "Internal server error.");
            }
        }



        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromForm] PersonCreateDTO personCreateDTO)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var createdPerson = await _personService.CreatePersonAsync(userId.Value, personCreateDTO);
                return CreatedAtAction(nameof(GetPersonById), new { id = createdPerson.Id }, createdPerson);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the person.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var deletedPerson = await _personService.DeletePersonAsync(userId.Value, id);
                return Ok(deletedPerson);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the person.");
                return StatusCode(500, "Internal server error.");
            }
        }

        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID claim not found.");
                foreach (var claim in User.Claims)
                {
                    _logger.LogInformation($"Claim type: {claim.Type}, value: {claim.Value}");
                }
                return null;
            }
            return int.Parse(userIdClaim.Value);
        }


    }
}
