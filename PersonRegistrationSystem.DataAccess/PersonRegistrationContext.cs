using Microsoft.EntityFrameworkCore;
using PersonRegistrationSystem.DataAccess.Entities;
using PersonRegistrationSystem.DataAccess.Helpers;

namespace PersonRegistrationSystem.DataAccess
{
    public class PersonRegistrationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PlaceOfResidence> PlacesOfResidence { get; set; }

        public PersonRegistrationContext(DbContextOptions<PersonRegistrationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Persons)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Person>()
               .HasIndex(p => p.Email)
               .IsUnique();

            modelBuilder.Entity<Person>()
                .HasOne(p => p.PlaceOfResidence)
                .WithOne(pr => pr.Person)
                .HasForeignKey<PlaceOfResidence>(pr => pr.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            var (passwordHash, salt) = PasswordHasher.CreatePasswordHash("AdminPassword2");

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 3,
                Username = "admin2",
                PasswordHash = passwordHash,
                Salt = salt,
                Role = "Admin"
            });
        }

    }
}
