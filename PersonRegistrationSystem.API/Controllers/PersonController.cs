using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
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
                var persons = await _personService.GetAllPersonsByUserIdAsync(userId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while retrieving users with id ={userId} all persons.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
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
