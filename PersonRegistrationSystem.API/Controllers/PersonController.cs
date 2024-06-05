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
                if (person == null)
                {
                    return NotFound("Person not found.");
                }
                return Ok(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the person.");
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
