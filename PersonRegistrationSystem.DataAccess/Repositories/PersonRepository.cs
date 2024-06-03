using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly PersonRegistrationContext _context;
        private readonly ILogger<PersonRepository> _logger;

        public PersonRepository(PersonRegistrationContext context, ILogger<PersonRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateAsync(Person person)
        {
            _logger.LogInformation($"Adding person for user ID: {person.UserId}");
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Person added for user ID: {person.UserId}");
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Person>> GetAllPersonsByUserIdAsync(int userId)
        {
            _logger.LogInformation("Retrieving all persons for user ID: {UserId}", userId);
            return await _context.Persons
                .Include(p => p.PlaceOfResidence)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Person> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving person with ID: {Id} from the database.", id);
            return await _context.Persons
                .Include(p => p.PlaceOfResidence)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task UpdateAsync(Person person)
        {
            throw new NotImplementedException();
        }
    }
}
