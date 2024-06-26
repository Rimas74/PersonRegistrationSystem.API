﻿using Microsoft.EntityFrameworkCore;
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

    public async Task<bool> PersonalCodeExistsForUserAsync(int userId, string personalCode)
    {
        return await _context.Persons.AnyAsync(p => p.UserId == userId && p.PersonalCode == personalCode);
    }

    public async Task CreateAsync(Person person)
    {
        _logger.LogInformation($"Creating person for user ID: {person.UserId} to the database.");
        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Person added for user ID: {person.UserId} to the database.");
    }
    public async Task<bool> EmailExistsAsync(string email)
    {
        _logger.LogInformation($"From database getting person by email: {email}.");
        return await _context.Persons.AnyAsync(p => p.Email == email);
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

            var placeOfResidence = await _context.PlacesOfResidence.FirstOrDefaultAsync(pr => pr.PersonId == person.Id);
            if (placeOfResidence != null)
            {
                _context.PlacesOfResidence.Remove(placeOfResidence);
                _logger.LogInformation($"Place of residence with ID: {placeOfResidence.Id} for person ID: {id} has been removed.");
            }


            if (!string.IsNullOrEmpty(person.ProfilePhotoPath) && File.Exists(person.ProfilePhotoPath))
            {
                File.Delete(person.ProfilePhotoPath);
                _logger.LogInformation($"Profile photo at path: {person.ProfilePhotoPath} for person ID: {id} has been deleted.");
            }
            else
            {
                _logger.LogInformation($"No profile photo found or file does not exist for person ID: {id}.");
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Person with ID: {person.Id} and related data has been removed from the database.");
        }
        else
        {
            _logger.LogWarning($"Person with ID: {id} not found in the database.");
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

    public async Task UpdatePersonDetailsAsync(Person person)
    {
        _logger.LogInformation($"Updating person details for person ID: {person.Id}");
        _context.Update(person);  //might be more accurateopyion:  _context.Entry(person).State = EntityState.Modified
        if (person.PlaceOfResidence != null)
        {
            await UpdatePlaceOfResidenceAsync(person.PlaceOfResidence);
        }
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Person details updated for person ID: {person.Id}");
    }


    public async Task UpdatePlaceOfResidenceAsync(PlaceOfResidence placeOfResidence)
    {
        _logger.LogInformation($"Updating place of residence for person ID: {placeOfResidence.PersonId}");
        _context.PlacesOfResidence.Update(placeOfResidence);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Place of residence updated for person ID: {placeOfResidence.PersonId}");
    }

}
