using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.DataAccess;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class PersonRepository : IPersonRepository
{
    private readonly PersonRegistrationContext _context;
    private readonly ILogger<PersonRepository> _logger;

    public PersonRepository(PersonRegistrationContext context, ILogger<PersonRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> ExistsAsync(string personalCode)
    {
        return await _context.Persons.AnyAsync(p => p.PersonalCode == personalCode);
    }

    public async Task CreateAsync(Person person)
    {
        _logger.LogInformation($"Creating person for user ID: {person.UserId} to the database.");

        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Person added for user ID: {person.UserId} to the database.");
    }

    public async Task CreatePlaceOfResidenceAsync(PlaceOfResidence placeOfResidence)
    {
        _logger.LogInformation("Creating place of residence to the database.");

        await _context.PlacesOfResidence.AddAsync(placeOfResidence);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Place of residence added to the database.");
    }

    public async Task DeleteAsync(int id)
    {
        var person = await _context.Persons.FindAsync(id);
        if (person != null)
        {
            var placeOfResidence = await _context.PlacesOfResidence.FindAsync(person.PlaceOfResidence.Id);
            if (placeOfResidence != null)
            {
                _context.PlacesOfResidence.Remove(placeOfResidence);
            }
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();


            if (!string.IsNullOrEmpty(person.ProfilePhotoPath) && File.Exists(person.ProfilePhotoPath))
            {
                File.Delete(person.ProfilePhotoPath);
            }
        }
    }

    public async Task<IEnumerable<Person>> GetAllPersonsByUserIdAsync(int userId)
    {
        _logger.LogInformation($"Retrieving all persons for user ID: {userId} from the database.");
        return await _context.Persons
            .Include(p => p.PlaceOfResidence)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<Person> GetByIdAsync(int id)
    {
        _logger.LogInformation($"Retrieving person with ID: {id} from the database.");
        return await _context.Persons
            .Include(p => p.PlaceOfResidence)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateAsync(Person person)
    {
        _context.Persons.Update(person);
        await _context.SaveChangesAsync();
    }
}
